
namespace Museum.BLL.Strategies.Models
{
    public class TourCreationModel
    {
        public int ExhibitionId { get; set; }
        public DateTime TourDate { get; set; }
        public string GuideName { get; set; }
        public int ParticipantsCount { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
    }
}
