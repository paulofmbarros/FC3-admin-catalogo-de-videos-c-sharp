// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Domain.Exceptions;

using Validation;

public class EntityValidationException : Exception
{
    public IReadOnlyCollection<ValidationError> Errors { get; }

    public EntityValidationException(string? message, IReadOnlyCollection<ValidationError>? errors = null ) : base(message)
    {
        Errors = errors ?? new List<ValidationError>();
    }
    
}