// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Openvia">
//     Copyright (c) Openvia. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FC.CodeFlix.Catalog.Infra.Messaging.Configuration;

using RabbitMQ.Client;

public class ChannelManager
{
    private readonly IConnection connection;
    private readonly object connectionLock = new object();
    private IModel? channel = null;

    public ChannelManager(IConnection connection)
    {
        this.connection = connection;
    }

    public IModel GetChannel()
    {
        lock (this.connectionLock)
        {
            if (this.channel == null || this.channel.IsClosed)
            {
                this.channel = this.connection.CreateModel();
            }
            return this.channel;
        }
    }
}
