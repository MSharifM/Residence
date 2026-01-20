using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Core.DTOs.Residence
{
    public class HomePageViewModel
    {
        public IEnumerable<ResidenceBoxDetailViewModel>? PopularResidencesList { get; set; } = new List<ResidenceBoxDetailViewModel>();

        public IEnumerable<ResidenceBoxDetailViewModel>? LuxResidencesList { get; set; } = new List<ResidenceBoxDetailViewModel>();

        public IEnumerable<ResidenceBoxDetailViewModel>? SuggestResidencesList { get; set; } = new List<ResidenceBoxDetailViewModel>();

        public IEnumerable<ResidenceBoxDetailViewModel>? EspecialResidencesList { get; set; } = new List<ResidenceBoxDetailViewModel>();
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

    public class AllResidencesViewModel
    {
        public IEnumerable<ResidenceBoxDetailViewModel> ResidenceBoxDetail { get; set; }

        public int CountPage { get; set; }
    }

    public class ReserveDraftViewModel
    {
        public ReserveDraftViewModel()
        {
        }

        public ReserveDraftViewModel(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
            CountNights = (EndDate - StartDate).Days;
        }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ResidenceId { get; set; }

        public int CountNights { get; set; }
    }

    public class ReserveResidenceViewModel
    {
        public ReserveResidenceViewModel()
        {
            Clients = new List<ClientListViewModel>();
            NewClients = new List<ClientListViewModel>();
            RemovedClientIndices = new List<int>();
        }

        public ReserveResidenceViewModel(string residenceName, decimal pricePerDay,
               ReserveDraftViewModel reserveDraft, List<ClientListViewModel> clients)
        {
            this.ResidenceName = residenceName;
            this.PricePerDay = pricePerDay;
            this.ReserveDraft = reserveDraft;
            this.Clients = clients;
            this.Price = PricePerDay * this.ReserveDraft.CountNights;
        }

        public string ResidenceName { get; set; }

        public decimal PricePerDay { get; set; }

        public decimal Price { get; set; }

        public ReserveDraftViewModel ReserveDraft { get; set; }

        public List<ClientListViewModel> Clients { get; set; }

        public List<ClientListViewModel> NewClients { get; set; } = new List<ClientListViewModel>();

        public List<int> RemovedClientIndices { get; set; } = new List<int>();
    }

    public class ClientListViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [MaxLength(30, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [MaxLength(30, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public bool IsMan { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [DataType(DataType.PostalCode)]
        public string Pin { get; set; }
    }

    public class ResidenceDetailForHostPanelViewModel
    {
        public string Name { get; set; }

        public string PostalCode { get; set; }

        public string Street { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public bool Status { get; set; }

        public int Capacity { get; set; }
    }
}