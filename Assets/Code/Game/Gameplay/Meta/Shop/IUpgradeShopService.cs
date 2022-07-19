using System.Numerics;

namespace Game.Gameplay.Meta.Shop
{
    public interface IUpgradeShopService
    {
        bool EnoughFundsToUpgrade(string fieldName);
        void BuyUpgrade(string fieldName, string fieldIncrement = "1");
        BigInteger GetUpgradePrice(string fieldName);
        string GetUpgradeValue(string fieldName);
    } 
}