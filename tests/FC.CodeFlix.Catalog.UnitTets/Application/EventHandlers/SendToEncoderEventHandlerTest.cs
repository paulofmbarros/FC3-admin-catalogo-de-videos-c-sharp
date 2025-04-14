// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets.Application.EventHandlers;

using Fc.CodeFlix.Catalog.Application.EventHandlers;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using Fc.CodeFlix.Catalog.Domain.Events;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;

public class SendToEncoderEventHandlerTest
{
    [Fact(DisplayName = nameof(HandleAsync))]
    [Trait("Application", "EventHandlers")]
    public async Task HandleAsync()
    {
        var messageProducerMock = new Mock<IMessageProducer>();
        messageProducerMock.Setup(x => x.SendMessageAsync(It.IsAny<VideoUploadedEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var handler = new SendToEncodedEventHandler(messageProducerMock.Object);
        var @event = new VideoUploadedEvent(Guid.NewGuid(), "medias/video.mp4");

        await handler.HandleAsync(@event, CancellationToken.None);
        messageProducerMock.Verify(x => x.SendMessageAsync(It.IsAny<VideoUploadedEvent>(),It.IsAny<CancellationToken>()), Times.Once);
    }
}
