using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrustGate.Api.Data;
using TrustGate.Api.Models;

namespace TrustGate.Api.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        public readonly AppDbContext _db;
        public RolesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<List<Role>> GetAll() => await _db.Roles.ToListAsync();

        [HttpPost]
        public async Task<Role> Create(Role role)
        {
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
            return role;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _db.Roles.FindAsync(id);
            if (role == null) return NotFound();
            _db.Roles.Remove(role);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
