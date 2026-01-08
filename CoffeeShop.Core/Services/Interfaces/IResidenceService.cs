using CoffeeShop.Core.DTOs.Residence;

namespace CoffeeShop.Core.Services.Interfaces
{
    public interface IResidenceService
    {
        Task<HomePageViewModel> GetHomePageViewModelsAsync();

        Task<ResidenceDetailViewModel> GetResidenceDetailById(int id);

        Task<AllResidencesViewModel> GetAllResidences(string? search, int page);
    }
}