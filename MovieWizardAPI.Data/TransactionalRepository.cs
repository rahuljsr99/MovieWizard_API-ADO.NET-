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
                                transaction.Amount = (double)reader["Amount"];
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
    }
}
                      
