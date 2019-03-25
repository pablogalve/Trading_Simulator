using UnityEngine;

public class IAMarketMaker1 : MonoBehaviour {

    SellOrder sellOrder;
    BuyOrder buyOrder;
    ShareHolder shareHolder;
    Variables variables;
	
	void Start () {
        buyOrder = GameObject.FindObjectOfType(typeof(BuyOrder)) as BuyOrder;
        sellOrder= GameObject.FindObjectOfType(typeof(SellOrder)) as SellOrder;
        shareHolder = GameObject.FindObjectOfType(typeof(ShareHolder)) as ShareHolder;
        variables = GameObject.FindObjectOfType(typeof(Variables)) as Variables;
        InvokeRepeating("Buy", 3, 5);
        InvokeRepeating("Sell", 5, 3);
        InvokeRepeating("Limit", 3, 3);
    }	
	
    void Buy()
    {
        buyOrder.Buy(shareHolder.IAMarketMaker1name,buyOrder.buyOrders[0].Price * Random.Range(0.85f, 0.98f),Random.Range(7000.0f, 12000.0f), variables.timeContador);
    
        int index =0;
        index = shareHolder.GetIndexOfUser(shareHolder.IAMarketMaker1name);
        if(shareHolder.ShareholdersList[index].AvailableFunds <= 0.3f * shareHolder.ShareholdersList[index].Funds)
        {
            buyOrder.DeleteBuyOrder(shareHolder.IAMarketMaker1name);
        }
    }
    void Sell()
    {
        sellOrder.Sell(shareHolder.IAMarketMaker1name,sellOrder.sellOrders[0].Price * Random.Range(1.05f, 1.15f), Random.Range(50.0f, 130.0f), variables.timeContador);
        
        int index =0;
        index = shareHolder.GetIndexOfUser(shareHolder.IAMarketMaker1name);
        if(shareHolder.ShareholdersList[index].AvailableShares <= 0.3f * shareHolder.ShareholdersList[index].Shares)
        {
            sellOrder.DeleteSellOrder(shareHolder.IAMarketMaker1name);
        }
    }
    void Limit() //Usaremos está variable para "regalar" lo extra de "Limit"
    {
        //sellOrder.Sell("Limit", 0, 50000, variables.timeContador);
        buyOrder.Buy("Limit", sellOrder.sellOrders[sellOrder.sellOrders.Count-1].Price,1 ,variables.timeContador);
    }
}
