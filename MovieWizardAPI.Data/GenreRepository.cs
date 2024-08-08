using MovieWizardAPI.Data.Interfaces;
using MovieWizardAPI.Models;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace MovieWizardAPI.Data
{
    public class GenreRepository : IGenreRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public GenreRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("MovieWizardConnection");
        }

        public async Task<IEnumerable<MovieGenre>> GetAllGenresAsync()
        {
            var allGenreList = new List<MovieGenre>();
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Genre";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var genre = new MovieGenre()
                            {
                                GenreId = (int)reader["GenreId"],
                                GenreName = (string)reader["GenreName"]
                            };
                            allGenreList.Add(genre);
                        }
                      
                    }
                }
            }
            return allGenreList;
        }

    }
}

                              /*  MovieID = (int)reader["MovieID"],
                                Title = (string)reader["Title"],
                                Description = (string)reader["Description"],
                                Genre = (string)reader["Genre"],
                                Budget = (decimal)reader["Budget"],
                                IMDBRating = (decimal)reader["IMDBRating"],
                                RottenTomatoesRating = (decimal)reader["RottenTomatoesRating"],
                                Price = (decimal)reader["Price"],
                                IsActive = (bool)reader["IsActive"],
                                Poster = (byte[])reader["Poster"],
                                Revenue = (decimal)reader["Revenue"],
                                CreatedAt = (DateTime)reader["CreatedAt"],
                                ReleaseDate = (DateTime)reader["ReleaseDate"],
                                UpdatedAt = (DateTime)reader["UpdatedAt"],
                                CreatedBy = (string)reader["CreatedBy"],
                                UpdatedBy = (string)reader["UpdatedBy"]*/