using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Core.DTOs.UserPanel
{
    public class UserPanelViewModel
    {
        public UserInformationViewModel InformationViewModel { get; set; }

        public IEnumerable<ReservesViewModel> FutureReserves { get; set; }

        public ReservesViewModel LastReserve { get; set; }

        public IEnumerable<HostListResidencesViewModel>? HostListResidences { get; set; } = new List<HostListResidencesViewModel>();
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

    public class ReservesViewModel
    {
        public string ResidenceName { get; set; }

        public string ResidenceCity { get; set; }

        public decimal Price { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ReserveStatus ReserveStatus { get; set; }
    }

    public enum ReserveStatus
    {
        Unpaid,
        PendingApproval,
        Approved,
        Cancelled,
        InStay,
    }

    #region HostPanel

    public class HostListResidencesViewModel
    {
        public string Name { get; set; }

        public int ResidenceId { get; set; }
    }

    public class ListFutureReservesForHostViewModel
    {
        public string ResidenceName { get; set; }

        public DateTime StartDate { get; set; }

        public string PhoneNumber { get; set; }
    }

    public class ListCompletedReservesForHostViewModel
    {
        public string ResidenceName { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }
    }

    public class ListResidenceCommentsForHostViewModel
    {
        public string UserName { get; set; }

        public string Description { get; set; }

        public int Rate { get; set; }
    }

    #endregion HostPanel
}