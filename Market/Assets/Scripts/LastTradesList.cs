using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LastTradesList : MonoBehaviour
{
    public GameObject tradesEntryPrefab;
    
    float Funds { get; set; }
    string Username { get; set; }
    float Amount { get; set; }
    public float Price { get; set; }

    public List<LastTradesList> lastTradesList;

    public LastTradesList(string Username, float Amount, float Funds, float Price)
    {
        this.Funds = Funds;
        this.Username = Username;
        this.Amount = Amount;
        this.Price = Price;
    }
    public void Start()
    {
        if(lastTradesList == null)
        {
            lastTradesList = new List<LastTradesList>();
            for(int i =0; i<=9;i++)
            {
                var order = new LastTradesList("System", 0, 0, 0);
                lastTradesList.Add(order);
            }
            
        }
        var order1 = new LastTradesList("System", 0, 0, 0);
        lastTradesList.Add(order1);
        InvokeRepeating("DisplayLastTradesList", 1, 1);
    }    
    void CleanList()
    {
        if(lastTradesList.Count > 9)
        {
            int i = 0;
            while(lastTradesList.Count>9)
            {
            lastTradesList.RemoveAt(i); 
            i++;
            }
        }       
    }
    public void DisplayLastTradesList()
    {
        int ListSize;
        ListSize = lastTradesList.Count;
        CleanList();
        while (this.transform.childCount > 0)
        {
            Transform c = this.transform.GetChild(0);
            c.SetParent(null);
            Destroy(c.gameObject);
        }
        for(int j =8;j>=0;j--) //Show Last Trades List in the UI
        {
            if (name != "invalid") //Only users who are not "invalid" will be shown on screen
            {
                GameObject go = (GameObject)Instantiate(tradesEntryPrefab);
                go.transform.SetParent(this.transform);
                go.transform.Find("Username").GetComponent<Text>().text = lastTradesList[j].Username.ToString();
                go.transform.Find("Amount").GetComponent<Text>().text = lastTradesList[j].Amount.ToString();
                go.transform.Find("Price").GetComponent<Text>().text = lastTradesList[j].Price.ToString();
                go.transform.Find("Funds").GetComponent<Text>().text = lastTradesList[j].Funds.ToString();
                
            }
        }
    }
}