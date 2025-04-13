using Museum.BLL.Strategies.Models;

namespace Museum.BLL.Strategies.Pricing
{
    public class PrivateTourPricingStrategy : ITourPricingStrategy
    {
        private const decimal BasePricePerHour = 200;

        public decimal CalculatePrice(TourCreationModel model)
        {
            if (model.Duration <= 0)
                throw new ArgumentException("Тривалість має бути більше 0");

            return model.Duration * BasePricePerHour;
        }

        public string GetTourTypeDescription()
        {
            return "Приватна екскурсія";
        }
    }
}
