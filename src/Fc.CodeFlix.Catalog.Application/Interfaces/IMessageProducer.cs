// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.Interfaces;

public interface IMessageProducer
{
    Task SendMessageAsync<T>(T message, CancellationToken cancellationToken);
}
