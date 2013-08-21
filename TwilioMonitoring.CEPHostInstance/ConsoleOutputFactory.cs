using Microsoft.ComplexEventProcessing.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwilioMonitoring.CEPHostInstance
{
  public enum QueryType
  {
    ByTotal = 1,
    ByCall = 2,
    ByLocationSummary = 3
  }

  public class ConsoleOutputConfig
  {
    public QueryType Query { get; set; } 
  }

  public class ConsoleOutputFactory : ITypedOutputAdapterFactory<ConsoleOutputConfig>
  {
    public OutputAdapterBase Create<TPayload>(ConsoleOutputConfig configInfo, Microsoft.ComplexEventProcessing.EventShape eventShape)
    {
      if (eventShape == Microsoft.ComplexEventProcessing.EventShape.Point)
        return new ConsolePointOutput<TPayload>(configInfo);

      throw new NotImplementedException();
    }

    public void Dispose()
    {
    }
  }
}
