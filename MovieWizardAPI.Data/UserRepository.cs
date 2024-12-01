using MovieWizardAPI.Data.Interfaces;
using MovieWizardAPI.Models;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Data;
using System.Numerics;

namespace MovieWizardAPI.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("MovieWizardConnection");
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var allUserList = new List<User>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Users";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                var user = new User
                                {
                                    UserID = (int)reader["UserId"],
                                    Username = (string)reader["Username"],
                                    Age = (int)reader["Age"],
                                    Email = (string)reader["Email"],
                                    Phone = (int)reader["Phone"],
                                    DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? (DateTime)reader["DateOfBirth"] : DateTime.MinValue,
                                    IsActive = (bool)reader["IsActive"],
                                    ProfilePicture = reader["ProfilePicture"] != DBNull.Value ? (byte[])reader["ProfilePicture"] : null,
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    UpdatedAt = (DateTime)reader["UpdatedAt"],
                                    CreatedBy = (string)reader["CreatedBy"],
                                    Nationality = (string)reader["Nationality"],
                                    UpdatedBy = (string)reader["UpdatedBy"]
                                };
                                allUserList.Add(user);

                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                    }
                }
                return allUserList;
            }
        }

        public async Task<(int totalRecords, IEnumerable<User> users)> GetPagedUsersAsync(int pageNumber, int pageSize)
        {
            List<User> users = new List<User>();
            int totalRecords = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Query to get the paged users
                string pagedQuery = @"
                SELECT *
                FROM Users
                ORDER BY UserID
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";

                // Query to get the total record count
                string countQuery = "SELECT COUNT(*) FROM Users";

                using (SqlCommand command = new SqlCommand(pagedQuery, connection))
                {
                    command.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
                    command.Parameters.AddWithValue("@PageSize", pageSize);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var user = new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Role = reader.GetString(reader.GetOrdinal("Role")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                ProfilePicture = reader.IsDBNull(reader.GetOrdinal("ProfilePicture")) ? null : reader["ProfilePicture"] as byte[],
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                                UpdatedBy = reader.GetString(reader.GetOrdinal("UpdatedBy")),
                                Nationality = reader.GetString(reader.GetOrdinal("Nationality")),
                                Age = reader.GetInt32(reader.GetOrdinal("Age")),
                                Phone = reader.GetInt32(reader.GetOrdinal("Phone")),
                                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),

                            };

                            users.Add(user);
                        }
                    }
                }

                // Get the total record count
                using (SqlCommand countCommand = new SqlCommand(countQuery, connection))
                {
                    totalRecords = (int)await countCommand.ExecuteScalarAsync();
                }
            }

            return (totalRecords, users);

        }

        public async Task<int> AddUserAsync(User addUserRequest)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                // Begin transaction and create command
                using (SqlTransaction transaction = sqlConnection.BeginTransaction())
                using (SqlCommand command = sqlConnection.CreateCommand())
                {
                    command.Transaction = transaction;

                    try
                    {
                        // Set up the command text for the INSERT operation
                        command.CommandText = @"
                    INSERT INTO Users 
                        (Username, Email, PasswordHash, Phone, Age, Nationality, 
                         ProfilePicture, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
                    VALUES 
                        (@Username, @Email, @PasswordHash, @Phone, @Age, @Nationality, @Role, 
                         @ProfilePicture, @IsActive, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy);
                    SELECT SCOPE_IDENTITY();"; // Return the new UserID

                        // Add parameters for the user data
                        command.Parameters.AddWithValue("@Username", addUserRequest.Username);
                        command.Parameters.AddWithValue("@Email", addUserRequest.Email);
                        command.Parameters.AddWithValue("@PasswordHash", addUserRequest.PasswordHash); // Assume password is already hashed
                        command.Parameters.AddWithValue("@Phone", addUserRequest.Phone ?? (object)DBNull.Value); // Allow null for phone
                        command.Parameters.AddWithValue("@Age", addUserRequest.Age ?? (object)DBNull.Value); // Allow null for age
                        command.Parameters.AddWithValue("@Nationality", addUserRequest.Nationality ?? (object)DBNull.Value); // Allow null for nationality
                        command.Parameters.AddWithValue("@IsActive", true);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@CreatedBy", "AddUserEndpoint");
                        command.Parameters.AddWithValue("@UpdatedBy", "AddUserEndpoint");
                        command.Parameters.AddWithValue("@Role", addUserRequest.Role);

                        // Handle ProfilePicture (nullable binary data)
                        if (addUserRequest.ProfilePicture == null)
                        {
                            command.Parameters.Add("@ProfilePicture", SqlDbType.VarBinary).Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.Add("@ProfilePicture", SqlDbType.VarBinary).Value = addUserRequest.ProfilePicture;
                        }

                        // Execute the query and get the new UserId
                        int newUserId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        // Commit transaction if successful
                        transaction.Commit();

                        return newUserId; // Return the new user ID
                    }
                    catch (Exception)
                    {
                        // Rollback transaction if any error occurs
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task UpdateUserAsync(User updateUser)
        {
            return;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                string query = "SELECT * FROM Users where Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Role = reader.GetString(reader.GetOrdinal("Role")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                ProfilePicture = reader.IsDBNull(reader.GetOrdinal("ProfilePicture")) ? null : reader["ProfilePicture"] as byte[],
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                                UpdatedBy = reader.GetString(reader.GetOrdinal("UpdatedBy")),
                                Nationality = reader.GetString(reader.GetOrdinal("Nationality")),
                                Age = reader.GetInt32(reader.GetOrdinal("Age")),
                                Phone = reader.GetInt32(reader.GetOrdinal("Phone")),
                                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                            };
                        }
                        else
                        {
                            return null;
                        }

                    }
                }
            }

        }
    }
}
