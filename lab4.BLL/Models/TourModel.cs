namespace Museum.BLL.Models
{
    public class TourModel
    {
        public int TourId { get; set; }
        public int ExhibitionId { get; set; }
        public string GuideName { get; set; }
        public DateTime TourDate { get; set; }
        public bool IsPrivate { get; set; }
        public decimal Price { get; set; } // Ціна екскурсії

        public ExhibitionModel Exhibition { get; set; }
    }
}
