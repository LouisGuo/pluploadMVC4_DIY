using DNAWeb.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DNAWeb.Controllers
{
    public class HomeController : Controller
    {

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UploadC()
        {
            return View();
        }

        public ActionResult test()
        {

            return View();
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        public ActionResult Upload()
        {
            //for (int i = 0; i < Request.Files.Count; i++)
            //{
            //    var file = Request.Files[i];
            //    if (file != null)
            //    {
            //        file.SaveAs(Server.MapPath("/Uploads/" + file.FileName));
            //    }
            //}


            int chunk = Request.Params["chunk"] != null ? int.Parse(Request.Params["chunk"]) : 0;


            string fileName = Request.Params["name"] != null ? Request.Params["name"] : "1.jpg";
            //open a file, if our chunk is 1 or more, we should be appending to an existing file, otherwise create a new file
            
            FileStream fs = new FileStream(Server.MapPath(PathConfig.Upload_Dir + fileName), chunk == 0 ? FileMode.OpenOrCreate : FileMode.Append);

            //write our input stream to a buffer
            Byte[] buffer = null;
            if (Request.ContentType == "application/octet-stream" && Request.ContentLength > 0)
            {
                buffer = new Byte[Request.InputStream.Length];
                Request.InputStream.Read(buffer, 0, buffer.Length);
            }
            else if (Request.ContentType.Contains("multipart/form-data") && Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
            {
                buffer = new Byte[Request.Files[0].InputStream.Length];
                Request.Files[0].InputStream.Read(buffer, 0, buffer.Length);
            }


            //write the buffer to a file.
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();


            return Json(new { success = true}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckFileOffSet(string filename, string chunk_size, string file_size)
        {
            return Json(new { offset = 1048576 }, JsonRequestBehavior.AllowGet);
        }

    }
}
