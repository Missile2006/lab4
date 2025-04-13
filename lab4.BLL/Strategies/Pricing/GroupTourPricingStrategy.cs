using Museum.BLL.Strategies.Models;

namespace Museum.BLL.Strategies.Pricing
{
    public class GroupTourPricingStrategy : ITourPricingStrategy
    {
        private const decimal BasePricePerPerson = 30;

        public decimal CalculatePrice(TourCreationModel model)
        {
            if (model.ParticipantsCount <= 0)
                throw new ArgumentException("Кількість учасників має бути більше 0");

            return model.ParticipantsCount * BasePricePerPerson;
        }

        public string GetTourTypeDescription()
        {
            return "Групова екскурсія";
        }
    }
}
