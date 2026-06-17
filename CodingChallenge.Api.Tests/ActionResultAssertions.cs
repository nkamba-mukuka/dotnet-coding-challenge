using Microsoft.AspNetCore.Mvc;

namespace CodingChallenge.Api.Tests;

internal static class ActionResultAssertions
{
    public static void AssertBadRequest(IActionResult result)
    {
        switch (result)
        {
            case BadRequestObjectResult:
                return;
            case ObjectResult objectResult:
                Assert.Equal(400, objectResult.StatusCode);
                return;
            default:
                Assert.Fail($"Expected 400 Bad Request but got {result.GetType().Name}");
                break;
        }
    }
}
