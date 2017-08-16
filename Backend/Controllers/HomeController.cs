using Backend.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Backend.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }

        public string ChatMessage()
        {
            if (Request.QueryString["method"] == "1")
            {
                QnABotAssistant qnaBotAssist = new QnABotAssistant();
                return qnaBotAssist.Answer(Request.QueryString["msg"]);
            }
            else if (Request.QueryString["method"] == "2")
            {
                AccordBotAssistant accdBotAssist = new AccordBotAssistant();
                return accdBotAssist.Answer(Request.QueryString["msg"]);
            }
            else
            {
                return "NowayToGetHereError";
            }
        }
    }
}
