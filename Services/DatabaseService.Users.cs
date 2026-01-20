using Microsoft.Data.Sqlite;

namespace ManaHub.Services
{
    sealed partial class DatabaseService
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
        
        public bool CheckExistUsername(string username)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT COUNT(*) FROM Users
                    WHERE Username = $user
                ";
                command.Parameters.AddWithValue("user", username);

                long count = (long)command.ExecuteScalar();
                return count > 0;
            }
        }

        public bool CreateUserAccount(string username, string password)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT INTO Users (username, password)
                        VALUES ($user, $pass)
                    ";
                    command.Parameters.Add("$user", SqliteType.Text).Value = username;
                    command.Parameters.Add("$pass", SqliteType.Text).Value = password;

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
