using Microsoft.ComplexEventProcessing.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilioMonitoring.CEPHostInstance
{
  public class RabbitMQMonitoringFactory : ITypedInputAdapterFactory<RabbitMQConfig>
  {
    public InputAdapterBase Create<TPayload>(RabbitMQConfig configInfo, Microsoft.ComplexEventProcessing.EventShape eventShape)
    {
      switch (eventShape)
      {
        case Microsoft.ComplexEventProcessing.EventShape.Edge:
          throw new NotImplementedException();
        case Microsoft.ComplexEventProcessing.EventShape.Interval:
          throw new NotImplementedException();
        case Microsoft.ComplexEventProcessing.EventShape.Point:
          return new RabbitMQInputAdapter(configInfo);
      }

      throw new NotImplementedException();
    }

    public void Dispose()
    {
    }
  }
}
