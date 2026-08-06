using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrustGate.Api.Data;
using TrustGate.Api.Models;

namespace TrustGate.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    public UsersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<List<User>> GetAll() => await _db.Users.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> Get(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();
        return user;
    }

    [HttpPost]
    public async Task<User> Create(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult> Deactivate(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();
        user.IsActive = false;
        await _db.SaveChangesAsync();
        return Ok();
    }

}
