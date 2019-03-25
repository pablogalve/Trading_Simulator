using UnityEngine;
using System.Collections;

public class MatchEngine : MonoBehaviour {
    
    LastTradesList lastTradesList;
    ShareHolder shareHolder;
    BuyOrder buyOrder;
    SellOrder sellOrder;
    Variables variables;

    void Awake ()
    {
        variables = GameObject.FindObjectOfType(typeof(Variables)) as Variables;
        shareHolder = GameObject.FindObjectOfType(typeof(ShareHolder)) as ShareHolder;        
        lastTradesList = GameObject.FindObjectOfType(typeof(LastTradesList)) as LastTradesList;
        buyOrder = GameObject.FindObjectOfType(typeof(BuyOrder)) as BuyOrder;
        sellOrder = GameObject.FindObjectOfType(typeof(SellOrder)) as SellOrder;
    }	

	void Start()
    {
        InvokeRepeating("Wait", 0.5f, 0.5f);
    }
    void Wait()
    {
        if (buyOrder.buyOrders != null && sellOrder.sellOrders != null)
        {
            CheckMatch();
        }
    }
    public void CheckMatch()
    {
        string buyUser;
        string sellUser;
        bool matchFound = false;
        bool makerIsBuyer = GetMaker(buyOrder.buyOrders[0].timeCreated,sellOrder.sellOrders[0].timeCreated); 
        float EF=0; //Exchanged Funds
        float EP=0; //EP-Exchanged agreed Price
        float EA =0; //EA-Exchanged Amount
        if (buyOrder.buyOrders[0].Price >= sellOrder.sellOrders[0].Price) //Match Found! Biggest Bid is equal or Bigger than Smallest Ask
        {
            buyUser = buyOrder.buyNames[0];
            sellUser = sellOrder.sellNames[0];

            float spentFunds=0; //This are the funds that the market taker initially spent
            float returnAvailableFunds=0; //return available funds to market takers because they have matched a better operation, resulting cheaper

            matchFound = true;        
            
            if (makerIsBuyer)  //The market maker is the buyer and the taker is the seller
            {
                EP = buyOrder.buyOrders[0].Price;
                print("Match Found and buyer is maker at a price of: " + EP);
                spentFunds = sellOrder.sellOrders[0].Funds;
                returnAvailableFunds = buyOrder.buyOrders[0].Price * sellOrder.sellOrders[0].Amount;

                int sellerIndex = shareHolder.GetIndexOfUser(sellUser);

                //ShareHolder.ShareholdersList[sellerIndex].AvailableFunds += spentFunds - returnAvailableFunds;

                Exchange(EP,EA,EF,matchFound,buyUser,sellUser,makerIsBuyer);
            }
            else if (!makerIsBuyer) //Market Maker: Seller, Market Taker: Buyer
            {
                EP = sellOrder.sellOrders[0].Price;
                print("Match Found and seller is maker at a price of: " + EP);
                spentFunds = buyOrder.buyOrders[0].Funds;
                returnAvailableFunds = sellOrder.sellOrders[0].Price * buyOrder.buyOrders[0].Amount;

                int buyerIndex = shareHolder.GetIndexOfUser(buyUser);

                shareHolder.ShareholdersList[buyerIndex].AvailableFunds += spentFunds -returnAvailableFunds; //Gives a rebate to the buyer to compensate him for being a taker
                
                Exchange(EP,EA,EF,matchFound,buyUser,sellUser,makerIsBuyer);
            }
            
        }
        else         //Match not found! Biggest Bid is smaller than Smallest Ask
        {
            matchFound = false;
        }
    }
    public void Exchange(float EP, float EA, float EF, bool matchFound, string buyUser, string sellUser, bool whoIsMaker)
    {
        
        if (matchFound == true)
        {     
            if (buyOrder.buyOrders[0].Amount >= sellOrder.sellOrders[0].Amount)
            {
                EA = sellOrder.sellOrders[0].Amount;
                EF = sellOrder.sellOrders[0].Amount * EP;
            }
            else
            {
                EA = buyOrder.buyOrders[0].Amount;
                EF = buyOrder.buyOrders[0].Amount * EP;
            }
            shareHolder.ShareHolders();
            //Now that Price, Amount and Funds variables for this transaction have been calculated, let's make the transaction!
            int buyerIndex = shareHolder.GetIndexOfUser(buyUser);
            int sellerIndex = shareHolder.GetIndexOfUser(sellUser);

            shareHolder.ShareholdersList[buyerIndex].Shares += EA * variables.Fee;               //Updates the buyer assets in ShareHolderList
            shareHolder.ShareholdersList[buyerIndex].AvailableShares += EA * variables.Fee;
            shareHolder.ShareholdersList[buyerIndex].Funds -= EF;

            shareHolder.ShareholdersList[sellerIndex].Shares -= EA;             //Updates the seller assets in ShareHolderList
            shareHolder.ShareholdersList[sellerIndex].Funds += EF * variables.Fee;
            shareHolder.ShareholdersList[sellerIndex].AvailableFunds += EF * variables.Fee;

            variables.Balance += EF * (1 - variables.Fee);
            variables.DividendsShare += EA * (1 - variables.Fee);

            buyOrder.buyOrders[0].Amount -= EA;
            buyOrder.buyOrders[0].Funds -= EF;

            sellOrder.sellOrders[0].Amount -= EA;
            sellOrder.sellOrders[0].Funds -= EF;
            
            var order = new LastTradesList(buyOrder.buyOrders[0].Username, EA, EF, EP);
            lastTradesList.lastTradesList.Add(order);           
            
            variables.lastPrice = lastTradesList.lastTradesList[0].Price;
            variables.totalValuation = variables.totalShares * variables.lastPrice;

            matchFound = false;

            DeleteNullOrder();
            buyOrder.DisplayBuyList();
            sellOrder.DisplaySellList();            
            shareHolder.ShareHolders();  //Actualizar la lista de accionistas
            CheckMatch(); //Checks for another match
        }        
    }
    public void DeleteNullOrder()
    {
        if(buyOrder.buyOrders[0].Amount <= 0)
        {
            buyOrder.buyOrders.RemoveAt(0);
            buyOrder.buyNames.RemoveAt(0);
        }
        if(sellOrder.sellOrders[0].Amount <= 0)
        {
            sellOrder.sellOrders.RemoveAt(0);
            sellOrder.sellNames.RemoveAt(0);
        }
    }
    bool GetMaker(int buyerTime, int sellerTime)  //Returns who is the maker, based on the time where the order was created
    {
        if(buyerTime>sellerTime) //Seller order was created first
        {
            return false; //Buyer is NOT the maker
        }
        else if(buyerTime<sellerTime)
        {
            return true; //Buyer is the maker
        }
        else
        {
            return true;
        }
    }
}