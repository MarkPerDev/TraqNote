using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraqNote.Data.Views
{
	public class Registration
	{
		[Required(ErrorMessage = "User Name required")]
		[Display(Name = "User Name")]
		public string Username { get; set; }

		[Required(ErrorMessage = "First Name required")]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Last Name required")]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Email required")]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Email")]
		public string Email { get; set; }
		[Required]
		public Guid ActivationCode { get; set; }

		[Required(ErrorMessage = "Password required")]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Confirm Password required")]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
		[Compare("Password", ErrorMessage = "Error : Confirm password does not match with password")]
		public string ConfirmPassword { get; set; }

		// We may want to change this to just return the hashed value
		public byte[] PasswordByte { get; set; }
	}
}
