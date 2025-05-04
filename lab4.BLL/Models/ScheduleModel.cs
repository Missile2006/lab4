namespace Museum.BLL.Models
{
    public class ScheduleModel
    {
        public int ScheduleId { get; set; }
        public int ExhibitionId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
