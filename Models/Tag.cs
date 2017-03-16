using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Tag
    {
        public int TagId { get; set; }

        public string TagName { get; set; }

     
    }
}
