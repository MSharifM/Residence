using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Core.DTOs.UserPanel
{
    public class UserPanelViewModel
    {
        public UserInformationViewModel InformationViewModel { get; set; }

        public IEnumerable<ReservesViewModel> FutureReserves { get; set; }

        public ReservesViewModel LastReserve { get; set; }

        public IEnumerable<HostListResidencesViewModel>? HostListResidences { get; set; } = new List<HostListResidencesViewModel>();

        public AddCommentViewModel Comment { get; set; }
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
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string RePassword { get; set; }
    }

    public class ReservesViewModel
    {
        public int ResidenceId { get; set; }

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

    public class AddCommentViewModel
    {
        [Range(0, 5, ErrorMessage = "امتیاز باید بین 0 تا 5 باشد.")]
        public int Rate { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
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

    public class ListResidenceSalaryViewModel
    {
        public string ResidenceName { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public string FixedDate { get; set; } // It must be converted to like this: 22 - March //

        public decimal Salary { get; set; }
    }

    #endregion HostPanel

    public class StripListViewModel
    {
        public int ResidenceId { get; set; }

        public string ResidenceName { get; set; }

        public string City { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }
    }
}