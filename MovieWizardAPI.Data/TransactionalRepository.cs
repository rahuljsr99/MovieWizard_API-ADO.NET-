using MovieWizardAPI.Data.Interfaces;
using MovieWizardAPI.Models;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Data;
using System.Transactions;
using System.Text.Json.Nodes;
using System.Reflection.PortableExecutable;

namespace MovieWizardAPI.Data
{
    public class TransactionalRepository : ITransactionalRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString; // Changed from string? to string (required field)

        // Constructor that receives IConfiguration and connection string.
        public TransactionalRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("MovieWizardConnection"); // Assumes connection string name
        }

        // Synchronous method to get the latest invoice number
        public async Task<int> GetLatestInvoiceNumber()
        {
            int latestInvoiceNumber = 0;


            string query = "SELECT TOP 1 InvoiceNumber FROM Invoices ORDER BY InvoiceNumber DESC";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {

                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {

                    object result = await cmd.ExecuteScalarAsync();

                    if (result != null && int.TryParse(result.ToString(), out int invoiceNumber))
                    {
                        latestInvoiceNumber = invoiceNumber;
                    }
                }
            }

            return latestInvoiceNumber;
        }

        public async Task<Transactions> GetTransactionAsync(int invoiceNumber)
        {
            Transactions transaction = new();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();
                string query = @"SELECT * from Transactions
                                WHERE InvoiceNumber = @InvoiceNumber";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (reader.Read())
                        {
                            try
                            {
                                transaction.TransactionId = (string)reader["TransactionId"];
                                transaction.Amount = (int)reader["Amount"];
                                transaction.InvoiceNumber = (int)reader["InvoiceNumber"];
                                transaction.CreatedBy = (string)reader["CreatedBy"];
                                transaction.CreatedOn = (DateTime)reader["CreatedOn"];
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message);
                            }
                        }
                    }
                }

                return transaction;
            }
        }

        public async Task<int> SaveInvoiceToDb(Invoice invoice)
        {
            if (invoice == null)
            {
                return 0;
            }
            else
            {
                int invoiceNumber = 0;
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlTransaction sqlTransaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string query = @" INSERT INTO dbo.Invoices 
                                    (MovieId, DirectorId, TransactionNumber, ModeOfPayment, CreatedBy, Amount, ItemType) 
                                    values (@MovieId, @DirectorId, @TransactionNumber, @ModeOfPayment, @CreatedBy, @Amount, @ItemType)

                                    -- Get the last inserted InvoiceNumber
                                    SELECT SCOPE_IDENTITY()";
                            using (SqlCommand cmd = new SqlCommand(query, connection))
                            {
                                cmd.Transaction = sqlTransaction;
                                cmd.Parameters.AddWithValue("@MovieId", invoice.MovieId);
                                cmd.Parameters.AddWithValue("@DirectorId", invoice.DirectorId);
                                cmd.Parameters.AddWithValue("@TransactionNumber", invoice.TransactionNumber);
                                cmd.Parameters.AddWithValue("@ModeOfPayment", invoice.ModeOfPayment);
                                cmd.Parameters.AddWithValue("@CreatedBy", invoice.CreatedBy);
                                cmd.Parameters.AddWithValue("@Amount", invoice.Amount);
                                cmd.Parameters.AddWithValue("@ItemType", invoice.ItemType);

                                // Execute the command and retrieve the InvoiceNumber (last inserted identity)
                                invoiceNumber = Convert.ToInt32(await cmd.ExecuteScalarAsync());



                            }
                            sqlTransaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            await sqlTransaction.RollbackAsync();
                            throw ex;
                        }

                    }

                }
                return invoiceNumber;
            }
        }

        public async Task<Invoice?> GetInvoiceByInvoiceNumber(int invoiceNumber)
        {
            Invoice invoice = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "EXEC GetInvoiceProc @InvoiceNumber"; // Call the stored procedure

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            // Assuming only one record is returned for the given InvoiceNumber
                            while (await reader.ReadAsync())
                            {
                                try
                                {
                                    invoice = new Invoice
                                    {
                                        InvoiceNumber = invoiceNumber,
                                        MovieId = (int)reader["MovieId"],
                                        DirectorId = (int)reader["DirectorId"],
                                        MovieName = (string)reader["MovieName"], 
                                        DirectorName = (string)reader["DirectorName"], 
                                        ModeOfPayment = (string)reader["ModeOfPayment"],
                                        TransactionNumber = (string)reader["TransactionNumber"],
                                        CreatedOn = ConvertDateTimeToFormattedString((DateTime)reader["CreatedOn"]),
                                        CreatedBy = (string)reader["CreatedBy"],
                                        Amount = Convert.ToDouble(reader["Amount"])
                                    };

                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Error mapping data: " + ex.Message);
                                }
                            }
                        }
                    }
                }
            }

            return invoice; // Returns the invoice object
        }

        public async Task<TotalRevenue> GetTotalRevenue()
        {
            TotalRevenue totalRevenue = new TotalRevenue() ;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = "Exec TotalRevenue";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            // Assuming only one record is returned for the given InvoiceNumber
                            while (await reader.ReadAsync())
                            {
                                try
                                {
                                    totalRevenue = new TotalRevenue
                                    {
                                        TotalRevenueAmount = (decimal)reader["TotalRevenue"],
                                        TotalRevenueFromMovies = (decimal)reader["TotalRevenueFromMovies"],
                                        TotalRevenueFromTVShows = (decimal)reader["TotalRevenueFromTVShows"]
                                    };

                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Error mapping data: " + ex.Message);
                                }
                            }
                        }
                    }

                }
            }
                return totalRevenue;
        }

        private string ConvertDateTimeToFormattedString(DateTime date)
        {
            // Get the day of the month with the ordinal suffix
            string dayWithOrdinal = GetDayWithOrdinal(date.Day);

            // Format the date as "6th February, 2025 02:11 PM"
            return $"{dayWithOrdinal} {date.ToString("MMMM, yyyy")} {date.ToString("hh:mm tt")}";
        }

        private string GetDayWithOrdinal(int day)
        {
            if (day % 10 == 1 && day != 11)
                return $"{day}st";
            if (day % 10 == 2 && day != 12)
                return $"{day}nd";
            if (day % 10 == 3 && day != 13)
                return $"{day}rd";
            return $"{day}th";
        }

        

    }
}


