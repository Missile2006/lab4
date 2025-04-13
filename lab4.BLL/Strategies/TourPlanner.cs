using Museum.BLL.Strategies.Models;
using Museum.BLL.Strategies.Pricing;
using Museum.DAL.Entities;

namespace Museum.BLL.Strategies
{
    public class TourPlanner
    {
        private ITourPricingStrategy _pricingStrategy;

        public TourPlanner(ITourPricingStrategy initialStrategy)
        {
            _pricingStrategy = initialStrategy;
        }

        public void SetPricingStrategy(ITourPricingStrategy strategy)
        {
            _pricingStrategy = strategy;
        }

        public Tour CreateTour(TourCreationModel model)
        {
            return new Tour
            {
                TourDate = model.TourDate,
                GuideName = model.GuideName,
                IsPrivate = _pricingStrategy is PrivateTourPricingStrategy,
                Price = _pricingStrategy.CalculatePrice(model),
                ExhibitionId = model.ExhibitionId
            };
        }

        public string GetTourDescription()
        {
            return _pricingStrategy.GetTourTypeDescription();
        }
    }
}
