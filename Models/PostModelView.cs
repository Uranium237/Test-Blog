using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class PostModelView
    {
        public List<PostModel> PostModels { get; set; }

        public SelectList CategoryNames;

        public string CategoryName { get; set; }
    }
}
