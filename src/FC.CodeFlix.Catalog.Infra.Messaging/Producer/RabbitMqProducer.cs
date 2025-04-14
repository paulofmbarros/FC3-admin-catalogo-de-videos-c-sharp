// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Messaging.Producer;

using System.Text.Json;
using Configuration;
using Fc.CodeFlix.Catalog.Application.Interfaces;
using JsonPolicies;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

public class RabbitMqProducer : IMessageProducer
{
    private readonly IModel channel;
    private readonly string exchange;

    public RabbitMqProducer(IModel channel,  IOptions<RabbitMqConfiguration> options)
    {
        this.channel = channel;
        this.exchange = options.Value.Exchange!;
    }

    public Task SendMessageAsync<T>(T message, CancellationToken cancellationToken)
    {
        var routingKey = EventsMapping.GetRoutingKey<T>();
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = new JsonSnakeCasePolicy(),
        };
        var @event = JsonSerializer.SerializeToUtf8Bytes(message, jsonOptions);
        this.channel.BasicPublish(exchange:this.exchange, routingKey: routingKey, body: @event);
        this.channel.WaitForConfirmsOrDie();

        return Task.CompletedTask;
    }
}
