using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUILibrary.Models
{
    public class CreateUserUIModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string? EmailAddress { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[.!@#$%^&*])(?=.*[^a-zA-Z0-9]).{6,100}$",
            ErrorMessage = "The {0} must conteins digit, non alphanumeric, lowercase, uppercase characters")]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "The password do not match")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }

        //(?=.*[0-9]) - строка содержит хотя бы одно число;
        //(?=.*[!@#$%^&*]) - строка содержит хотя бы один спецсимвол;
        //(?=.*[a-z]) - строка содержит хотя бы одну латинскую букву в нижнем регистре;
        //(?=.*[A-Z]) - строка содержит хотя бы одну латинскую букву в верхнем регистре;
    }
}
