using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WebApplication1.DTOs;
namespace WebApplication1.Services;

public class TripService : ITripService
    {
        private readonly string _connectionString;

        public TripService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TravelAgencyDb");
        }

     
        public async Task<IEnumerable<TripDTO>> GetAllTripsAsync()
        {
            var trips = new List<TripDTO>();
            var tripCountries = new Dictionary<int, List<string>>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

               
                using (var command = new SqlCommand(
                    @"SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople 
                      FROM Trip t 
                      ORDER BY t.DateFrom", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var trip = new TripDTO
                            {
                                IdTrip = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                DateFrom = reader.GetDateTime(3),
                                DateTo = reader.GetDateTime(4),
                                MaxPeople = reader.GetInt32(5)
                            };

                            trips.Add(trip);
                            tripCountries[trip.IdTrip] = new List<string>();
                        }
                    }
                }

               
                if (trips.Count > 0)
                {
                    using (var command = new SqlCommand(
                        @"SELECT ct.IdTrip, c.Name
                          FROM Country_Trip ct
                          JOIN Country c ON ct.IdCountry = c.IdCountry", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int tripId = reader.GetInt32(0);
                                string countryName = reader.GetString(1);

                                if (tripCountries.ContainsKey(tripId))
                                {
                                    tripCountries[tripId].Add(countryName);
                                }
                            }
                        }
                    }
                }
            }

          
            foreach (var trip in trips)
            {
                if (tripCountries.TryGetValue(trip.IdTrip, out var countries))
                {
                    trip.Countries = countries;
                }
            }

            return trips;
        }

      
        public async Task<IEnumerable<ClientTripDTO>> GetClientTripsAsync(int clientId)
        {
            var clientTrips = new List<ClientTripDTO>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

               
                using (var command = new SqlCommand(
                    "SELECT COUNT(1) FROM Client WHERE IdClient = @ClientId", connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    int clientCount = (int)await command.ExecuteScalarAsync();

                    if (clientCount == 0)
                    {
                        return clientTrips; 
                    }
                }

                
                using (var command = new SqlCommand(
                    @"SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, ct.RegisteredAt, ct.PaymentDate
                      FROM Client_Trip ct
                      JOIN Trip t ON ct.IdTrip = t.IdTrip
                      WHERE ct.IdClient = @ClientId
                      ORDER BY ct.RegisteredAt DESC", connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            clientTrips.Add(new ClientTripDTO
                            {
                                IdTrip = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                DateFrom = reader.GetDateTime(3),
                                DateTo = reader.GetDateTime(4),
                                RegisteredAt = reader.GetInt32(5),
                                PaymentDate = reader.IsDBNull(6) ? null : (int?)reader.GetInt32(6)
                            });
                        }
                    }
                }
            }

            return clientTrips;
        }

      
        public async Task<(bool exists, bool hasSpace)> CheckTripAvailabilityAsync(int tripId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                
                using (var command = new SqlCommand(
                    "SELECT MaxPeople FROM Trip WHERE IdTrip = @TripId", connection))
                {
                    command.Parameters.AddWithValue("@TripId", tripId);
                    var result = await command.ExecuteScalarAsync();

                    if (result == null || result == DBNull.Value)
                    {
                        return (false, false); 
                    }

                    int maxPeople = (int)result;

                   
                    using (var countCommand = new SqlCommand(
                        "SELECT COUNT(1) FROM Client_Trip WHERE IdTrip = @TripId", connection))
                    {
                        countCommand.Parameters.AddWithValue("@TripId", tripId);
                        int currentRegistrations = (int)await countCommand.ExecuteScalarAsync();

                        return (true, currentRegistrations < maxPeople);
                    }
                }
            }
        }
    }