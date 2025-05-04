using Museum.DAL.Entities;

namespace Museum.BLL.Models.Reports
{
    // Статистика візитів для окремої експозиції
    public class ExhibitionVisitStats
    {
        public ExhibitionModel Exhibition { get; set; }
        public int VisitCount { get; set; }
        public decimal TotalIncome { get; set; }
    }
}
