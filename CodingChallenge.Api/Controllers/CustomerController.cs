using CodingChallenge.Api.Infrastructure;
using CodingChallenge.Api.Models;
using Levelbuild.CodingChallenge.Data.Context;
using Levelbuild.CodingChallenge.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace Levelbuild.CodingChallenge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CustomerController : Controller
{
    private readonly AppDbContext _context;

    public CustomerController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [EnableQuery]
    public IActionResult List()
    {
        var query = _context.Customers.Select(c => new CustomerResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            WebSite = c.WebSite
        });

        return Ok(query);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
            return NotFound();

        return Ok(new CustomerResponseDto { Id = customer.Id, Name = customer.Name, WebSite = customer.WebSite });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto)
    {
        if (!ValidationHelper.IsValid(dto, ModelState))
            return BadRequest(ModelState);

        if (await _context.Customers.AnyAsync(c => c.Name == dto.Name))
            return BadRequest("Customer name must be unique.");

        var customer = new Customer { Id = Guid.NewGuid(), Name = dto.Name, WebSite = dto.WebSite ?? string.Empty };
        _context.Customers.Add(customer);

        var saveResult = await DbSaveHelper.TrySaveChangesAsync(_context, () => BadRequest("Customer name must be unique."));
        if (saveResult != null)
            return saveResult;

        var response = new CustomerResponseDto { Id = customer.Id, Name = customer.Name, WebSite = customer.WebSite };
        return CreatedAtAction(nameof(Get), new { id = customer.Id }, response);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CustomerUpdateDto dto)
    {
        if (!ValidationHelper.IsValid(dto, ModelState))
            return BadRequest(ModelState);

        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
            return NotFound();

        if (!string.IsNullOrEmpty(dto.Name) && dto.Name != customer.Name)
        {
            if (await _context.Customers.AnyAsync(c => c.Name == dto.Name))
                return BadRequest("Customer name must be unique.");

            customer.Name = dto.Name;
        }

        if (dto.WebSite != null)
            customer.WebSite = dto.WebSite;

        var saveResult = await DbSaveHelper.TrySaveChangesAsync(_context, () => BadRequest("Customer name must be unique."));
        if (saveResult != null)
            return saveResult;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
            return NotFound();

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
