using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilioMonitoring.CEPHostInstance.EventTypes
{
  public class CallEventType
  {
    public DateTime Timestamp { get; set; }
    public string Location { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string EventType { get; set; }

    public override string ToString()
    {
      return string.Format("{{ \"From\" : \"{0}\", \"To\" : \"{1}\", \"CallType\": \"{2}\" }}", From, To, EventType);
    }
  }
}
