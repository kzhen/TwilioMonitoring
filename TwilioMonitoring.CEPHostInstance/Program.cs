using System;
using System.Configuration;
using System.Reactive.Subjects;
using System.ServiceModel;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Linq;
using Microsoft.ComplexEventProcessing.ManagementService;
using Microsoft.Owin.Hosting;
using TwilioMonitoring.CEPHostInstance.InputAdapters;
using TwilioMonitoring.CEPHostInstance.EventTypes;
using TwilioMonitoring.CEPHostInstance.OutputAdapters;
using TwilioMonitoring.CEPHostInstance.Config;

namespace TwilioMonitoring.CEPHostInstance
{
  class Program
  {
    static void Main(string[] args)
    {
      string instanceName = "default";
      string applicationName = "twilioAnalytics";

      using (Server cepServer = Server.Create(instanceName))
      {
        if (cepServer.Applications.ContainsKey(applicationName))
        {
          cepServer.Applications[applicationName].Delete();
        }

        ServiceHost host = new ServiceHost(cepServer.CreateManagementService());
        host.AddServiceEndpoint(typeof(IManagementService), new WSHttpBinding(SecurityMode.Message),
                                "http://localhost:8090/default");

        host.Open();

        string url = "http://localhost:9876";

        WebApp.Start<SignalRConfig>(url);

        Application cepApplication = cepServer.CreateApplication(applicationName);

        TwilioConfig config = ReadTwilioConfig();
        RabbitMQConfig rabbitMQConfig = GetRabbitMQConfig();

        var twilioInput = cepApplication.DefineStreamable<CallEventType>(typeof(TwilioMonitoringFactory), config, EventShape.Point, null);

        var rabbitMQInput = cepApplication.DefineStreamable<CallEventType>(typeof(RabbitMQMonitoringFactory), rabbitMQConfig, EventShape.Point, null);

        //var allCalls = from call in twilioInput
        //               select call;

        //var abc = from win in twilioInput.TumblingWindow(TimeSpan.FromSeconds(1))
        //          select new CallSummary
        //          {
        //            TotalCalls = win.Count()
        //          };

        var allCalls2 = from call in rabbitMQInput
                       select call;

        //var abc2 = from win in rabbitMQInput.TumblingWindow(TimeSpan.FromSeconds(10))
        //           select new CallSummary
        //           {
        //             TotalCalls = win.Count()
        //           };

        var abc2 = from call in rabbitMQInput
                   group call by call.EventType into groups
                   from abc in groups.TumblingWindow(TimeSpan.FromSeconds(5))
//                   from abc in groups.HoppingWindow(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1))
                   select new CallSummary
                   {
                     TotalCalls = abc.Count(),
                     EventType = groups.Key
                   };

        //var callsByLocation = from call in rabbitMQInput
        //                      group call by call.Location into groups
        //                      from a in groups.TumblingWindow(TimeSpan.FromSeconds(5))
        //                      select new CallLocationSummary
        //                      {
        //                        Location = groups.Key,
        //                        Total = a.Count()
        //                      };

        var byConsoleSink = cepApplication.DefineStreamableSink<CallEventType>(typeof(ConsoleOutputFactory),
          GetConsoleOutputConfig(QueryType.ByCall), EventShape.Point, StreamEventOrder.ChainOrdered);

        var byConsoleSummarySink = cepApplication.DefineStreamableSink<CallSummary>(typeof(ConsoleOutputFactory),
          GetConsoleOutputConfig(QueryType.ByTotal), EventShape.Point, StreamEventOrder.ChainOrdered);

        var bySignalrSink = cepApplication.DefineStreamableSink<CallSummary>(typeof(SignalROutputFactory),
          GetConsoleOutputConfig(QueryType.ByTotal), EventShape.Point, StreamEventOrder.ChainOrdered);

        var bySignalrLocationSummarySink = cepApplication.DefineStreamableSink<CallLocationSummary>(typeof(SignalROutputFactory),
          GetConsoleOutputConfig(QueryType.ByLocationSummary), EventShape.Point, StreamEventOrder.ChainOrdered);

        var bySignalrRawCallSink = cepApplication.DefineStreamableSink<CallEventType>(typeof(SignalROutputFactory),
          GetConsoleOutputConfig(QueryType.ByCall), EventShape.Point, StreamEventOrder.ChainOrdered);

        //using (allCalls.Bind(byConsoleSink)
        //  .With(abc.Bind(byConsoleSummarySink))
        //  .With(abc2.Bind(bySignalrSink))
        //  .Run())
        //using (abc2.Bind(bySignalrSink)
        //  .Run())
        using (abc2.Bind(bySignalrSink)
          //.With(callsByLocation.Bind(bySignalrLocationSummarySink))
          .With(allCalls2.Bind(bySignalrRawCallSink))
          .Run())
        {
          Console.WriteLine("----------------------------------------------------------------");
          Console.WriteLine("Client is running, press Enter to exit the client");
          Console.WriteLine("----------------------------------------------------------------");
          Console.WriteLine(" ");
          Console.ReadLine();
        }

        host.Close();
      }
    }

    private static RabbitMQConfig GetRabbitMQConfig()
    {
      return new RabbitMQConfig()
      {
        Host = "disorderlydata.cloudapp.net",
        VirtualHost = "/",
        UserName = "guest",
        Password = "guest",
        Port = 5672,
        Queue = "cep"
      };
    }

    private static TwilioConfig ReadTwilioConfig()
    {
      return new TwilioConfig();
    }

    private static ConsoleOutputConfig GetConsoleOutputConfig(QueryType type)
    {
      return new ConsoleOutputConfig()
      {
        Query = type
      };
    }
  }
}
