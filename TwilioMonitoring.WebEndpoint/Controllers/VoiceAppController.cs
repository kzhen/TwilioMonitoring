using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Twilio.Mvc;
using Twilio.TwiML;

namespace TwilioMonitoring.WebEndpoint.Controllers
{
  public class VoiceAppController : ApiController
  {
    private RabbitMQRepository.IRabbitMQRepository repo;

    public VoiceAppController(RabbitMQRepository.IRabbitMQRepository repository)
    {
      this.repo = repository;
    }

    //public HttpResponseMessage Get(VoiceRequest request)
    //{
    //  try
    //  {
    //    if (request != null)
    //    {
    //      repo.AddMessage(string.Format("unknown,\"{0}\",\"{1}\",starting", request.From, request.To));
    //    }
    //    else
    //    {
    //      repo.AddMessage("unknown,unknown,unknown,starting");
    //    }

    //    var response = new TwilioResponse();
        
    //    response.Say("G'day and welcome to the twilio monitoring thing...you should see your call appear on the web now...");

    //    //return response;
    //    return this.Request.CreateResponse(HttpStatusCode.OK, response.Element, new XmlMediaTypeFormatter());
    //  }
    //  catch (Exception ex)
    //  {
    //    var response = new TwilioResponse();
    //    response.Say(ex.Message);
    //    response.Say(ex.StackTrace);
    //    return this.Request.CreateResponse(HttpStatusCode.OK, response.Element, new XmlMediaTypeFormatter());
    //  }
    //}

    public HttpResponseMessage Post(VoiceRequest request)
    {
      try
      {
        if (request != null)
        {
          repo.AddMessage(string.Format("unknown,{0},{1},starting", request.From, request.To));
        }
        else
        {
          repo.AddMessage("unknown,unknown,unknown,starting");
        }

        var response = new TwilioResponse();

        response.Say("G'day and welcome to the twilio monitoring thing...you should see your call appear on the web now...");
        response.Pause(5);
        response.Say("Good bye...");
        //return response;
        return this.Request.CreateResponse(HttpStatusCode.OK, response.Element, new XmlMediaTypeFormatter());
      }
      catch (Exception ex)
      {
        var response = new TwilioResponse();
        response.Say(ex.Message);
        response.Say(ex.StackTrace);
        return this.Request.CreateResponse(HttpStatusCode.OK, response.Element, new XmlMediaTypeFormatter());
      }
    }
  }
}
