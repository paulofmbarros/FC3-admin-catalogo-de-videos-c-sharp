// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Fc.CodeFlix.Catalog.Application.EventHandlers;

using Domain.Events;
using Domain.SeedWork;
using Interfaces;

public class SendToEncodedEventHandler : IDomainEventHandler<VideoUploadedEvent>
{
    private readonly IMessageProducer messageProducer;

    public SendToEncodedEventHandler(IMessageProducer messageProducer) => this.messageProducer = messageProducer;

    public async Task HandleAsync(VideoUploadedEvent domainEvent, CancellationToken cancellationToken)
    {
        await this.messageProducer.SendMessageAsync(domainEvent, cancellationToken);
    }
}
