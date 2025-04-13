
namespace Museum.BLL.Models.Reports
{
    // Модель звіту про екскурсії
    public class ToursReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalIncome { get; set; }
        public int PrivateToursCount { get; set; }
        public int GroupToursCount { get; set; }
        public List<GuideTourStats> GuideStats { get; set; }
    }
}
