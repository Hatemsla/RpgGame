using TMPro;
using UnityEngine;

namespace World.Trader.UI
{
    public class TraderShopView : MonoBehaviour
    {
        public Trader currentTrader;
        public TraderPage traderShopPageForBuy;
        public TraderPage traderShopPageForSell;

        public TMP_Text traderCoins;
        public TMP_Text playerCoins;

        public void OpenBuyPage()
        {
            traderShopPageForBuy.gameObject.SetActive(true);
            traderShopPageForSell.gameObject.SetActive(false);
        }
        
        public void OpenSellPage()
        {
            traderShopPageForBuy.gameObject.SetActive(false);
            traderShopPageForSell.gameObject.SetActive(true);
        }
    }
}