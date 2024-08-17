// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.Interfaces;

public interface IUnitOfWork
{
    public Task Commit(CancellationToken cancellationToken);
}