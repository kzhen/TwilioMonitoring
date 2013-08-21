using Microsoft.ComplexEventProcessing.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwilioMonitoring.CEPHostInstance.OutputAdapters
{
  public class SignalROutputFactory : ITypedOutputAdapterFactory<ConsoleOutputConfig>
  {
    public OutputAdapterBase Create<TPayload>(ConsoleOutputConfig configInfo, Microsoft.ComplexEventProcessing.EventShape eventShape)
    {
      if (eventShape == Microsoft.ComplexEventProcessing.EventShape.Point)
        return new SignalRPointOutput<TPayload>(configInfo);

      throw new NotImplementedException();
    }

    public void Dispose()
    {
    }
  }
}
