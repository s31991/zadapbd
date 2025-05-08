
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.DTOs;
namespace WebApplication1.Services;

public interface ITripService
{
 
    Task<IEnumerable<TripDTO>> GetAllTripsAsync();
        
    
    Task<IEnumerable<ClientTripDTO>> GetClientTripsAsync(int clientId);
        
  
    Task<(bool exists, bool hasSpace)> CheckTripAvailabilityAsync(int tripId);
}