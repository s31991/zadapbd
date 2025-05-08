using System.Threading.Tasks;
using WebApplication1.DTOs;
namespace WebApplication1.Services;

public interface IClientService
{
    
    Task<bool> ClientExistsAsync(int clientId);
        
  
    Task<int> CreateClientAsync(ClientDTO client);
        

    Task RegisterClientForTripAsync(int clientId, int tripId);
        

    Task<bool> UnregisterClientFromTripAsync(int clientId, int tripId);
        
    
    Task<bool> IsClientRegisteredForTripAsync(int clientId, int tripId);
}