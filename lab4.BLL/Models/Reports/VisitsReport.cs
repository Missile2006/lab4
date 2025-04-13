using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Museum.BLL.Services;

namespace Museum.BLL.Models.Reports
{
    // Модель звіту про візити
    public class VisitsReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalVisits { get; set; }
        public decimal TotalIncome { get; set; }
        public List<ExhibitionVisitStats> ExhibitionStats { get; set; }
    }
}
