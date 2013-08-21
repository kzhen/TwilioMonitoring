using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TwilioMonitoring.CEPHostInstance
{
  public class ConsolePointOutput<T> : TypedPointOutputAdapter<T>
  {
    private ConsoleOutputConfig configInfo;

    public ConsolePointOutput(ConsoleOutputConfig configInfo)
    {
      this.configInfo = configInfo;
    }

    public override void Start()
    {
      ConsumeEvents();
    }

    public override void Resume()
    {
      ConsumeEvents();
    }

    private void ConsumeEvents()
    {
      PointEvent<T> currentEvent;
      DequeueOperationResult result;

      while (AdapterState != AdapterState.Stopping)
      {
        result = Dequeue(out currentEvent);

        if (result == DequeueOperationResult.Empty)
        {
          PrepareToResume();
          Ready();
          return;
        }
        else
        {
          if (currentEvent.EventKind == EventKind.Insert)
          {
            Console.WriteLine(currentEvent.Payload.ToString());
          }

          ReleaseEvent(ref currentEvent);
        }

      }
    }

    private void PrepareToResume()
    {
    }
  }
}
