namespace CoffeeShop.Core.DTOs.Residence
{
    public class HomePageViewModel
    {
        public IEnumerable<ResidenceBoxDetailViewModel>? PopularResidencesList { get; set; } = new List<ResidenceBoxDetailViewModel>();
    }

    public class ResidenceBoxDetailViewModel
    {
        public string Name { get; set; }

        public string City { get; set; }

        public float Stars { get; set; }

        public string MainImage { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }
    }
}