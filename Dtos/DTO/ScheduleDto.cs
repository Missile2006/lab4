namespace Dtos.DTO
{
    public class ScheduleDto
    {
        public int ScheduleId { get; set; }
        public int ExhibitionId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}