using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrustGate.Api.Data;
using TrustGate.Api.Models;

namespace TrustGate.Api.Controllers
{
    [ApiController]
    [Route("api/requests")]
    public class AccessRequestsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AccessRequestsController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<List<AccessRequest>> GetAll() => await _db.AccessRequests.ToListAsync();

        [HttpPost]
        public async Task<AccessRequest> Create(AccessRequest accessRequest)
        {
            accessRequest.Status = "Pending";
            accessRequest.CreatedAt = DateTime.Now;
            _db.AccessRequests.Add(accessRequest);
            await _db.SaveChangesAsync();
            return accessRequest;
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id, [FromQuery] string adminUsername)
        {
            var admin = _db.Users.FirstOrDefaultAsync(u => u.Username == adminUsername);
            if (admin == null || admin.Role != "Admin")
                return BadRequest("Not an admin");

            var req = await _db.AccessRequests.FindAsync(id);
            if (req == null) return NotFound();

            req.Status = "Rejected";
            req.DecidedBy = adminUsername;
            await _db.SaveChangesAsync();
            return Ok(req);
        }
    }
}
