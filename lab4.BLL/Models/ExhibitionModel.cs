namespace Museum.BLL.Models
{
    public class ExhibitionModel
    {
        public int ExhibitionId { get; set; }  // Додаємо ExhibitionId для правильного мапінгу з сутністю
        public string Title { get; set; }
        public string Theme { get; set; }
        public string TargetAudience { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<ScheduleModel> Schedules { get; set; }
        public List<VisitModel> Visits { get; set; }
    }
}
