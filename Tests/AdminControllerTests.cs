using Broker_Projekt_Zaliczeniowy.Controllers;
using Broker_Projekt_Zaliczeniowy.Models;
using Broker_Projekt_Zaliczeniowy.ModelsDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Broker_Projekt_Zaliczeniowy.Tests
{
    public class AdminControllerTests
    {
        private ProjektBdContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ProjektBdContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ProjektBdContext(options);
        }

        [Fact]
        public async Task AddAsset_ReturnsOk_WhenAssetIsNew()
        {
            var dto = new AddAssetDto { Ticker = "TSLA", FullName = "Tesla Inc." };

            using (var context = GetDatabaseContext())
            {
                var controller = new AdminController(context);

                var result = await controller.AddAsset(dto);

                var okResult = Assert.IsType<OkObjectResult>(result);
                var assetInDb = await context.Assets.FirstOrDefaultAsync(a => a.Ticker == "TSLA");
                Assert.NotNull(assetInDb);
                Assert.Equal("Tesla Inc.", assetInDb.FullName);
                Assert.Equal("Stock", assetInDb.Type);
            }
        }

        [Fact]
        public async Task AddAsset_ReturnsConflict_WhenTickerAlreadyExists()
        {
            var existingTicker = "AAPL";
            using (var context = GetDatabaseContext())
            {
                context.Assets.Add(new Asset { Ticker = existingTicker, FullName = "Apple", Type = "Stock" });
                await context.SaveChangesAsync();

                var controller = new AdminController(context);
                var dto = new AddAssetDto { Ticker = existingTicker, FullName = "Apple Inc. New" };

                var result = await controller.AddAsset(dto);

                var conflictResult = Assert.IsType<ConflictObjectResult>(result);
                var val = conflictResult.Value;
                var msg = val.GetType().GetProperty("message").GetValue(val, null);
                Assert.Contains($"Spólka o tickerze {existingTicker} już istnieje", msg.ToString());
            }
        }

        [Fact]
        public async Task AddAsset_ReturnsBadRequest_WhenRequestIsNull()
        {
            using (var context = GetDatabaseContext())
            {
                var controller = new AdminController(context);
                var result = await controller.AddAsset(null);
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

        [Fact]
        public async Task GetUsers_ReturnsOnlyUsers_NotAdmins()
        {
            using (var context = GetDatabaseContext())
            {
                context.Users.Add(new User
                {
                    Id = 1,
                    Role = "Admin",
                    Email = "admin@test.com",
                    PasswordHash = "dummyHash123", 
                    Account = new Account { Balance = 0 }
                });

          
                context.Users.Add(new User
                {
                    Id = 2,
                    Role = "User",
                    Email = "user@test.com",
                    PasswordHash = "dummyHash456", 
                    CreatedAt = DateTime.Now,
                    Account = new Account { Balance = 1000m }
                });

                await context.SaveChangesAsync();

                var controller = new AdminController(context);

                var result = await controller.GetUsers();

                var actionResult = Assert.IsType<ActionResult<IEnumerable<UsersDataDto>>>(result);
                var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
                var usersList = Assert.IsAssignableFrom<IEnumerable<UsersDataDto>>(okResult.Value);

                Assert.Single(usersList);
                Assert.Equal("user@test.com", usersList.First().Email);
                Assert.Equal(1000m, usersList.First().Balance);
            }
        }

        [Fact]
        public async Task DeleteUser_ReturnsOk_WhenUserExistsAndIsNotAdmin()
        {
            var userId = 5;
            using (var context = GetDatabaseContext())
            {
              
                context.Users.Add(new User
                {
                    Id = userId,
                    Role = "User",
                    Email = "todelete@test.com",
                    PasswordHash = "secretHash", 
                    Account = new Account { Balance = 0 }
                });
                await context.SaveChangesAsync();

                var controller = new AdminController(context);

                var result = await controller.DeleteUser(userId);

                Assert.IsType<OkObjectResult>(result);

                var deletedUser = await context.Users.FindAsync(userId);
                Assert.Null(deletedUser);
            }
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            using (var context = GetDatabaseContext())
            {
                var controller = new AdminController(context);
                var result = await controller.DeleteUser(999);
                Assert.IsType<NotFoundObjectResult>(result);
            }
        }

        [Fact]
        public async Task DeleteUser_ReturnsBadRequest_WhenTryingToDeleteAdmin()
        {
            var adminId = 1;
            using (var context = GetDatabaseContext())
            {
                context.Users.Add(new User
                {
                    Id = adminId,
                    Role = "Admin",
                    Email = "admin@test.com",
                    PasswordHash = "adminSecretHash", 
                    Account = new Account { Balance = 0 }
                });
                await context.SaveChangesAsync();

                var controller = new AdminController(context);

                var result = await controller.DeleteUser(adminId);

                var badRequest = Assert.IsType<BadRequestObjectResult>(result);

                var val = badRequest.Value;
                var msg = val.GetType().GetProperty("message").GetValue(val, null);
                Assert.Equal("Nie można usunąć konta Administratora", msg);

                Assert.NotNull(await context.Users.FindAsync(adminId));
            }
        }
    }
}