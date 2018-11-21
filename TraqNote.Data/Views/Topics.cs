using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraqNote.Data.Views
{
	public class Topics
	{
	 public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "* A Valid topic is required.")]
    [Display(Name = "Topic")]
    public string TopicName { get; set; }

    public DateTime? Created_On { get; set; }
	}
}
