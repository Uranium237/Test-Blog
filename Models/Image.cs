using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Image
    {
        public int ImageId { get; set; }

        public string ImageName { get; set; }

        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }
    }
}
