using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WebApplication1.DTOs;
namespace WebApplication1.Services;

public class ClientService : IClientService
    {
        private readonly string _connectionString;

        public ClientService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TravelAgencyDb");
        }

        
        public async Task<bool> ClientExistsAsync(int clientId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using (var command = new SqlCommand(
                    "SELECT COUNT(1) FROM Client WHERE IdClient = @ClientId", connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    int count = (int)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

       
        public async Task<int> CreateClientAsync(ClientDTO client)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using (var command = new SqlCommand(
                    @"INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
                      OUTPUT INSERTED.IdClient
                      VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel)", connection))
                {
                    command.Parameters.AddWithValue("@FirstName", client.FirstName);
                    command.Parameters.AddWithValue("@LastName", client.LastName);
                    command.Parameters.AddWithValue("@Email", client.Email);
                    
                    if (string.IsNullOrEmpty(client.Telephone))
                        command.Parameters.AddWithValue("@Telephone", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Telephone", client.Telephone);
                    
                    if (string.IsNullOrEmpty(client.Pesel))
                        command.Parameters.AddWithValue("@Pesel", DBNull.Value);
                    else
                        command.Parameters.AddWithValue("@Pesel", client.Pesel);
                    
                    return (int)await command.ExecuteScalarAsync();
                }
            }
        }

        
        public async Task RegisterClientForTripAsync(int clientId, int tripId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                
                int registeredAt = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                
                using (var command = new SqlCommand(
                    @"INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
                      VALUES (@ClientId, @TripId, @RegisteredAt, NULL)", connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@TripId", tripId);
                    command.Parameters.AddWithValue("@RegisteredAt", registeredAt);
                    
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        
        public async Task<bool> UnregisterClientFromTripAsync(int clientId, int tripId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using (var command = new SqlCommand(
                    @"DELETE FROM Client_Trip
                      WHERE IdClient = @ClientId AND IdTrip = @TripId", connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@TripId", tripId);
                    
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

      
        public async Task<bool> IsClientRegisteredForTripAsync(int clientId, int tripId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using (var command = new SqlCommand(
                    @"SELECT COUNT(1) FROM Client_Trip
                      WHERE IdClient = @ClientId AND IdTrip = @TripId", connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@TripId", tripId);
                    
                    int count = (int)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }
    }