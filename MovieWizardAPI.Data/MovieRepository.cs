using MovieWizardAPI.Data.Interfaces;
using MovieWizardAPI.Models;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Data;

namespace MovieWizardAPI.Data
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public MovieRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("MovieWizardConnection");
        }

        public async Task<IEnumerable<MovieRequest>> GetAllMoviesAsync()
        {
            var allMovieList = new List<MovieRequest>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Movies";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            try
                            {

                                var MovieID = (int)reader["MovieID"];
                                var Title = (string)reader["Title"];
                                var Description = (string)reader["Description"];
                                var Budget = (decimal)reader["Budget"];
                                var IMDBRating = (double)reader["IMDBRating"];
                                var RottenTomatoesRating = (double)reader["RottenTomatoesRating"];
                                var Price = (decimal)reader["Price"];
                                var IsActive = (bool)reader["IsActive"];
                                var Poster = reader["Poster"] as byte[] ?? null;
                                var Revenue = (decimal)reader["Revenue"];
                                var CreatedAt = (DateTime)reader["CreatedAt"];
                                var ReleaseDate = (DateTime)reader["ReleaseDate"];
                                var UpdatedAt = reader["UpdatedAt"] != DBNull.Value ? (DateTime)reader["UpdatedAt"] : DateTime.MinValue;
                                var CreatedBy = reader["CreatedBy"] as string ?? "";
                                var UpdatedBy = reader["UpdatedBy"] as string ?? "";

                                var movie = new MovieRequest()
                                {
                                    MovieID = MovieID,
                                    Title = Title,
                                    Description = Description,
                                    Budget = Budget,
                                    IMDBRating = IMDBRating,
                                    RottenTomatoesRating = RottenTomatoesRating,
                                    Price = Price,
                                    IsActive = IsActive,
                                    Poster = Poster,
                                    Revenue = Revenue,
                                    CreatedAt = CreatedAt,
                                    ReleaseDate = ReleaseDate,
                                    UpdatedAt = UpdatedAt,
                                    CreatedBy = CreatedBy,
                                    UpdatedBy = UpdatedBy,

                                };
                                allMovieList.Add(movie);

                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                    }
                }
                return allMovieList;
            }
        }

        public async Task<int> AddMovieAsync(MovieRequest movie)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();

                using (SqlTransaction transaction = sqlConnection.BeginTransaction())
                using (SqlCommand command = sqlConnection.CreateCommand())
                {
                    command.Transaction = transaction;

                    try
                    {
                        // Insert into Movies table
                        command.CommandText = @"INSERT INTO Movies 
                                        (Title, ReleaseDate, Description, Poster, Budget, Revenue, IMDBRating, 
                                         RottenTomatoesRating, Price, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
                                        VALUES 
                                        (@Title, @ReleaseDate, @Description, @Poster, @Budget, @Revenue, @IMDBRating, 
                                         @RottenTomatoesRating, @Price, @IsActive, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy);
                                        SELECT SCOPE_IDENTITY();";

                        command.Parameters.AddWithValue("@Title", movie.Title);
                        command.Parameters.AddWithValue("@ReleaseDate", movie.ReleaseDate);
                        command.Parameters.AddWithValue("@Description", movie.Description);

                        command.Parameters.AddWithValue("@Budget", movie.Budget);
                        command.Parameters.AddWithValue("@Revenue", movie.Revenue);
                        command.Parameters.AddWithValue("@IMDBRating", movie.IMDBRating);
                        command.Parameters.AddWithValue("@RottenTomatoesRating", movie.RottenTomatoesRating);
                        command.Parameters.AddWithValue("@Price", movie.Price);
                        command.Parameters.AddWithValue("@IsActive", true);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        command.Parameters.AddWithValue("@CreatedBy", "AddMovieEndpoint");
                        command.Parameters.AddWithValue("@UpdatedBy", "AddMovieEndpoint");

                        if (movie.Poster == null)
                        {
                            command.Parameters.Add("@Poster", SqlDbType.VarBinary).Value = DBNull.Value;
                        }
                        else
                        {
                            command.Parameters.Add("@Poster", SqlDbType.VarBinary).Value = movie.Poster;
                        }

                        // Execute the query and get the new MovieId

                        int newMovieId = Convert.ToInt32(await command.ExecuteScalarAsync());
                        if (movie.Genres != null && movie.Genres.Any())
                        {
                            foreach (var genreId in movie.Genres)
                            {
                                // Insert into MovieGenre table
                                command.CommandText = "INSERT INTO MovieGenres (MovieId, GenreId) VALUES (@MovieId, @GenreId)";
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@MovieId", newMovieId);
                                command.Parameters.AddWithValue("@GenreId", genreId);
                                await command.ExecuteNonQueryAsync();
                            }
                        }

                        // Commit transaction
                        transaction.Commit();

                        return newMovieId;
                    }
                    catch
                    {
                        // Rollback transaction if any error occurs
                        transaction.Rollback();
                        throw;
                    }
                }
            }

        }
    }
}
