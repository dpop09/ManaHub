using Microsoft.Data.Sqlite;

namespace ManaHub.Services
{
    internal sealed partial class DatabaseService
    {
        public bool CheckUser(string username, string password)
        {
            // check if a user exists given username and password
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
                        SELECT COUNT(*) FROM Users 
                        WHERE Username = $user AND Password = $pass
                    ";
                command.Parameters.AddWithValue("user", username);
                command.Parameters.AddWithValue("pass", password);

                long count = (long)command.ExecuteScalar();
                return count > 0;
            }
        }
    }
}
