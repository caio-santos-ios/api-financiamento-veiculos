using api_financiamento.src.Models;
using MongoDB.Driver;

namespace api_financiamento.src.Configuration
{
    public class AppDbContext
    {
        public static string? ConnectionString { get; set; }
        public static string? DatabaseName { get; set; }
        public static bool IsSSL { get; set; }
        private IMongoDatabase Database { get; }

        public AppDbContext()
        {
            try
            {
                MongoClientSettings mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));
                if (IsSSL)
                {
                    mongoClientSettings.SslSettings = new SslSettings
                    {
                        EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
                    };
                }

                var mongoClient = new MongoClient(mongoClientSettings);
                Database = mongoClient.GetDatabase(DatabaseName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to connect to database. Error: {ex.Message}");
            }
        }

        #region SIMULATOR
        public IMongoCollection<Simulation> Simulations => Database.GetCollection<Simulation>("simulations");
        #endregion
    }
}
