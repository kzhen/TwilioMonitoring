using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace TwilioMonitoring.WebEndpoint.RabbitMQRepository
{
  public class RabbitMQRepository : IRabbitMQRepository
  {
    private IConnection connection;
    private IModel channel;
    private string exchangeName;


    public RabbitMQRepository()
    {
      SetupConnection();
      this.exchangeName = "cep";
    }

    private void SetupConnection()
    {
      ConnectionFactory factory = new ConnectionFactory()
      {
        HostName = "disorderlydata.cloudapp.net",
        VirtualHost = "/",
        UserName = "guest",
        Password = "guest",
        Port = 5672
      };

      this.connection = factory.CreateConnection();
      this.channel = connection.CreateModel();
      this.channel.ExchangeDeclare("cep", "direct", true, false, null);
    }

    public void AddMessage(string message)
    {
      byte[] contents = System.Text.Encoding.UTF8.GetBytes(message);
      channel.BasicPublish(exchangeName, "cep", null, contents);
    }
  }
}