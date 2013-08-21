using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilioMonitoring.CEPHostInstance
{
  public class CallSummary
  {
    public long TotalCalls { get; set; }
    public string EventType { get; set; }

    public override string ToString()
    {
      return string.Format("{{ \"TotalCalls\": {0}, \"EventType\" : \"{1}\" }}", TotalCalls, EventType);
    }
  }
}
