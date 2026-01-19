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
        //public bool CreateUserAccount(string username, string password)
        //{
        //    try
        //    {
        //        using (var connection = new SqliteConnection(_connectionString))
        //        {
        //            // Ensure the connection is open before creating the command
        //            connection.Open();

        //            using (var command = connection.CreateCommand())
        //            {
        //                command.CommandText = @"
        //            INSERT INTO Users (Username, Password)
        //            VALUES ($user, $pass)";

        //                // Using specific SqliteType helps prevent mapping errors
        //                command.Parameters.Add("$user", SqliteType.Text).Value = username;
        //                command.Parameters.Add("$pass", SqliteType.Text).Value = password;

        //                // ExecuteNonQuery returns the number of rows affected
        //                int rowsAffected = command.ExecuteNonQuery();
        //                return rowsAffected > 0;
        //            }
        //        }
        //    }
        //    catch (SqliteException ex)
        //    {
        //        // SQLite error 19 is 'Constraint' (Username already exists)
        //        if (ex.SqliteErrorCode == 19) return false;

        //        throw; // Other DB errors
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // This catches the specific error you're seeing for debugging
        //        System.Diagnostics.Debug.WriteLine($"DB Error: {ex.Message}");
        //        return false;
        //    }
        //}
    }
}
