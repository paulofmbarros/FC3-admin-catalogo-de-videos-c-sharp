// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Exceptions;

public class EntityValidationException : Exception
{
    public EntityValidationException(string? message) : base(message)
    {
    }
    
}