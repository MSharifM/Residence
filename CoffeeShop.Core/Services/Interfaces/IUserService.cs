using CoffeeShop.Core.DTOs.Account;
using CoffeeShop.Core.DTOs.UserPanel;
using CoffeeShop.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CoffeeShop.Core.Services.Interfaces
{
    public interface IUserService
    {
        #region Account

        Task<IdentityResult> RegisterAsync(RegisterViewModel user, string baseUrl);

        Task<SignInResult> SignInAsync(string userName, string password);

        Task LogOutAsync();

        bool IsUserSignIn(ClaimsPrincipal user);

        Task<bool> IsExistEmailAsync(string email);

        Task<bool> IsExistUserNameAsync(string userName);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<bool> IsEmailConfirmedAsync(string? userName = null, string? userId = null, string? email = null);

        Task SendResetPasswordEmailAsync(User user, string baseUrl);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword);

        Task<bool> SendEmailConfirmAgain();

        #endregion Account

        #region User common methods

        Task<User?> GetUserByIdAsync(string userId);

        Task<User?> GetUserByUserNameAsync(string userName);

        Task<User?> GetUserByEmailAsync(string email);

        #endregion User common methods

        #region UserPanel

        Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model);

        Task EditProfileAsync(string userId, UserInformationViewModel model);

        Task<IEnumerable<ReservesViewModel>> GetFutureUserReserves(string userName);

        Task<ReservesViewModel> GetLastUserReserve(string userName);

        Task<bool> AddCommentForResidence(AddCommentViewModel model, int residenceId, string userId);

        #endregion UserPanel

        #region HostPanel

        Task<bool> IsHost(string userName);

        Task<IEnumerable<HostListResidencesViewModel>> GetListResidencesNameForHostAsync(string hostUserName);

        Task<IEnumerable<ListFutureReservesForHostViewModel>> GetFutureReservesForHostByResidenceIdAsync(int residenceId);

        Task<IEnumerable<ListCompletedReservesForHostViewModel>> GetCompletedReservesForHostByResidenceIdAsync(int residenceId);

        Task<IEnumerable<ListResidenceCommentsForHostViewModel>> GetResidenceCommentsForHostByResidenceIdAsync(int residenceId);

        #endregion HostPanel
    }
}