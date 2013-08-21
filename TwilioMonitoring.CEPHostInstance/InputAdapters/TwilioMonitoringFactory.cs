using Microsoft.ComplexEventProcessing.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwilioMonitoring.CEPHostInstance.Config;

namespace TwilioMonitoring.CEPHostInstance.InputAdapters
{
  public class TwilioMonitoringFactory : ITypedInputAdapterFactory<TwilioConfig>
  {
    public InputAdapterBase Create<TPayload>(TwilioConfig configInfo, Microsoft.ComplexEventProcessing.EventShape eventShape)
    {
      switch (eventShape)
      {
        case Microsoft.ComplexEventProcessing.EventShape.Edge:
          throw new NotImplementedException();
        case Microsoft.ComplexEventProcessing.EventShape.Interval:
          throw new NotImplementedException();
        case Microsoft.ComplexEventProcessing.EventShape.Point:
          return new TwilioInputAdapter(configInfo);
      }

      throw new NotImplementedException();
    }

    public void Dispose()
    {
    }
  }
}
