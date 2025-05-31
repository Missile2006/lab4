using Museum.BLL.Models;
using System.Collections.Generic;

namespace Museum.BLL.Interfaces
{
    public interface IExhibitionService
    {
        void AddExhibition(ExhibitionModel exhibitionModel);
        void UpdateExhibition(ExhibitionModel exhibitionModel);
        void DeleteExhibition(int id);
        ExhibitionModel GetExhibition(int id);
        IEnumerable<ExhibitionModel> GetAllExhibitions();
        IEnumerable<ExhibitionModel> GetCurrentExhibitions();
        IEnumerable<ExhibitionModel> SearchExhibitionsByTheme(string theme);
        IEnumerable<ExhibitionModel> SearchExhibitionsByAudience(string audience);
    }
}