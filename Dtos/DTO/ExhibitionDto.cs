namespace Dtos.DTO
{
    public class ExhibitionDto
    {
        public int ExhibitionId { get; set; }
        public string Title { get; set; }
        public string Theme { get; set; }
        public string TargetAudience { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}