using Broker_Projekt_Zaliczeniowy.Controllers;
using Broker_Projekt_Zaliczeniowy.Models;
using Broker_Projekt_Zaliczeniowy.ModelsDto;
using Broker_Projekt_Zaliczeniowy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;


namespace Broker_Projekt_Zaliczeniowy.Tests
{
    public class TransactionControllerTests
    {
        private ProjektBdContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ProjektBdContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ProjektBdContext(options);
        }


        [Fact]
        public async Task BuyStock_CreatesNewPosition_WhenUserHasFunds()
        {
            var userId = 1;
            var assetId = 10;
            var initialBalance = 1000m;
            var stockPrice = 100m;
            var quantityToBuy = 5;

            var mockUserService = new Mock<IUserContextService>();
            mockUserService.Setup(s => s.GetUserId()).Returns(userId);

            using (var context = GetDatabaseContext())
            {
                var asset = new Asset { Id = assetId, Ticker = "NVDA", FullName = "Nvidia" , Type = "Stock" };
                context.Assets.Add(asset);

                var account = new Account { Id = 1, UserId = userId, Balance = initialBalance, Positions = new List<Position>() };
                context.Accounts.Add(account);

                context.MarketData.Add(new MarketDatum
                {
                    AssetId = assetId,
                    DataDate = DateOnly.FromDateTime(DateTime.Now),
                    Close = stockPrice
                });

                await context.SaveChangesAsync();

                var controller = new TransactionController(context, mockUserService.Object);
                var request = new TransactionRequestDto { AssetId = assetId, Quantity = quantityToBuy };

                var result = await controller.BuyStock(request);

                Assert.IsType<OkObjectResult>(result);

                var updatedAccount = await context.Accounts.Include(a => a.Positions).FirstAsync();
                Assert.Equal(500m, updatedAccount.Balance);

                Assert.Single(updatedAccount.Positions);
                var position = updatedAccount.Positions.First();
                Assert.Equal(quantityToBuy, position.Quantity);
                Assert.Equal(stockPrice, position.AverageCost);

                var transaction = await context.Transactions.FirstAsync();
                Assert.Equal("BUY", transaction.Type);
                Assert.Equal(500m, transaction.TotalAmount);
            }
        }

        [Fact]
        public async Task BuyStock_UpdatesAverageCost_WhenPositionAlreadyExists()
        {
            var userId = 1;
            var assetId = 10;

            var initialQty = 10;
            var initialAvgCost = 50m;

            var newQty = 10;
            var newPrice = 100m;

            var expectedAvgCost = 75m;

            var mockUserService = new Mock<IUserContextService>();
            mockUserService.Setup(s => s.GetUserId()).Returns(userId);

            using (var context = GetDatabaseContext())
            {
                var account = new Account
                {
                    Id = 1,
                    UserId = userId,
                    Balance = 2000m,
                    Positions = new List<Position>
                    {
                        new Position { AssetId = assetId, Quantity = initialQty, AverageCost = initialAvgCost }
                    }
                };
                context.Accounts.Add(account);

                context.MarketData.Add(new MarketDatum { AssetId = assetId, DataDate = DateOnly.FromDateTime(DateTime.Now), Close = newPrice });
                await context.SaveChangesAsync();

                var controller = new TransactionController(context, mockUserService.Object);
                var request = new TransactionRequestDto { AssetId = assetId, Quantity = newQty };

                await controller.BuyStock(request);

           
                var position = await context.Positions.FirstAsync();
                Assert.Equal(initialQty + newQty, position.Quantity); 
                Assert.Equal(expectedAvgCost, position.AverageCost); 
            }
        }

