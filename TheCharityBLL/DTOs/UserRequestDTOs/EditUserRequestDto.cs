using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityBLL.DTOs.UserRequestDTOs
{
    public class EditUserRequestDto
    {


        
        [StringLength(256, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 256 characters")]
        [Display(Name = "Username")]
        public string? UserName { get; set; }

        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Address")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }
        [Display(Name = "Full Name")]
       
        public string? FullName { get; set; }




    }
}
