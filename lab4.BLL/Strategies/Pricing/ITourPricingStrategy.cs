using Museum.BLL.Strategies.Models;

namespace Museum.BLL.Strategies.Pricing
{
    public interface ITourPricingStrategy
    {
        decimal CalculatePrice(TourCreationModel model);
        string GetTourTypeDescription();
    }
}
