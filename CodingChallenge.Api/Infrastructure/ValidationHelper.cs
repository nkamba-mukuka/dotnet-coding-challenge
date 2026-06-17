using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CodingChallenge.Api.Infrastructure;

public static class ValidationHelper
{
    public static bool IsValid(object model, ModelStateDictionary modelState)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        if (Validator.TryValidateObject(model, context, results, validateAllProperties: true))
            return true;

        foreach (var result in results)
        {
            foreach (var memberName in result.MemberNames.DefaultIfEmpty(string.Empty))
                modelState.AddModelError(memberName, result.ErrorMessage ?? "Validation failed.");
        }

        return false;
    }
}
