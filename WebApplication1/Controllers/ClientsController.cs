using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ITripService _tripService;

        public ClientsController(IClientService clientService, ITripService tripService)
        {
            _clientService = clientService;
            _tripService = tripService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientDTO clientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int clientId = await _clientService.CreateClientAsync(clientDto);
            return CreatedAtAction(nameof(GetClientTrips), new { id = clientId }, new { IdClient = clientId });
        }


        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetClientTrips(int id)
        {
            bool clientExists = await _clientService.ClientExistsAsync(id);
            if (!clientExists)
            {
                return NotFound($"Client with ID {id} not found");
            }

            var trips = await _tripService.GetClientTripsAsync(id);
            return Ok(trips);
        }


        [HttpPut("{id}/trips/{tripId}")]
        public async Task<IActionResult> RegisterClientForTrip(int id, int tripId)
        {
            
            bool clientExists = await _clientService.ClientExistsAsync(id);
            if (!clientExists)
            {
                return NotFound($"Client with ID {id} not found");
            }

            
            var (tripExists, hasSpace) = await _tripService.CheckTripAvailabilityAsync(tripId);
            if (!tripExists)
            {
                return NotFound($"Trip with ID {tripId} not found");
            }

            if (!hasSpace)
            {
                return BadRequest("Trip has reached its maximum capacity");
            }

            
            bool isRegistered = await _clientService.IsClientRegisteredForTripAsync(id, tripId);
            if (isRegistered)
            {
                return BadRequest("Client is already registered for this trip");
            }

            
            await _clientService.RegisterClientForTripAsync(id, tripId);
            return Ok("Client successfully registered for trip");
        }


        [HttpDelete("{id}/trips/{tripId}")]
        public async Task<IActionResult> UnregisterClientFromTrip(int id, int tripId)
        {
           
            bool clientExists = await _clientService.ClientExistsAsync(id);
            if (!clientExists)
            {
                return NotFound($"Client with ID {id} not found");
            }

            
            bool isRegistered = await _clientService.IsClientRegisteredForTripAsync(id, tripId);
            if (!isRegistered)
            {
                return NotFound("Client is not registered for this trip");
            }

            
            bool success = await _clientService.UnregisterClientFromTripAsync(id, tripId);
            if (success)
            {
                return Ok("Client successfully unregistered from trip");
            }
            else
            {
                return StatusCode(500, "An error occurred while trying to unregister the client");
            }
        }
    }
}