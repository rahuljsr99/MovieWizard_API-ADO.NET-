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
                                    (MovieId, DirectorId, TransactionId, ModeOfPayment, CreatedBy, Amount) 
                                    values (@MovieId, @DirectorId, @TransactionId, @ModeOfPayment, @CreatedBy, @Amount)

                                    -- Get the last inserted InvoiceNumber
                                    SELECT SCOPE_IDENTITY()";
                            using (SqlCommand cmd = new SqlCommand(query, connection))
                            {
                                cmd.Transaction = sqlTransaction;
                                cmd.Parameters.AddWithValue("@MovieId", invoice.MovieId);
                                cmd.Parameters.AddWithValue("@DirectorId", invoice.DirectorId);
                                cmd.Parameters.AddWithValue("@TransactionId", invoice.TransactionId);
                                cmd.Parameters.AddWithValue("@ModeOfPayment", invoice.ModeOfPayment);
                                cmd.Parameters.AddWithValue("@CreatedBy", invoice.CreatedBy);
                                cmd.Parameters.AddWithValue("@Amount", invoice.Amount);

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

    }
}


