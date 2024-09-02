using MovieWizardAPI.Data.Interfaces;
using MovieWizardAPI.Models;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Linq.Expressions;
using System.Data;

namespace MovieWizardAPI.Data
{
    public class ActorRepository : IActorRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public ActorRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("MovieWizardConnection");
        }

        public async Task<List<Actor>> GetAllActorsAsync()
        {
            var allActorsList = new List<Actor>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Actors;";
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var actor = new Actor
                            {
                                ActorID = (int)reader["ActorId"],
                                Name = (string)reader["Name"],
                                Bio = reader["Bio"] != DBNull.Value ? (string)reader["Bio"] : null,
                                DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? (DateTime)reader["DateOfBirth"] : DateTime.MinValue,
                                IsActive = (bool)reader["IsActive"],
                                ProfilePicture = reader["ProfilePicture"] != DBNull.Value ? (byte[])reader["ProfilePicture"] : null,
                                CreatedAt = (DateTime)reader["CreatedAt"],
                                UpdatedAt = (DateTime)reader["UpdatedAt"],
                                CreatedBy = (string)reader["CreatedBy"],
                                Nationality = (string)reader["Nationality"],
                                UpdatedBy = (string)reader["UpdatedBy"]
                            };
                            allActorsList.Add(actor);
                        }
                    }
                }
            }

            return allActorsList;
        }
        public async Task<int> AddActorAsync(Actor addActorRequest)
        {
            int actorId = 0;


           using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    INSERT INTO Actors (Name, Bio, DateOfBirth, IsActive, ProfilePicture, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Nationality)
                    OUTPUT INSERTED.ActorID
                    VALUES (@Name, @Bio, @DateOfBirth, @IsActive, @ProfilePicture, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy, @Nationality);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", addActorRequest.Name);
                    command.Parameters.AddWithValue("@Bio", addActorRequest.Bio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@DateOfBirth", addActorRequest.DateOfBirth != DateTime.MinValue ? (object)addActorRequest.DateOfBirth : DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", addActorRequest.IsActive);
                    command.Parameters.AddWithValue("@CreatedAt", addActorRequest.CreatedAt);
                    command.Parameters.AddWithValue("@UpdatedAt", addActorRequest.UpdatedAt);
                    command.Parameters.AddWithValue("@CreatedBy", addActorRequest.CreatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", addActorRequest.UpdatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Nationality", addActorRequest.Nationality ?? (object)DBNull.Value);
                    if (addActorRequest.ProfilePicture == null)
                    {
                        command.Parameters.Add("@ProfilePicture", SqlDbType.VarBinary).Value = DBNull.Value;
                    }
                    else
                    {
                        command.Parameters.Add("@ProfilePicture", SqlDbType.VarBinary).Value = addActorRequest.ProfilePicture;
                    }
                    actorId = (int)await command.ExecuteNonQueryAsync();
                }
                return actorId;
            }
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

