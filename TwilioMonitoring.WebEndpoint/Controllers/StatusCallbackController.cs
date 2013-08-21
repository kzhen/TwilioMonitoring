using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Twilio.Mvc;
using Twilio.TwiML;

namespace TwilioMonitoring.WebEndpoint.Controllers
{
  public class StatusCallbackController : ApiController
  {
    private RabbitMQRepository.IRabbitMQRepository repo;

    public StatusCallbackController(RabbitMQRepository.IRabbitMQRepository repo)
    {
      this.repo = repo;
    }

    [HttpPost]
    public void Post(StatusCallbackRequest request)
    {
      if (request != null)
      {
        repo.AddMessage(string.Format("unknown,\"{0}\",\"{1}\",{2}", request.From, request.To, request.CallStatus));
      }
      else
      {
        repo.AddMessage("unknown,unknown,unknown,ending");
      }
    }
  }
}
