using UnityEngine;

public class IAMarketMaker2 : MonoBehaviour
{
    SellOrder sellOrder;
    BuyOrder buyOrder;
    LoanController loanController;
    ShareHolder shareHolder;
    Variables variables;

    void Start()
    {
        variables = GameObject.FindObjectOfType(typeof(Variables)) as Variables;
        buyOrder = GameObject.FindObjectOfType(typeof(BuyOrder)) as BuyOrder;
        sellOrder = GameObject.FindObjectOfType(typeof(SellOrder)) as SellOrder;
        loanController = GameObject.FindObjectOfType(typeof(LoanController)) as LoanController;
        shareHolder = GameObject.FindObjectOfType(typeof(ShareHolder)) as ShareHolder;
        InvokeRepeating("Buy", 3, 2);
        InvokeRepeating("Sell", 3, 2);
        InvokeRepeating("GiveLoan", 10, 10);
    }
    void Buy()
    {
        buyOrder.Buy(shareHolder.IAMarketMaker2name,buyOrder.buyOrders[0].Price * Random.Range(1.05f, 1.15f),Random.Range(7000.0f, 12000.0f), variables.timeContador);
        
        int index =0;
        index = shareHolder.GetIndexOfUser(shareHolder.IAMarketMaker2name);
        if(shareHolder.ShareholdersList[index].AvailableFunds <= 0.5f * shareHolder.ShareholdersList[index].Funds)
        {
            buyOrder.DeleteBuyOrder(shareHolder.IAMarketMaker2name);
        }
    }
    void Sell()
    {
        sellOrder.Sell(shareHolder.IAMarketMaker2name,sellOrder.sellOrders[0].Price * Random.Range(0.85f, 0.98f), Random.Range(50.0f, 130.0f), variables.timeContador);
        
        int index =0;
        index = shareHolder.GetIndexOfUser(shareHolder.IAMarketMaker2name);
        if(shareHolder.ShareholdersList[index].AvailableShares <= 0.5f * shareHolder.ShareholdersList[index].Shares)
        {
            sellOrder.DeleteSellOrder(shareHolder.IAMarketMaker2name);
        }
    }
	void GiveLoan()
	{
        loanController.GiveLoan(shareHolder.IAMarketMaker1name, 500);
        loanController.AskDebt(shareHolder.IAMarketMaker2name, 500);
	}
}