
namespace Museum.BLL.Models.Reports
{
    // Статистика гідів
    public class GuideTourStats
    {
        public string GuideName { get; set; }
        public int TourCount { get; set; }
        public decimal TotalIncome { get; set; }
    }
}
