// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.Exceptions;

public abstract class ApplicationException : Exception
{
    public ApplicationException(string? message) : base(message)
    {

    }
}