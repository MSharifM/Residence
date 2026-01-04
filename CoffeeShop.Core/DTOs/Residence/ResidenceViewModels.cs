using System.Security.AccessControl;

namespace CoffeeShop.Core.DTOs.Residence
{
    public class HomePageViewModel
    {
        public IEnumerable<ResidenceBoxDetailViewModel>? PopularResidencesList { get; set; } = new List<ResidenceBoxDetailViewModel>();

        public IEnumerable<ResidenceBoxDetailViewModel>? LuxResidencesList { get; set; } = new List<ResidenceBoxDetailViewModel>();

        public IEnumerable<ResidenceBoxDetailViewModel>? SuggestResidencesList { get; set; } = new List<ResidenceBoxDetailViewModel>();
    }

    public class ResidenceBoxDetailViewModel
    {
        public int ResidenceId { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public float Stars { get; set; }

        public string MainImage { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }
    }

    public class ResidenceDetailViewModel
    {
        public int ResidenceId { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public int Capacity { get; set; }

        public float Stars { get; set; }

        public int CountRate { get; set; }

        public string MainImage { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public int PostalCode { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<ResidenceCommentsViewModel> Comments;

        public IEnumerable<string> ImageNames { get; set; }

        public IEnumerable<ResidenceOptionsViewModel> Options { get; set; }
    }

    public class ResidenceCommentsViewModel
    {
        public string CommentDescription { get; set; }

        public string Name { get; set; }

        public int Rate { get; set; }

        public DateTime Date { get; set; }
    }

    public class ResidenceOptionsViewModel
    {
        public string OptionName { get; set; }

        public string OptionDescription { get; set; }
    }
}