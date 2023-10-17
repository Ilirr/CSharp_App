using MySql.Data.MySqlClient;

namespace ObservationCSharp.DatabaseConnect
{
    public class DBConnecter
    {

        private static string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<DBConnecter>()
                .Build();

            return configuration["ConnectionString"];
        }

        public static MySqlConnection ConnectDatabase()
        {
            string connectionString = GetConnectionString();


            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}