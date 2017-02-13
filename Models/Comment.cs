using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string Text { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

       
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public string Author { get; set; }
    }
}
