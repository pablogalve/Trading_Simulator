using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BuyOrder : MonoBehaviour
{
    public GameObject bidEntryPrefab;
    public float Funds { get; set; }
    public string Username { get; set; }
    public float Amount { get; set; }
    public float Price { get; set; }
    public int timeCreated{get;set;}
    
    public List<BuyOrder> buyOrders;
    public List<string> buyNames = new List<string>();
    
    ShareHolder shareHolder;
    Variables variables;
    MatchEngine matchEngine;

    public BuyOrder(string Username, float Amount, float Funds, float Price, int timeCreated)
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
        var order3 = new BuyOrder("Alberto", 20, 0.008f, 0.0004f,variables.timeContador);
        var order4 = new BuyOrder("Alberto", 200, 0.04f, 0.0002f,variables.timeContador);
        var order5 = new BuyOrder("Pepe", 5000, 0.5f, 0.0001f,variables.timeContador);
        var order6 = new BuyOrder("Alberto", 20, 0.006f, 0.0003f,variables.timeContador);
        var order7 = new BuyOrder("Alberto", 5, 0.0035f, 0.0007f,variables.timeContador);
        var order8 = new BuyOrder("Pepe", 37, 0.01295f, 0.00035f,variables.timeContador);
        var order9 = new BuyOrder("Limit", 10000000, 1000, 0.0001f,variables.timeContador);
        buyOrders.Add(order3);
        buyOrders.Add(order4);
        buyOrders.Add(order5);
        buyOrders.Add(order6);
        buyOrders.Add(order7);
        buyOrders.Add(order8);
        buyOrders.Add(order9);
        buyOrders.Sort((p1, p2) => -p1.Price.CompareTo(p2.Price));
        DisplayBuyList();    
    }
    public void Init()
    {
        if (buyOrders == null)
        {
            buyOrders = new List<BuyOrder>();      
        }
        if (buyNames == null)
        {
            buyNames = new List<string>();
            buyNames.Add("invalid");
            buyNames.Add("invalid");
            buyNames.Add("invalid");
        }
    }
    public void SetBuyer()
    {
        Buy(shareHolder.playerName,variables.Price,variables.Amount, variables.timeContador);
    }
    public void Buy(string buyer, float buyPrice, float buyAmount, int timeCreated)
    {
        float buyFunds = 0;
        bool tradeable = false;
        Init();
        if (buyPrice <= 0)
        {
            buyPrice = 0.00001f;
        }
        else if(buyPrice <= 0.001)
        {
            buyPrice = variables.Round(buyPrice, 5);
        }
        else if(buyPrice < 1)
        {
            buyPrice = variables.Round(buyPrice, 4);
        }
        else if(buyPrice >= 1)
        {
            buyPrice = variables.Round(buyPrice, 2);
        }
        if (buyAmount <= 0)
        {
            buyAmount = 0.01f;
        }
        buyFunds = buyAmount * buyPrice;
        tradeable = CheckEnough(buyer, buyFunds);
        
        if (tradeable == true)
        {     
            var order = new BuyOrder(buyer, buyAmount, buyFunds, buyPrice, timeCreated);

            buyOrders.Add(order);            
                        
            Availability(buyer, buyFunds);
            matchEngine.CheckMatch(); 
        }
        tradeable = false;
        buyOrders.Sort((p1, p2) => -p1.Price.CompareTo(p2.Price)); //Order the bidList based in the price in a descending order
        DisplayBuyList();
    }
    public bool CheckEnough(string buyer, float buyFunds)
    {
        Init();             
        if(shareHolder.ShareholdersList[shareHolder.GetIndexOfUser(buyer)].AvailableFunds >= buyFunds)  //Are there available funds for this user?
        {
            return true;
        }
        else
        {
            return false;
            print("Error! There are not enough available funds in your account");
        }
    }
    public void Availability(string buyer, float buyFunds)
    {
        int buyerIndex = shareHolder.GetIndexOfUser(buyer);
        shareHolder.ShareholdersList[buyerIndex].AvailableFunds -= buyFunds;
    }
    public void SetDeleteUser()
    {
        DeleteBuyOrder(shareHolder.playerName);
    }
    public void DeleteBuyOrder(string deleteUser)
    {
        foreach(var name in buyNames)
        {
            if (name.Equals(deleteUser))
            {
                int index = buyNames.IndexOf(deleteUser);
                int index2 = shareHolder.GetIndexOfUser(deleteUser);
                shareHolder.ShareholdersList[index2].AvailableFunds += buyOrders[index].Funds;
                buyOrders.RemoveAt(index);
            }
        }
        buyOrders.Sort((p1, p2) => -p1.Price.CompareTo(p2.Price)); //Order the bidList based in the price in a descending order
        DisplayBuyList();
    }
    public void DisplayBuyList()
    {        
        buyNames.Clear();
        int i = 0;
        foreach (var buyOrder in buyOrders) //Adds an "invalid" name for every buyOrder
        {
            buyNames.Add("invalid");
            buyNames[i] = buyOrders[i].Username;
            i++;
        }
        while (this.transform.childCount > 0)
        {
            Transform c = this.transform.GetChild(0);
            c.SetParent(null);
            Destroy(c.gameObject);
        }
        int j = 0;
        foreach (string buyName in buyNames) //Show BuyOrders List in the UI
        {
            if (name != "invalid" && j < 15) //Only users who are not "invalid" will be shown on screen
            {
                GameObject go = (GameObject)Instantiate(bidEntryPrefab);
                go.transform.SetParent(this.transform);
                go.transform.Find("Username").GetComponent<Text>().text = buyName;
                go.transform.Find("Amount").GetComponent<Text>().text = buyOrders[j].Amount.ToString();
                go.transform.Find("Price").GetComponent<Text>().text = buyOrders[j].Price.ToString();
                go.transform.Find("Funds").GetComponent<Text>().text = buyOrders[j].Funds.ToString();
                j++;
            }
        }
    }
}   