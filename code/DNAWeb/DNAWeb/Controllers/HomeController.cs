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
           if (Request.Params["checkUploadedBeforeUpload"] != null)
            {
                string fileNameT = Request.Params["filename"];
                string fileSizeT = Request.Params["file_size"];
                int chunk_size = int.Parse(Request.Params["chunk_size"]);

                return Json(new { offset = 18402146 }, JsonRequestBehavior.AllowGet);
            }

            int chunk = Request.Params["chunk"] != null ? int.Parse(Request.Params["chunk"]) : 0;
            double total = Request.Params["total"] != null ? Double.Parse(Request.Params["total"]) : 0;
            string fileName = Request.Params["name"] != null ? Request.Params["name"] : "1.jpg";
            double offsetAlready = Request.Params["offsetAlready"] != null ? Double.Parse(Request.Params["offsetAlready"]) : 0;

            userinfo userLogin=Session["userLogin"] as userinfo;
            dnafile fi = (from d in userInfoService.CurrentContext.dnafile
                          where d.FileName.Equals(fileName) && d.UserId == userLogin.Id
                                &&d.FileSize==total 
                          select d).FirstOrDefault();

            if(fi!=null)
            {
                FileStream fs = new FileStream(Server.MapPath(fi.FilePath), FileMode.Append);
                try
                {
                    if (fs.Length == total)
                    {
                        return Json(new { success = true, offset = total }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (offsetAlready < fs.Length)
                        {
                            return Json(new { success = true, offset = fs.Length }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
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
                            long len = fs.Length;
                            fs.Close();

                            return Json(new { success = true, offset = len }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                finally
                {
                    fs.Close();
                }
                
                
            }
            else
            {
                string newFilePath = PathConfig.Upload_Dir + "Base"+Guid.NewGuid().ToString();

                FileStream fs = new FileStream(Server.MapPath(newFilePath), FileMode.Append);

                try
                {
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
                    long len = fs.Length;
                    fs.Close();

                    dnafile df = new dnafile()
                    {
                        FileName = fileName,
                        FilePath = newFilePath,
                        FileSize = total,
                        UserId = userLogin.Id,
                        UploadTime=DateTime.Now
                    };
                    userInfoService.CurrentContext.dnafile.Add(df);
                    userInfoService.CurrentContext.SaveChanges();

                    return Json(new { success = true, offset = len }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    fs.Close();
                }
            }


            return Json(new { success = true}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckFileOffSet(string filename, string chunk_size, string file_size)
        {
            return Json(new { offset = 1048576 }, JsonRequestBehavior.AllowGet);
        }

    }
}
