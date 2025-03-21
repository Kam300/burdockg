using Npgsql;
using System;

public class DatabaseHelper
{
    private string _connectionString = "Host=localhost;Port=5432;Database=лопух;Username=postgres;Password=00000000;";

    // Регистрация пользователя
    public bool RegisterUser(string lastName, string firstName, string middleName, string login, string password, string role)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("register_user", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_lastname", lastName);
                cmd.Parameters.AddWithValue("p_firstname", firstName);
                cmd.Parameters.AddWithValue("p_middlename", middleName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("p_login", login);
                cmd.Parameters.AddWithValue("p_password", password);
                cmd.Parameters.AddWithValue("p_role", role);
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (PostgresException ex)
                {
                    // Обработка ошибок (например, дубликат логина)
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
    }

    // Авторизация пользователя
    public (int UserId, string Role) AuthenticateUser(string login, string password)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("authenticate_user", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("p_login", login);
                cmd.Parameters.AddWithValue("p_password", password);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (reader.GetInt32(0), reader.GetString(1));
                    }
                    else
                    {
                        return (-1, null); // Пользователь не найден
                    }
                }
            }
        }
    }
}