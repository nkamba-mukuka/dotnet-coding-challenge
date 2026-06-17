using CodingChallenge.Api.Models;
using Levelbuild.CodingChallenge.Data.Entities;
using Levelbuild.CodingChallenge.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CodingChallenge.Api.Tests;

public class CustomerControllerTests
{
    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameMissing()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new CustomerController(context);

        var result = await controller.Create(new CustomerCreateDto { WebSite = "https://example.com" });

        ActionResultAssertions.AssertBadRequest(result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameExceedsMaxLength()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new CustomerController(context);

        var result = await controller.Create(new CustomerCreateDto
        {
            Name = new string('a', 256)
        });

        ActionResultAssertions.AssertBadRequest(result);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new CustomerController(context);

        var result = await controller.Create(new CustomerCreateDto
        {
            Name = "Acme Corp",
            WebSite = "https://acme.example"
        });

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var customer = Assert.IsType<CustomerResponseDto>(created.Value);
        Assert.Equal("Acme Corp", customer.Name);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameNotUnique()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new CustomerController(context);

        await controller.Create(new CustomerCreateDto { Name = "Duplicate Co" });
        var result = await controller.Create(new CustomerCreateDto { Name = "Duplicate Co" });

        ActionResultAssertions.AssertBadRequest(result);
    }

    [Fact]
    public async Task Delete_CascadesAssociatedUsers()
    {
        await using var context = TestDbContextFactory.Create();
        var customerController = new CustomerController(context);
        var userController = new UserController(context);

        var createCustomer = await customerController.Create(new CustomerCreateDto { Name = "Cascade Co" });
        var customer = (CustomerResponseDto)((CreatedAtActionResult)createCustomer).Value!;

        await userController.Create(customer.Id, new UserCreateDto
        {
            DisplayName = "cascade-user",
            FirstName = "Case",
            LastName = "Cade",
            Email = "case@example.com",
            DateOfBirth = new DateTime(1990, 1, 1)
        });

        await customerController.Delete(customer.Id);

        var listResult = await userController.List(customer.Id);
        Assert.IsType<NotFoundResult>(listResult);
    }
}
