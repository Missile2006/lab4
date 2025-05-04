
namespace Museum.DAL.Entities
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int ExhibitionId { get; set; }
        public int StartTimeMinutes => (int)StartTime.TotalMinutes;
        public virtual Exhibition Exhibition { get; set; }
    }
}