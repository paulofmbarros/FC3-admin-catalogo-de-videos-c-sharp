// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.UnitTets;

using Catalog.Infra.Messaging.Configuration;
using Catalog.Infra.Messaging.Producer;
using Fc.CodeFlix.Catalog.Domain.Events;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

public class RabbitMqProducerTests
{
    [Fact]
    public async Task SendMessageAsync()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "adm_videos",
            Password = "123456"
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.ConfirmSelect();
        var options = Options.Create(new RabbitMqConfiguration
        {
            Exchange = "video.events"
        });
        var producer = new RabbitMqProducer(channel, options );

        var @event = new VideoUploadedEvent(Guid.NewGuid(), "videos/test.mp4");
        await producer.SendMessageAsync(@event, CancellationToken.None);

        Assert.True(channel.IsOpen);
    }
}
