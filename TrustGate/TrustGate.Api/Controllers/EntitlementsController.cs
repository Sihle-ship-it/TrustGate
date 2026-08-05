using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrustGate.Api.Data;
using TrustGate.Api.Models;

namespace TrustGate.Api.Controllers;

[ApiController]
[Route("api/entitlements")]
public class EntitlementsController : ControllerBase
{
    private readonly AppDbContext _db;
    public EntitlementsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<List<Entitlement>> GetAll() => await _db.Entitlements.ToListAsync();

    [HttpPost]
    public async Task<Entitlement> Create(Entitlement entitlement)
    {
        _db.Add(entitlement);
        await _db.SaveChangesAsync();
        return entitlement;
    }
}
