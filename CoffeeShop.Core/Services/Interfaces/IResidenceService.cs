using CoffeeShop.Core.DTOs.Residence;
using Microsoft.AspNetCore.Http;

namespace CoffeeShop.Core.Services.Interfaces
{
    public interface IResidenceService
    {
        Task<HomePageViewModel> GetHomePageViewModelsAsync();

        Task<ResidenceDetailViewModel> GetResidenceDetailById(int id);

        Task<AllResidencesViewModel> GetAllResidences(string? search, int page);

        Task<ResidenceDetailForHostPanelViewModel> GetResidenceDetailForHost(int residenceId);

        Task UpdateResidenceDetail(ResidenceDetailForHostPanelViewModel model, int residenceId);

        Task<ReserveResidenceViewModel> GetDetailForReserve(int residenceId, string userName,
            DateTime startDate, DateTime endDate);

        Task<bool> ReserveSubmitAsync(ReserveResidenceViewModel model, int residenceId, string userId);

        Task<EditResidenceImagesViewModel> GetResidenceImagesForEditAsync(int residenceId);

        Task EditImageResidence(List<IFormFile>? newResidenceImages, int residenceId, List<int>? removedResidences);

        Task<List<string>> GetAllOptions();

        Task<int> AddResidence(AddResidenceViewModel model);
    }
}