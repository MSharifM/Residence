using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Core.DTOs.UserPanel
{
    public class UserPanelViewModel
    {
        public UserInformationViewModel InformationViewModel { get; set; }
    }

    public class UserInformationViewModel
    {
        public string UserName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string Phone { get; set; }

        public ChangePasswordViewModel? ChangePassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public string? UserName { get; set; }

        [Display(Name = " رمزعبور فعلی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = " رمز عبور جدید")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [MinLength(6, ErrorMessage = "رمز عبور نمی تواند کمتر از 6 کاراکتر باشد")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "تکرار رمزعبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "کلمه عبور  با تکرار آن برابر نیست")]
        public string RePassword { get; set; }
    }
}