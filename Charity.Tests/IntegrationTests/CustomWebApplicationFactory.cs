using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheCharityBLL.DTOs.PaymentDTOs;
using TheCharityBLL.Services.Abstraction.Payment;
using TheCharityDAL.Database;

namespace TaskManagement.Tests.IntegrationTests
{
    /// <summary>
    /// Boots the real API in-memory (Kestrel-free TestServer) but swaps the SQL Server/PostgreSQL
    /// DbContext registration for a SQLite connection that lives only in RAM for the life of the test class.
    /// Requires Program.cs to be reachable as a partial class, e.g. add this line at the bottom of
    /// Program.cs in the API project:  public partial class Program { }
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<TheCharityPL.Program>, IAsyncLifetime
    {
        public const string TestPaymobHmacKey = "test-hmac-secret-key-for-integration";

        // Keeping this connection open for the factory's lifetime is what keeps the in-memory DB alive
        // between requests — SQLite's ":memory:" db is destroyed the instant the connection closes.
        private readonly SqliteConnection _connection = new("DataSource=:memory:");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = "Data Source=:memory:",
                    ["Paymob:HmacKey"] = TestPaymobHmacKey
                });
            });
            builder.ConfigureServices(services =>
            {
                // Remove whatever DbContextOptions<TaskManagementDbContext> registration Program.cs added
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TheCharityDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<TheCharityDbContext>(options =>
                    options.UseSqlite(_connection));

                var paymobDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IPaymobService));
                if (paymobDescriptor != null)
                    services.Remove(paymobDescriptor);

                var fakePaymob = A.Fake<IPaymobService>();
                A.CallTo(() => fakePaymob.CreatePayment(
                        A<decimal>._,
                        A<PaymentOrderMetadata?>._,
                        A<BillingData?>._,
                        A<string>._))
                    .Returns("https://accept.paymob.com/api/acceptance/iframes/test");
                services.AddSingleton(fakePaymob);

                // Build the schema once per test class
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TheCharityDbContext>();
                db.Database.EnsureCreated();
            });
        }

        public async Task InitializeAsync() => await _connection.OpenAsync();

        public new async Task DisposeAsync()
        {
            await _connection.CloseAsync();
            await base.DisposeAsync();
        }
    }
}
