
namespace Museum.BLL.Models.Reports
{
    // Звіт про популярність експозицій
    public class PopularExhibitionsReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ExhibitionPopularity> Exhibitions { get; set; }
    }
}
