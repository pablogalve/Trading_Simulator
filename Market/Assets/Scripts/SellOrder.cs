using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SellOrder : MonoBehaviour
{
    public GameObject bidEntryPrefab;
    public float Funds { get; set; }
    public string Username { get; set; }
    public float Amount { get; set; }
    public float Price { get; set; }
    public int timeCreated {get; set;}

    public List<SellOrder> sellOrders;
    public List<string> sellNames = new List<string>();

    ShareHolder shareHolder;
    Variables variables;
    MatchEngine matchEngine;

    public SellOrder(string Username, float Amount, float Funds, float Price, int timeCreated)
    {
        this.Funds = Funds;
        this.Username = Username;
        this.Amount = Amount;
        this.Price = Price;
        this.timeCreated = timeCreated;
    }
    public void Start()
    {
        shareHolder = GameObject.FindObjectOfType(typeof(ShareHolder)) as ShareHolder;
        variables = GameObject.FindObjectOfType(typeof(Variables)) as Variables;
        matchEngine = GameObject.FindObjectOfType(typeof(MatchEngine))as MatchEngine;
        Init();
        //var order0 = new SellOrder(shareHolder.companyName, 400000, 320, 0.0008f, variables.timeContador);
        var order = new SellOrder("Limit", 1, 9999999, 9999999, variables.timeContador);
        sellOrders.Add(order);
        //sellOrders.Add(order0);

        for(int i=0;i<20;i++)
        {
            var orderi = new SellOrder(shareHolder.companyName, 100000*i,100000*i*0.0008f*i ,0.0008f*i,variables.timeContador);
            sellOrders.Add(orderi);
        }

        sellOrders.Sort((p1, p2) => p1.Price.CompareTo(p2.Price));
        DisplaySellList();
    }
    void Init()
    {
        if (sellOrders == null)
        {
            sellOrders = new List<SellOrder>();
        }
        if (sellNames == null)
        {
            sellNames = new List<string>();
        }
    }
    public void SetSeller()
    {
        Sell(shareHolder.playerName,variables.Price,variables.Amount, variables.timeContador);
    }
    public void Sell(string seller, float sellPrice, float sellAmount, int timeCreated)
    {
        Init();
        float sellFunds =0;
        bool tradeable = false;
        if (sellAmount <= 0)
        {
            sellAmount = 0.01f;
        }
        if (sellPrice <= 0)
        {
            sellPrice = 0.00001f;
        }
        else if(sellPrice<=0.001f)
        {
            sellPrice = variables.Round(sellPrice, 5);
        }
        else if(sellPrice<1)
        {
            sellPrice = variables.Round(sellPrice, 4);
        }
        else if(sellPrice>=1)
        {
            sellPrice = variables.Round(sellPrice, 2);
        }
        sellFunds = sellAmount * sellPrice;
        tradeable = CheckEnough(seller, sellAmount);
        if (tradeable == true)
        {

            var order = new SellOrder(seller, sellAmount, sellFunds, sellPrice, timeCreated);

            sellOrders.Add(order);            
                        
            Availability(seller, sellAmount);
            matchEngine.CheckMatch();
        }
        tradeable = false;
        sellOrders.Sort((p1, p2) => p1.Price.CompareTo(p2.Price)); //Order the bidList based in the price in a ascending order
        DisplaySellList();
    }
    public bool CheckEnough(string seller, float sellAmount)
    {
        Init();
        int index = shareHolder.GetIndexOfUser(seller);
        if(shareHolder.ShareholdersList[index].AvailableShares >= sellAmount)
        {
            return true;
        }
        else
        {
            return false;
            print("Error! There are not enough available shares in your account");
        }        
    }
    public void Availability(string seller, float sellAmount)
    {
        int sellerIndex = shareHolder.GetIndexOfUser(seller);
        shareHolder.ShareholdersList[sellerIndex].AvailableShares -= sellAmount;
    }
    public void SetDeleteUser()
    {
        DeleteSellOrder(shareHolder.playerName);
    }
    public void DeleteSellOrder(string deleteUser)
    {
        bool canDelete = false;
        foreach (var name in sellNames)
        {
            if (name.Equals(deleteUser))
            {
                canDelete = true;
            }
        }
        if (canDelete == true)
        {
            int index = sellNames.IndexOf(deleteUser);
            int index2 = shareHolder.GetIndexOfUser(deleteUser);
            shareHolder.ShareholdersList[index2].AvailableShares += sellOrders[index].Amount;
            sellOrders.RemoveAt(index);
            canDelete = false;
        }
        sellOrders.Sort((p1, p2) => p1.Price.CompareTo(p2.Price)); //Order the bidList based in the price in a ascending order
        DisplaySellList();
    }
    public void DisplaySellList()
    {
        sellNames.Clear();
            int i = 0;
            foreach (var sellOrder in sellOrders) //Adds an "invalid" name for every buyOrder
            {
                sellNames.Add("invalid");
                sellNames[i] = sellOrders[i].Username;
                i++;
            }
        while (this.transform.childCount > 0)
        {
            Transform c = this.transform.GetChild(0);
            c.SetParent(null);
            Destroy(c.gameObject);
        }
        int j = 0;
        foreach (string sellName in sellNames) //Show BuyOrders List in the UI
        {
            if (name != "invalid" && j<15) //Only users who are not "invalid" will be shown on screen
            {
                GameObject go = (GameObject)Instantiate(bidEntryPrefab);
                go.transform.SetParent(this.transform);
                go.transform.Find("Username").GetComponent<Text>().text = sellName;
                go.transform.Find("Amount").GetComponent<Text>().text = sellOrders[j].Amount.ToString();
                go.transform.Find("Price").GetComponent<Text>().text = sellOrders[j].Price.ToString();
                go.transform.Find("Funds").GetComponent<Text>().text = sellOrders[j].Funds.ToString();
                j++;
            }
        }
    }
}