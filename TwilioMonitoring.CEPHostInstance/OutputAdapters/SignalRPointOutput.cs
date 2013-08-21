using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilioMonitoring.CEPHostInstance.OutputAdapters
{
  public class SignalRConfig
  {
    public void Configuration(IAppBuilder app)
    {
      var config = new HubConfiguration { EnableCrossDomain = true };
      app.MapHubs(config);
    }
  }

  [HubName("myHubThing")]
  public class MyHubThing : Hub
  {
    public void RawCall(string message)
    {
      Clients.All.byRawCall(message);
    }

    public void A(string message)
    {
      Clients.All.byTotal(message);
    }

    public void B(string message)
    {
      Clients.All.byLocation(message);
    }
  }

  public class SignalRPointOutput<T> : TypedPointOutputAdapter<T>
  {
    private ConsoleOutputConfig configInfo;
    private IHubContext hubContext;

    public SignalRPointOutput(ConsoleOutputConfig configInfo)
    {
      this.configInfo = configInfo;
      this.hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHubThing>();
    }

    public override void Resume()
    {
      ConsumeEvents();
    }

    public override void Start()
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
            if (hubContext != null)
            {
              Console.WriteLine("Broadcast to SignalR");

              switch (configInfo.Query)
              {
                case QueryType.ByTotal:
                  hubContext.Clients.All.byTotal(currentEvent.Payload.ToString());
                  break;
                case QueryType.ByCall:
                  hubContext.Clients.All.byRawCall(currentEvent.Payload.ToString());
                  break;
                case QueryType.ByLocationSummary:
                  hubContext.Clients.All.byLocation(currentEvent.Payload.ToString());
                  break;
                default:
                  break;
              }
            }
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
