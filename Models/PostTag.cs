using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class PostTag
    {
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }


        public int TagId { get; set; }
        [ForeignKey("TagId")]
        public Tag Tag { get; set; }

       
    }

   
}

