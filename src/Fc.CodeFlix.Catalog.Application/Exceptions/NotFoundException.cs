﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string? message) : base(message)
    {
    }

    public static void ThrowIfNull(object? obj, string message)
    {
        if (obj == null)
        {
            throw new NotFoundException(message);
        }
    }
}