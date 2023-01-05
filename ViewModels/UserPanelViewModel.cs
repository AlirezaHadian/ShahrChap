using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class EnableEmailViewModel
    {
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [EmailAddress(ErrorMessage = "پست الکترونیک وارد شده معتبر نمی باشد")]
        public string Email { get; set; }
    }
    public class ChangeEmailViewModel
    {
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [EmailAddress(ErrorMessage = "پست الکترونیک وارد شده معتبر نمی باشد")]
        public string Email { get; set; }
    }
    public class EnablePhoneViewModel
    {
        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [RegularExpression(@"09([0-9][0-9]|3[1-9]|2[1-9])-?[0-9]{3}-?[0-9]{4}$", ErrorMessage = "{0} معتبر نمی باشد!")]
        public string Phone { get; set; }
    }
    public class ChangePhoneViewModel
    {
        [Display(Name = "شماره موبایل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [RegularExpression(@"09([0-9][0-9]|3[1-9]|2[1-9])-?[0-9]{3}-?[0-9]{4}$", ErrorMessage = "{0} معتبر نمی باشد!")]
        public string Phone { get; set; }
    }
    public class AddAddressViewModel
    {
        [Display(Name ="نام و نام خانوادگی")][Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string FullName { get; set; }
        [Display(Name = "آدرس کامل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.MultilineText)]
        public string FullAddress { get; set; }
        [Display(Name = "استان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Province { get; set; }
        [Display(Name = "شهرستان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int City { get; set; }
        [Display(Name = "کد پستی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.PostalCode)]
        [RegularExpression(@"\b(?!(\d)\1{3})[13-9]{4}[1346-9][013-9]{5}\b", ErrorMessage = "کد پستی وارد شده نامعتبر می باشد.")]
        public string PostCode { get; set; }
    }
    public class ChangePasswordViewModel
    {
        [Display(Name = "کلمه عبور فعلی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Display(Name = "کلمه عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "تکرار کلمه عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "کلمه های عبور هم خوانی ندارند")]
        public string ConfirmPassword { get; set; }
    }
}
