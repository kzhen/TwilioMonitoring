using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilioMonitoring.CEPHostInstance
{
  public class CallLocationSummary
  {
    public string Location { get; set; }
    public long Total { get; set; }

    public override string ToString()
    {
      return string.Format("{{ \"Location\" : {0}, \"Total\" : {1} }}", Location, Total);
    }
  }
}
