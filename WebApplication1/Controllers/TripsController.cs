using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

    
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _tripService.GetAllTripsAsync();
            return Ok(trips);
        }
    }
}