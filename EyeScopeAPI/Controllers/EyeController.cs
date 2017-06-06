using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Hosting;
using EyeScopeAPI.Models;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Web;

namespace EyeScopeAPI.Controllers
{

    private int test()
    {
        return 0;
    }


    public class EyeController : ApiController
    {


        string ApiKey = "qJW2KO0gKRSEdcXX";
        private static List<string> dirs = GetDirs();
        // GET api/eye
        [HttpGet]
        [ActionName("welcome")]
        public void Welcome(string key)
        {
            if (IsAuthorized(key))
                throw new HttpResponseException(HttpStatusCode.OK);
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        // GET api/eye/catalog
        [HttpGet]
        [ActionName("catalog")]
        public HttpResponseMessage GetCatalogList(string key)
        {
            if (IsAuthorized(key))
                return new HttpResponseMessage()
                {
                    Content = new StringContent(GetDirJson(), System.Text.Encoding.UTF8, "application/json")
                };
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        // GET api/eye/photo/{id}
        [HttpGet]
        [ActionName("photo")]
        public HttpResponseMessage GeteyesFromCatalog(int id, string key)
        {
            if (IsAuthorized(key))
                return new HttpResponseMessage()
                {
                    Content = new StringContent(GetPhotos(id), System.Text.Encoding.UTF8, "application/json")
                };
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        [HttpPost]
        [ActionName("save")]
        public void Save(HttpRequestMessage request, string key)
        {
            if (IsAuthorized(key))
            {
                var response = String.Empty;
                try
                {
                    var dirId = Regex.Match(request.RequestUri.LocalPath, @"\d+$").Value;
                    if (String.IsNullOrEmpty(dirId))
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    string jsonContent = request.Content.ReadAsStringAsync().Result;
                    var fileLocation = GetPhotoDir(int.Parse(dirId));
                    File.WriteAllText(fileLocation, jsonContent);
                }
                catch
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
                throw new HttpResponseException(HttpStatusCode.OK);
            }
            else
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }


        [NonAction]
        private string GetPhotoDir(int id)
        {
            var fileLocation = Directory.EnumerateDirectories(HostingEnvironment.MapPath("~/Content/Eyes/" + dirs[id])).FirstOrDefault();
            fileLocation = fileLocation.Substring(fileLocation.IndexOf("Content") - 1);
            return HttpContext.Current.Server.MapPath(fileLocation) + "\\result.json";
        }


        [NonAction]
        private string GetPhotos(int id)
        {
            var filesLocation = Directory.EnumerateFiles(HostingEnvironment.MapPath("~/Content/Eyes/" + dirs[id]), "*.*", SearchOption.AllDirectories)
                        .Where(s => s.EndsWith(".json") || s.EndsWith(".jpg"));

            List<string> files = new List<string>();

            foreach (var file in filesLocation)
            {
                files.Add(file.Substring(file.IndexOf("Content") - 1));
            }

            var dic = new Dictionary<string, List<string>>();
            dic.Add("photos", files);
            return JsonConvert.SerializeObject(dic);
        }

        [NonAction]
        private static List<string> GetDirs()
        {
            var dirList = new List<string>();
            foreach (var dir in Directory.EnumerateDirectories(HostingEnvironment.MapPath("~/Content/Eyes")))
            {
                dirList.Add(dir.Substring(dir.LastIndexOf("\\") + 1));
            }
            return dirList;
        }

        [NonAction]
        private static string GetDirJson()
        {
            var dirList = new List<Folder>();
            foreach (var dir in dirs)
            {
                dirList.Add(new Folder(dir, IsJsonExist(dir)));
            }
            var dic = new Dictionary<string, List<Folder>>();
            dic.Add("dirs", dirList);
            return JsonConvert.SerializeObject(dic);
        }

        [NonAction]
        private bool IsAuthorized(string key)
        {
            if (key == ApiKey)
                return true;
            else
                return false;
        }

        [NonAction]
        private static bool IsJsonExist(string dir)
        {
            var filesLocation = Directory.EnumerateFiles(HostingEnvironment.MapPath("~/Content/Eyes/" + dir), "*.*", SearchOption.AllDirectories)
                        .Where(s => s.EndsWith(".json"));
            if (filesLocation.Count() > 0)
                return true;
            else
                return false;
        }

       
    }
}
