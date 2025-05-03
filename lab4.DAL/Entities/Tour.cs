namespace Museum.DAL.Entities
{
    public class Tour
    {
        public int TourId { get; set; }
        public DateTime TourDate { get; set; }
        public string GuideName { get; set; }
        public bool IsPrivate { get; set; }
        public decimal Price { get; set; }

        public int ExhibitionId { get; set; }
        public virtual Exhibition Exhibition { get; set; }
    }
}