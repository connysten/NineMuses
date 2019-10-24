using NineMuses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NineMuses.ViewModels
{
    public class UploadVideoViewModel
    {
        public VideoModel Video { get; set; }
        public HttpPostedFileBase VideoFile { get; set; }
    }
}