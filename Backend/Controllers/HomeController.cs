using Backend.Bot;
using Backend.Public;
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

        /// <summary>
        /// This function checks if the answer should come from QNA maker or Accord bot assistant and invites their answer function
        /// </summary>
        /// <returns></returns>
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
                return "You must choose a method (QnA or Accord Bot)!";
            }
        }

        /// <summary>
        /// This function checks if the accordBotAssistant could import a machine or not and returns a string accordingly
        /// </summary>
        /// <returns></returns>
        public string UpdateViewInfoMessage()
        {
            if (!AccordBotAssistant.Instance.machineLearned)
            {
                return "Please wait, the machine is currently learning, this may take a few minutes";
            }
            else
            {
                return "Please wait, the machine is trying to answer";
            }
        }

        /// <summary>
        /// This function gets the adequate image from the image folder and returns it as a file
        /// </summary>
        /// <param name="imgNumber"></param>
        /// <returns></returns>
        public ActionResult GetImage(int imgNumber)
        {
            FileStream fs = new FileStream(PublicFunctionsVariables.wordDocumentImagesFilePath + imgNumber + ".png",
            FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Byte[] bytes = br.ReadBytes((Int32)fs.Length);
            br.Close();
            fs.Close();
            return File(bytes, "image/png");
        }
    }
}
