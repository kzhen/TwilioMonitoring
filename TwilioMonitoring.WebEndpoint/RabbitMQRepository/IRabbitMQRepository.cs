using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilioMonitoring.WebEndpoint.RabbitMQRepository
{
  public interface IRabbitMQRepository
  {
    void AddMessage(string message);
  }
}
