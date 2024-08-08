using MovieWizardAPI.Data.Interfaces;
using MovieWizardAPI.Models;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace MovieWizardAPI.Data
{
    public class MovieWizardRepository : IMovieRepository
    {

        private readonly string connectionString = "Server=(localdb)\\MovieWizardLocal;Database=MovieWizardADO;Trusted_Connection=True;MultipleActiveResultSets=true;";

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            var allMovieList = new List<Movie>();
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Movies";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var movie = new Movie()
                            {
                                MovieID = (int)reader["MovieID"],
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
                                UpdatedBy = (string)reader["UpdatedBy"]
                            };
                            allMovieList.Add(movie);
                        }
                      
                    }
                }
            }
            return allMovieList;
        }

    }
}
