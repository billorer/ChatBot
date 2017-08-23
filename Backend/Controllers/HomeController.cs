using Backend.Bot;
using System;
using System.IO;
using System.Web.Mvc;

namespace Backend.Controllers
{
    public class HomeController : Controller
    {
        private QnABotAssistant qnaBotAssist;

        public ActionResult Index()
        { 
            ViewBag.Title = "Anonym";
            return View();
        }

        public string ChatMessage()
        {
            if (Request.QueryString["method"] == "1")
            {
                qnaBotAssist = new QnABotAssistant();
                return qnaBotAssist.Answer(Request.QueryString["msg"]);
            }
            else if (Request.QueryString["method"] == "2")
            {           
                return AccordBotAssistant.Instance.Answer(Request.QueryString["msg"]);
            }
            else
            {
                return "NowayToGetHereError";
            }
        }

        public ActionResult GetImage(int imgNumber)
        {
            FileStream fs = new FileStream(@"c:\WordDocImages\img_" + imgNumber + ".png",
            FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Byte[] bytes = br.ReadBytes((Int32)fs.Length);
            br.Close();
            fs.Close();
            return File(bytes, "image/png");
        }
    }
}
