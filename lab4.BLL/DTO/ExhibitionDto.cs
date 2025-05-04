using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Museum.BLL.DTO
{
    public class ExhibitionDto
    {
        public int ExhibitionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TargetAudience { get; set; }
        public string Theme { get; set; }
    }

}
