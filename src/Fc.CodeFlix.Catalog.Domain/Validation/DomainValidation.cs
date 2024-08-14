// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Validation;

using Exceptions;

public class DomainValidation
{
    public static void NotNull(object? target, string fieldName)
    {
        if (target == null)
        {
            throw new EntityValidationException(
                $"{fieldName} should not be null.");
        }
    }

    public static void NotNullOrEmpty(string? target, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(target))
        {
            throw new EntityValidationException(
                $"{fieldName} should not be empty or null.");
        }
    }

    public static void MinLength(string value, int minLength, string fieldName)
    {
        if (value.Length < minLength)
        {
            throw new EntityValidationException(
                $"{fieldName} should have at least {minLength} characters.");
        }
    }

    public static void MaxLength(string value, int maxLength, string fieldName)
    {
        if (value.Length > maxLength)
        {
            throw new EntityValidationException(
                $"{fieldName} should have less than {maxLength} characters.");
        }
    }
}