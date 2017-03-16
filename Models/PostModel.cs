using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class PostModel
    {
        public int PostId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string Text { get; set; }

        public string TagStr { get; set; }

        public string Author { get; set; }

        public string UserId { get; set; }

        public string CategoryName { get; set; }

        public SelectList CategoryNames;

        public List<Image> Images { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
