using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodingChallenge.Api.Infrastructure;

public static class DbSaveHelper
{
    public static async Task<IActionResult?> TrySaveChangesAsync(
        DbContext context,
        Func<IActionResult> onUniqueViolation)
    {
        try
        {
            await context.SaveChangesAsync();
            return null;
        }
        catch (DbUpdateException)
        {
            return onUniqueViolation();
        }
    }
}