        [Fact]
        public async Task BuyStock_ReturnsBadRequest_WhenInsufficientFunds()
        {
            
            var userId = 1;
            var mockUserService = new Mock<IUserContextService>();
            mockUserService.Setup(s => s.GetUserId()).Returns(userId);

            using (var context = GetDatabaseContext())
            {
                context.Accounts.Add(new Account { Id = 1, UserId = userId, Balance = 100m, Positions = new List<Position>() });
                context.MarketData.Add(new MarketDatum { AssetId = 1, DataDate = DateOnly.FromDateTime(DateTime.Now), Close = 200m });
                await context.SaveChangesAsync();

                var controller = new TransactionController(context, mockUserService.Object);
                var request = new TransactionRequestDto { AssetId = 1, Quantity = 1 }; 

                 
                var result = await controller.BuyStock(request);

                var badRequest = Assert.IsType<BadRequestObjectResult>(result);
                var val = badRequest.Value;
                var msg = val.GetType().GetProperty("message").GetValue(val, null);
                Assert.Equal("Niewystarczające środki", msg);
            }
        }

       
        [Fact]
        public async Task SellStock_CalculatesProfitAndUpdatesBalance_Correctly()
        {

            var userId = 1;
            var assetId = 5;
            var transactionId = 100;
            var buyPrice = 50m;   
            var sellPrice = 80m;  
            var quantityToSell = 5;

           

            var mockUserService = new Mock<IUserContextService>();
            mockUserService.Setup(s => s.GetUserId()).Returns(userId);

            using (var context = GetDatabaseContext())
            {
                var account = new Account
                {
                    Id = 1,
                    UserId = userId,
                    Balance = 0m,
                    Positions = new List<Position>
                    {
                        new Position { AssetId = assetId, Quantity = 10, AverageCost = 50m }
                    }
                };
                context.Accounts.Add(account);

                context.Transactions.Add(new Transaction
                {
                    Id = transactionId,
                    AccountId = 1,
                    AssetId = assetId,
                    Type = "BUY",
                    Quantity = 10,
                    Price = buyPrice
                });

                context.MarketData.Add(new MarketDatum { AssetId = assetId, DataDate = DateOnly.FromDateTime(DateTime.Now), Close = sellPrice });

                await context.SaveChangesAsync();

                var controller = new TransactionController(context, mockUserService.Object);
                var request = new SellBatchDto { AssetId = assetId, TransactionId = transactionId, Quantity = quantityToSell };

                var result = await controller.SellStock(request);

                
                Assert.IsType<OkObjectResult>(result);

                var updatedAccount = await context.Accounts.FirstAsync();
                Assert.Equal(400m, updatedAccount.Balance); 

          
                var sellTransaction = await context.Transactions.LastAsync(); 
                Assert.Equal("SELL", sellTransaction.Type);
                Assert.Equal(150m, sellTransaction.Profit);
                var oldTransaction = await context.Transactions.FindAsync(transactionId);
                Assert.Equal("BUY_SOLD", oldTransaction.Type);
            }
        }

        

        [Fact]
        public async Task GetPositionDetails_CalculatesWeightedAverageCorrectly()
        {
       
            var userId = 1;
            var ticker = "AAPL";
            var assetId = 1;

            var mockUserService = new Mock<IUserContextService>();
            mockUserService.Setup(s => s.GetUserId()).Returns(userId);

            using (var context = GetDatabaseContext())
            {
                var asset = new Asset { Id = assetId, Ticker = ticker, FullName = "Apple", Type = "Stock" };
                context.Assets.Add(asset);

                var account = new Account { Id = 1, UserId = userId, Positions = new List<Position> { new Position { AssetId = assetId, Asset = asset } } };
                context.Accounts.Add(account);

       
                context.Transactions.AddRange(
                    new Transaction { AccountId = 1, AssetId = assetId, Type = "BUY", Quantity = 10, Price = 100, TotalAmount = 1000, TransactionDate = DateTime.Now.AddDays(-2) },
                    new Transaction { AccountId = 1, AssetId = assetId, Type = "BUY", Quantity = 10, Price = 200, TotalAmount = 2000, TransactionDate = DateTime.Now.AddDays(-1) }
                );

             
                context.MarketData.Add(new MarketDatum { AssetId = assetId, DataDate = DateOnly.FromDateTime(DateTime.Now), Close = 300m });

                await context.SaveChangesAsync();

                var controller = new TransactionController(context, mockUserService.Object);

            
                var result = await controller.GetPositionDetails(ticker);

           
                var actionResult = Assert.IsType<ActionResult<PositionDetailsViewModel>>(result);
                var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
                var viewModel = Assert.IsType<PositionDetailsViewModel>(okResult.Value);

                Assert.Equal(20, viewModel.CurrentQuantity); 
                Assert.Equal(150m, viewModel.AverageCost);   


                Assert.Equal(3000m, viewModel.Profit);
            }
        }
    }
}