using CodingChallenge.Api.Infrastructure;
using CodingChallenge.Api.Models;
using Levelbuild.CodingChallenge.Data.Context;
using Levelbuild.CodingChallenge.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace Levelbuild.CodingChallenge.Api.Controllers;

[ApiController]
[Route("api/Customer/{customerId}/[controller]")]
[Produces("application/json")]
public class UserController : Controller
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [EnableQuery]
    public async Task<IActionResult> List(Guid customerId)
    {
        if (!await _context.Customers.AnyAsync(c => c.Id == customerId))
            return NotFound();

        var query = _context.Users
            .Where(u => u.CustomerId == customerId)
            .Select(u => new UserResponseDto
            {
                Id = u.Id,
                CustomerId = u.CustomerId,
                DisplayName = u.DisplayName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                DateOfBirth = u.DateOfBirth
            });

        return Ok(query);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid customerId, Guid id)
    {
        if (!await _context.Customers.AnyAsync(c => c.Id == customerId))
            return NotFound();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.CustomerId == customerId);

        if (user == null)
            return NotFound();

        return Ok(MapToResponse(user));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid customerId, [FromBody] UserCreateDto dto)
    {
        if (!ValidationHelper.IsValid(dto, ModelState))
            return BadRequest(ModelState);

        if (!await _context.Customers.AnyAsync(c => c.Id == customerId))
            return NotFound();

        if (await _context.Users.AnyAsync(u => u.DisplayName == dto.DisplayName))
            return BadRequest("Display name must be unique.");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email must be unique.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            DisplayName = dto.DisplayName,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth
        };

        _context.Users.Add(user);

        var saveResult = await DbSaveHelper.TrySaveChangesAsync(_context, () =>
            BadRequest("Display name or email must be unique."));
        if (saveResult != null)
            return saveResult;

        return CreatedAtAction(nameof(Get), new { customerId, id = user.Id }, MapToResponse(user));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid customerId, Guid id, [FromBody] UserUpdateDto dto)
    {
        if (!ValidationHelper.IsValid(dto, ModelState))
            return BadRequest(ModelState);

        if (!await _context.Customers.AnyAsync(c => c.Id == customerId))
            return NotFound();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.CustomerId == customerId);

        if (user == null)
            return NotFound();

        if (!string.IsNullOrEmpty(dto.DisplayName) && dto.DisplayName != user.DisplayName)
        {
            if (await _context.Users.AnyAsync(u => u.DisplayName == dto.DisplayName))
                return BadRequest("Display name must be unique.");

            user.DisplayName = dto.DisplayName;
        }

        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email must be unique.");

            user.Email = dto.Email;
        }

        if (!string.IsNullOrEmpty(dto.FirstName))
            user.FirstName = dto.FirstName;

        if (!string.IsNullOrEmpty(dto.LastName))
            user.LastName = dto.LastName;

        if (dto.DateOfBirth.HasValue)
            user.DateOfBirth = dto.DateOfBirth.Value;

        var saveResult = await DbSaveHelper.TrySaveChangesAsync(_context, () =>
            BadRequest("Display name or email must be unique."));
        if (saveResult != null)
            return saveResult;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid customerId, Guid id)
    {
        if (!await _context.Customers.AnyAsync(c => c.Id == customerId))
            return NotFound();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.CustomerId == customerId);

        if (user == null)
            return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static UserResponseDto MapToResponse(User user) =>
        new()
        {
            Id = user.Id,
            CustomerId = user.CustomerId,
            DisplayName = user.DisplayName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth
        };
}
