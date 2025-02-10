using MovieWizardAPI.Data.Interfaces;
using MovieWizardAPI.Models;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Data;
using System.IO;

namespace MovieWizardAPI.Data
{
    public class DirectorRepository : IDirectorRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public DirectorRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("MovieWizardConnection");
        }

        public async Task<IEnumerable<Director>> GetAllDirectorsAsync()
        {
            var allDirectorList = new List<Director>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "Select * from Directors";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            try
                            {
                                var director = new Director();
                                director.DirectorID = (int)reader["DirectorId"];
                                director.DateOfBirth = (DateTime)reader["DateOfBirth"];
                                director.UpdatedAt = (DateTime)reader["UpdatedAt"];
                                director.CreatedAt = (DateTime)reader["CreatedAt"];
                                director.Bio = (string)reader["Bio"];
                                director.Name = (string)reader["Name"];
                                director.IsActive = (bool)reader["IsActive"];
                                director.ProfilePicture = (byte[])reader["ProfilePicture"];

                                allDirectorList.Add(director);
                            }

                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                    }
                }
                return allDirectorList;
            }
        }

        public async Task<int> AddDirectorAsync(AddDirectorRequest movieRequest)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"INSERT INTO Directors 
                                     (DirectorID,Name,Bio,DateOfBirth,IsActive,ProfilePicture,CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
                                    VALUES 
                                    (@DirectorID,@Name,@Bio,@DateOfBirth,@IsActive,@ProfilePicture,@CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        command.Parameters.AddWithValue("@Name", movieRequest.Name);
                        command.Parameters.AddWithValue("@Bio", movieRequest.Bio);
                        command.Parameters.AddWithValue("@DateOfBirth", movieRequest.DateOfBirth);
                        command.Parameters.AddWithValue("@IsActive", movieRequest.IsActive);
                        command.Parameters.AddWithValue("@ProfilePicture", movieRequest.ProfilePicture);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@CreatedBy", "AddDirector");
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@UpdatedBy", "AddDirector");

                        return await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
        }

        public async Task<int> GetDirectorIdByNameAsync(string directorName)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = @" SELECT DirectorId from Directors
                                   WHERE Name like @DirectorName and IsActive = 1";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DirectorName", "%" + directorName + "%");
                    var result = await command.ExecuteScalarAsync();

                    if (result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }
    }
}


