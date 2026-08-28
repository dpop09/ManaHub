using Microsoft.Data.Sqlite;

namespace ManaHub.Services
{
    internal sealed partial class DatabaseService
    {
        public async Task<bool> CheckUserAsync(
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            // check if a user exists given username and password
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
                        SELECT COUNT(*) FROM Users 
                        WHERE Username = $user AND Password = $pass
                    ";
                command.Parameters.AddWithValue("user", username);
                command.Parameters.AddWithValue("pass", password);

                long count = (long)(await command.ExecuteScalarAsync(cancellationToken) ?? 0L);
                return count > 0;
            }
        }
        
        public async Task<bool> CheckExistUsernameAsync(
            string username,
            CancellationToken cancellationToken = default)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT COUNT(*) FROM Users
                    WHERE Username = $user
                ";
                command.Parameters.AddWithValue("user", username);

                long count = (long)(await command.ExecuteScalarAsync(cancellationToken) ?? 0L);
                return count > 0;
            }
        }

        public async Task<bool> CreateUserAccountAsync(
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT INTO Users (username, password)
                        VALUES ($user, $pass)
                    ";
                    command.Parameters.Add("$user", SqliteType.Text).Value = username;
                    command.Parameters.Add("$pass", SqliteType.Text).Value = password;

                    int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
                    return rowsAffected > 0;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
