using Broker_Projekt_Zaliczeniowy.Controllers;
using Broker_Projekt_Zaliczeniowy.Models;
using Broker_Projekt_Zaliczeniowy.ModelsDto;
using Broker_Projekt_Zaliczeniowy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;


namespace Broker_Projekt_Zaliczeniowy.Tests
{
    public class PortfolioControllerTests
    {
        private ProjektBdContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ProjektBdContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            return new ProjektBdContext(options);
        }

        [Fact]
        public async Task GetPortfolioTests_NoAccount()
        {
            var userId = 1;

            var mockUserService = new Mock<IUserContextService>();
            mockUserService.Setup(s => s.GetUserId()).Returns(userId);

            using (var context = GetDatabaseContext())
            {
                var controller = new PortfolioController(context, mockUserService.Object);

                var result = await controller.GetPortfolio();

                var actionResult = Assert.IsType<ActionResult<PortfolioDto>>(result);
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
                Assert.Equal("Nie znaleziono konta maklerskiego dla tego użytkownika", notFoundResult.Value);
            }
        }

        [Fact]
        public async Task GetPortfolio_Calculations()
        {
           
            var userId = 10;
            var assetId = 100;
            var initialBalance = 1000m;
            var quantity = 10;
            var averageCost = 50m; 
            var currentPrice = 75m;

            var mockUserService = new Mock<IUserContextService>();
            mockUserService.Setup(s => s.GetUserId()).Returns(userId);

            using (var context = GetDatabaseContext())
            {
                var asset = new Asset
                {
                    Id = assetId,
                    Ticker = "NVDA",
                    FullName = "Nvidia",
                    Type = "Stock"
                };
                context.Assets.Add(asset);
                context.Assets.Add(asset);

                var account = new Account
                {
                    Id = 1,
                    UserId = userId,
                    Balance = initialBalance,
                    Positions = new List<Position>
                    {
                        new Position
                        {
                            AssetId = assetId,
                            Asset = asset,
                            Quantity = quantity,
                            AverageCost = averageCost
                        }
                    }
                };
                context.Accounts.Add(account);

                context.MarketData.AddRange(
                 new MarketDatum { AssetId = assetId, DataDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)), Close = 40m },
                 new MarketDatum { AssetId = assetId, DataDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), Close = currentPrice }
             );

                await context.SaveChangesAsync();

                var controller = new PortfolioController(context, mockUserService.Object);

                var result = await controller.GetPortfolio();
                var actionResult = Assert.IsType<ActionResult<PortfolioDto>>(result);
                var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
                var dto = Assert.IsType<PortfolioDto>(okResult.Value);

                Assert.NotNull(dto);

                Assert.Equal(initialBalance, dto.CashBalance); 

                Assert.Single(dto.Positions); 
                var posDto = dto.Positions.First();

                Assert.Equal("NVDA", posDto.Ticker);

                Assert.Equal(currentPrice, posDto.CurrentPrice); 

                Assert.Equal(quantity * currentPrice, posDto.CurrentValue);

                var expectedProfit = (quantity * currentPrice) - (quantity * averageCost);
                Assert.Equal(expectedProfit, posDto.ProfitLoss);

                var expectedPercent = ((currentPrice - averageCost) / averageCost) * 100;
                Assert.Equal(expectedPercent, posDto.ProfitLossPercentage);

                Assert.Equal(posDto.CurrentValue, dto.TotalAssetsValue);
                Assert.Equal(initialBalance + posDto.CurrentValue, dto.TotalPortfolioValue);
            }
        }
    }
}