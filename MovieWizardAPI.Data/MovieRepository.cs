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
        public async Task<List<MovieResponseForGrid>> GetAllMoviesForGrid()
        {
            var movieListForGrid = new List<MovieResponseForGrid>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                 SELECT m.MovieID, m.Title, m.Description, m.ImdbRating, m.RottenTomatoesRating, 
                        m.Price, m.ReleaseDate, m.Poster,
                        d.Name AS DirectorName, 
                        a.Name AS ActorName, 
                        g.GenreName AS GenreName
                FROM Movies m
                        LEFT JOIN MovieDirectors md ON m.MovieID = md.MovieID
                        LEFT JOIN Directors d ON md.DirectorID = d.DirectorID
                        LEFT JOIN MovieActors ma ON m.MovieID = ma.MovieID
                        LEFT JOIN Actors a ON ma.ActorID = a.ActorID
                        LEFT JOIN MovieGenres mg ON m.MovieID = mg.MovieID
                        LEFT JOIN Genre g ON mg.GenreID = g.GenreID
                        WHERE m.IsActive = 1
                        ORDER BY m.MovieID;";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int? currentMovieID = null;
                        MovieResponseForGrid currentMovie = null;
                        var actors = new List<string>();
                        var genres = new List<string>();

                        while (reader.Read())
                        {
                            int movieID = reader.GetInt32(reader.GetOrdinal("MovieID"));

                            if (currentMovieID != movieID)
                            {
                                // Save the current movie and reset for the next movie
                                if (currentMovie != null)
                                {
                                    currentMovie.Actors = string.Join(", ", actors.Distinct());
                                    currentMovie.Genre = string.Join(", ", genres.Distinct());
                                    movieListForGrid.Add(currentMovie);
                                }                             

                                currentMovie = new MovieResponseForGrid
                                {
                                    MovieID = movieID,
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    ImdbRating = Convert.ToDecimal(reader.GetDouble(reader.GetOrdinal("ImdbRating"))),
                                    RottenTomatoesRating = Convert.ToDecimal(reader.GetDouble(reader.GetOrdinal("RottenTomatoesRating"))),
                                    Price = Convert.ToDecimal(reader.GetDecimal(reader.GetOrdinal("Price"))),
                                    ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                                    Director = reader.IsDBNull(reader.GetOrdinal("DirectorName")) ? "Unknown" : reader.GetString(reader.GetOrdinal("DirectorName"))
                                };

                                actors.Clear();
                                genres.Clear();

                                if (!reader.IsDBNull(reader.GetOrdinal("Poster")))
                                {
                                    byte[] posterBytes = (byte[])reader["Poster"];
                                    currentMovie.Poster = Convert.ToBase64String(posterBytes);
                                }

                                currentMovieID = movieID;
                            }

                            // Add actor and genre to the respective lists
                            if (!reader.IsDBNull(reader.GetOrdinal("ActorName")))
                            {
                                actors.Add(reader.GetString(reader.GetOrdinal("ActorName")));
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("GenreName")))
                            {
                                genres.Add(reader.GetString(reader.GetOrdinal("GenreName")));
                            }
                        }

                        // Add the last movie to the list
                        if (currentMovie != null)
                        {
                            currentMovie.Actors = string.Join(", ", actors.Distinct());
                            currentMovie.Genre = string.Join(", ", genres.Distinct());
                            movieListForGrid.Add(currentMovie);
                        }
                    }
                }
            }
            return movieListForGrid;
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
