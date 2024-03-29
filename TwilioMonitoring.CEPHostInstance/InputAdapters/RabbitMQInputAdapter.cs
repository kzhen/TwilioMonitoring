﻿using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwilioMonitoring.Common.EventTypes;

namespace TwilioMonitoring.CEPHostInstance.InputAdapters
{
  public class RabbitMQConfig
  {
    public string Host { get; set; }
    public string VirtualHost { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public string Queue { get; set; }
  }

  public class RabbitMQInputAdapter : TypedPointInputAdapter<CallEventType>
  {
    private RabbitMQConfig config;
    private IConnection rabbitMQConnection;
    private IModel consumerChannel;
    private EventingBasicConsumer eventingConsumer;
    private string consumerTag;

    private int connectionAttempts;

    public RabbitMQInputAdapter(RabbitMQConfig config)
    {
      this.config = config;
    }

    public override void Stop()
    {
      if (this.rabbitMQConnection.IsOpen)
      {
        if (this.consumerChannel.IsOpen)
        {
          if (eventingConsumer.IsRunning)
          {
            this.consumerChannel.BasicCancel(consumerTag);
          }
          this.consumerChannel.Close();
        }
        
        this.rabbitMQConnection.ConnectionShutdown -= rabbitMQConnection_ConnectionShutdown;
        this.rabbitMQConnection.Close();
      }
    }

    public override void Resume()
    {
      ConnectRabbitMQ();
    }

    public override void Start()
    {
      ConnectRabbitMQ();
    }

    private void ConnectRabbitMQ()
    {
      connectionAttempts++;

      ConnectionFactory connFactory = new ConnectionFactory()
      {
        HostName = config.Host,
        VirtualHost = config.VirtualHost,
        Port = config.Port,
        UserName = config.UserName,
        Password = config.Password
      };

      try
      {
        rabbitMQConnection = connFactory.CreateConnection();
        consumerChannel = rabbitMQConnection.CreateModel();
        eventingConsumer = new EventingBasicConsumer();
        eventingConsumer.Received += eventingConsumer_Received;
        consumerTag = consumerChannel.BasicConsume(config.Queue, true, eventingConsumer);
        rabbitMQConnection.ConnectionShutdown += rabbitMQConnection_ConnectionShutdown;
      }
      catch (Exception)
      {
        throw;
      }
    }

    private void rabbitMQConnection_ConnectionShutdown(IConnection connection, ShutdownEventArgs reason)
    {
      if (AdapterState == AdapterState.Running)
      {
        //sleep before attempting to reconnect
        Thread.Sleep(1000);

        //remove event handler, so it doesn't get over subscribed
        rabbitMQConnection.ConnectionShutdown -= rabbitMQConnection_ConnectionShutdown;

        //attempt to reconnect
        ConnectRabbitMQ();
      }
    }

    private void eventingConsumer_Received(IBasicConsumer sender, BasicDeliverEventArgs args)
    {
      EnqueueCtiEvent(DateTimeOffset.Now);

      var currentEvent = default(PointEvent<CallEventType>);

      currentEvent = CreateInsertEvent();
      currentEvent.StartTime = DateTime.Now;

      var messageContent = System.Text.Encoding.UTF8.GetString(args.Body);
      string[] contents = messageContent.Split(',');

      if (contents.Length == 4)
      {
        currentEvent.Payload = new CallEventType()
        {
          Location = contents[0],
          From = contents[1],
          To = contents[2],
          Timestamp = DateTime.Now,
          EventType = contents[3]
        };

        Enqueue(ref currentEvent);

        EnqueueCtiEvent(DateTimeOffset.Now);
      }
    }
  }
}
