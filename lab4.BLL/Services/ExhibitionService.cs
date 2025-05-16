using AutoMapper;
using Museum.DAL.Entities;
using Museum.DAL.Interfaces;
using Museum.BLL.Models;
using Museum.DAL.UoW;

namespace Museum.BLL.Services
{
    public class ExhibitionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExhibitionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void AddExhibition(ExhibitionModel exhibitionModel)
        {
            if (exhibitionModel == null)
                throw new ArgumentNullException(nameof(exhibitionModel), "Експозиція не може бути null");

            if (string.IsNullOrWhiteSpace(exhibitionModel.Title))
                throw new ArgumentException("Назва не може бути порожньою");

            if (exhibitionModel.StartDate >= exhibitionModel.EndDate)
                throw new ArgumentException("Дата завершення має бути пізніше за дату початку");

            var exhibition = _mapper.Map<Exhibition>(exhibitionModel);
            _unitOfWork.Exhibitions.Add(exhibition);
            _unitOfWork.SaveChanges();
        }

        public void UpdateExhibition(ExhibitionModel exhibitionModel)
        {
            if (exhibitionModel == null)
                throw new ArgumentNullException(nameof(exhibitionModel));

            var existingExhibition = _unitOfWork.Exhibitions.GetById(exhibitionModel.ExhibitionId);
            if (existingExhibition == null)
                throw new KeyNotFoundException("Експозиція не знайдена");

            // Оновлюємо властивості
            existingExhibition.Title = exhibitionModel.Title;
            existingExhibition.Theme = exhibitionModel.Theme;
            existingExhibition.TargetAudience = exhibitionModel.TargetAudience;
            existingExhibition.StartDate = exhibitionModel.StartDate;
            existingExhibition.EndDate = exhibitionModel.EndDate;

            _unitOfWork.Exhibitions.Update(existingExhibition);
            _unitOfWork.SaveChanges();
        }

        public void DeleteExhibition(int id)
        {
            var exhibition = _unitOfWork.Exhibitions.GetById(id);
            if (exhibition == null)
                throw new KeyNotFoundException("Експозицію не знайдено");

            bool hasRelatedData = _unitOfWork.Schedules.GetByExhibitionId(id).Any() ||
                                  _unitOfWork.Visits.GetByExhibitionId(id).Any() ||
                                  _unitOfWork.Tours.GetByExhibitionId(id).Any();

            if (hasRelatedData)
                throw new InvalidOperationException("Неможливо видалити експозицію, оскільки існують пов’язані дані");

            _unitOfWork.Exhibitions.Delete(id);
            _unitOfWork.SaveChanges();
        }

        // Отримати експозицію за id
        public ExhibitionModel GetExhibition(int id)
        {
            var exhibition = _unitOfWork.Exhibitions.GetById(id);
            return _mapper.Map<ExhibitionModel>(exhibition);
        }

        // Отримати всі експозиції
        public IEnumerable<ExhibitionModel> GetAllExhibitions()
        {
            var exhibitions = _unitOfWork.Exhibitions.GetAll();
            return _mapper.Map<IEnumerable<ExhibitionModel>>(exhibitions);
        }

        // Отримати поточні експозиції
        public IEnumerable<ExhibitionModel> GetCurrentExhibitions()
        {
            var exhibitions = _unitOfWork.Exhibitions.GetCurrentExhibitions(DateTime.Now);
            return _mapper.Map<IEnumerable<ExhibitionModel>>(exhibitions);
        }

        // Пошук експозицій за тематикою
        public IEnumerable<ExhibitionModel> SearchExhibitionsByTheme(string theme)
        {
            if (string.IsNullOrWhiteSpace(theme))
                return Enumerable.Empty<ExhibitionModel>();

            var exhibitions = _unitOfWork.Exhibitions.GetByTheme(theme);
            return _mapper.Map<IEnumerable<ExhibitionModel>>(exhibitions);
        }

        // Пошук експозицій за аудиторією
        public IEnumerable<ExhibitionModel> SearchExhibitionsByAudience(string audience)
        {
            if (string.IsNullOrWhiteSpace(audience))
                return Enumerable.Empty<ExhibitionModel>();

            var exhibitions = _unitOfWork.Exhibitions.GetByTargetAudience(audience);
            return _mapper.Map<IEnumerable<ExhibitionModel>>(exhibitions);
        }
    }
}
