using Museum.DAL.Entities;

namespace Museum.BLL.Models.Reports
{
    // Оцінка популярності експозиції
    public class ExhibitionPopularity
    {
        public ExhibitionModel Exhibition { get; set; }
        public int VisitsCount { get; set; }
        public int ToursCount { get; set; }
        public int TotalScore { get; set; }
    }
}
