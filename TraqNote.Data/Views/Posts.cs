using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraqNote.Data.Views
{
	public class Posts
	{
		public int PostId { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "* A Valid title is required.")]
    [Display(Name = "Title")]
    public string Title { get; set; }

		
		[Required]		
		[Display(Name = "Content")]
		public string Content { get; set; }
		public string TopicName { get; set; }
    public int? Topic_Id { get; set; }
		public DateTime? Created_On { get; set; }
		public DateTime? Modified_On { get; set; }
	}
}
