using CodingChallenge.Api.Models;
using Levelbuild.CodingChallenge.Data.Context;
using Levelbuild.CodingChallenge.Data.Entities;
using Levelbuild.CodingChallenge.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CodingChallenge.Api.Tests;

public class UserControllerTests
{
    private static async Task<Customer> SeedCustomerAsync(AppDbContext context, string name)
    {
        var customer = new Customer { Id = Guid.NewGuid(), Name = name, WebSite = string.Empty };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        return customer;
    }

    private static UserCreateDto ValidUser(string suffix) => new()
    {
        DisplayName = $"user-{suffix}",
        FirstName = "Test",
        LastName = "User",
        Email = $"test-{suffix}@example.com",
        DateOfBirth = new DateTime(1995, 6, 15)
    };

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenRequiredFieldsMissing()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = await SeedCustomerAsync(context, "Validation Co");
        var controller = new UserController(context);

        var result = await controller.Create(customer.Id, new UserCreateDto { DisplayName = "only-name" });

        ActionResultAssertions.AssertBadRequest(result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenEmailInvalid()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = await SeedCustomerAsync(context, "Email Validation Co");
        var controller = new UserController(context);
        var dto = ValidUser("invalid-email");
        dto.Email = "not-an-email";

        var result = await controller.Create(customer.Id, dto);

        ActionResultAssertions.AssertBadRequest(result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenDisplayNameExceedsMaxLength()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = await SeedCustomerAsync(context, "Length Co");
        var controller = new UserController(context);
        var dto = ValidUser("long-display");
        dto.DisplayName = new string('d', 256);

        var result = await controller.Create(customer.Id, dto);

        ActionResultAssertions.AssertBadRequest(result);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenValid()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = await SeedCustomerAsync(context, "User Create Co");
        var controller = new UserController(context);
        var dto = ValidUser("create");

        var result = await controller.Create(customer.Id, dto);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var user = Assert.IsType<UserResponseDto>(created.Value);
        Assert.Equal(customer.Id, user.CustomerId);
        Assert.Equal(dto.DisplayName, user.DisplayName);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenDisplayNameNotUnique()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = await SeedCustomerAsync(context, "Unique Display Co");
        var otherCustomer = await SeedCustomerAsync(context, "Other Co");
        var controller = new UserController(context);
        var dto = ValidUser("dup-display");

        await controller.Create(customer.Id, dto);
        dto.Email = "other@example.com";
        var result = await controller.Create(otherCustomer.Id, dto);

        ActionResultAssertions.AssertBadRequest(result);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenEmailNotUnique()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = await SeedCustomerAsync(context, "Unique Email Co");
        var controller = new UserController(context);
        var dto = ValidUser("dup-email");

        await controller.Create(customer.Id, dto);
        dto.DisplayName = "another-user";
        var result = await controller.Create(customer.Id, dto);

        ActionResultAssertions.AssertBadRequest(result);
    }

    [Fact]
    public async Task List_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        await using var context = TestDbContextFactory.Create();
        var controller = new UserController(context);

        var result = await controller.List(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_AssignsCustomerIdFromRoute()
    {
        await using var context = TestDbContextFactory.Create();
        var customer = await SeedCustomerAsync(context, "Route Co");
        var controller = new UserController(context);

        var result = await controller.Create(customer.Id, ValidUser("route"));
        var created = Assert.IsType<CreatedAtActionResult>(result);
        var user = Assert.IsType<UserResponseDto>(created.Value);

        Assert.Equal(customer.Id, user.CustomerId);
    }
}
