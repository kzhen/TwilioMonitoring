using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TwilioMonitoring.CEPHostInstance
{
  public class TwilioInputAdapter : TypedPointInputAdapter<CallEventType>
  {
    private TwilioConfig config;
    private static Random rnd = new Random();

    public TwilioInputAdapter(TwilioConfig config)
    {
      this.config = config;
    }

    public override void Resume()
    {
      ProduceEvents();
    }

    public override void Start()
    {
      ProduceEvents();
    }

    private void ProduceEvents()
    {
      var currentEvent = default(PointEvent<CallEventType>);

      while (AdapterState != AdapterState.Stopping)
      {
        EnqueueCtiEvent(DateTimeOffset.Now);

        currentEvent = CreateInsertEvent();
        currentEvent.StartTime = DateTime.Now;

        currentEvent.Payload = new CallEventType()
        {
          Location = "sydney",
          From = "1234",
          Timestamp = DateTime.Now
        };

        Enqueue(ref currentEvent);

        Thread.Sleep(rnd.Next(1, 150));
      }
    }
  }
}
