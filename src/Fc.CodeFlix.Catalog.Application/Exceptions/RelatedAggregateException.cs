// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.Exceptions;

public class RelatedAggregateException : ApplicationException
{
    public RelatedAggregateException(string? message) : base(message)
    {
    }

}