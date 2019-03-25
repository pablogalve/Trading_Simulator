using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShareHolder : MonoBehaviour {

    public GameObject ShareHolderEntryPrefab;

    public string companyName;
    public string playerName;
    public string IAMarketMaker1name;
    public string IAMarketMaker2name;

    public string Username { get; set; }
    public float Shares { get; set; }
    public float AvailableShares { get; set; }
    public float Funds { get; set; }
    public float AvailableFunds { get; set; }
    public float DividendsFiatReceived { get; set; }
    public float DividendsSharesReceived { get; set; }
    public float DebtPos { get; set; }
    public float DebtNeg { get; set; }
    public float percentage { get; set; }

    public List<ShareHolder> ShareholdersList;

    IAMarketMaker1 IAMaker;
    Variables variables;

    public ShareHolder(string Username, float Shares, float Funds, float percentage, float AvailableShares, float AvailableFunds, float DebtPos, float DebtNeg, float DividendsFiatReceived, float DividendsSharesReceived)
    {
        this.Username = Username;
        this.Shares = Shares;
        this.percentage = percentage;
        this.AvailableShares = AvailableShares; 
        this.Funds = Funds;
        this.AvailableFunds = AvailableFunds;
        this.DebtPos = DebtPos;
        this.DebtNeg = DebtNeg;
        this.DividendsFiatReceived = DividendsFiatReceived;
        this.DividendsSharesReceived = DividendsFiatReceived;
    }   
    void Start()
    {
        Init();
        variables = GameObject.FindObjectOfType(typeof(Variables)) as Variables;
        companyName = "Moon Funding";
        playerName = "Pablo Galve";
        IAMarketMaker1name = "Victor";
        IAMarketMaker2name = "Elena";

        var user = new ShareHolder(companyName, 50000000, 12000, 0, 0, 0, 15, 0, 0, 0);
        var user1 = new ShareHolder("John", 0, 2000, 0, 0, 0, 0, 0, 0, 0);
        var user2 = new ShareHolder("Albert", 0, 3000, 0, 0, 0, 0, 0, 0, 0);
        var user3 = new ShareHolder(playerName, 0, 1000, 0, 0, 0, 0, 0, 0, 0);
        var user4 = new ShareHolder(IAMarketMaker1name, 0, 1000, 0, 0, 0, 0, 0, 0, 0);
        var user5 = new ShareHolder(IAMarketMaker2name, 1000, 1000, 0, 0, 0, 0, 0, 0, 0);
        var user6 = new ShareHolder("Limit",1,1000,0,0,0,0,0,0,0);

        ShareholdersList.Add(user);
        ShareholdersList.Add(user1);
        ShareholdersList.Add(user2);
        ShareholdersList.Add(user3);
        ShareholdersList.Add(user4);
        ShareholdersList.Add(user5);
        ShareholdersList.Add(user6);

        int j = 0;
        foreach(var ShareHolder in ShareholdersList)
        {
            ShareholdersList[j].AvailableShares = ShareholdersList[j].Shares;
            ShareholdersList[j].AvailableFunds = ShareholdersList[j].Funds;            
            j++;
        }
        ShareholdersList[1].AvailableFunds -= 5.75f;
        ShareholdersList[2].AvailableFunds -= 1.795f;
        ShareholdersList[0].AvailableShares -= 35000000;
        ShareHolders();
    }
    void Init()
    {
        if(ShareholdersList == null)
        {
            ShareholdersList = new List<ShareHolder>();
        }        
    }

    public int GetIndexOfUser(string user)
    {
        int userIndex=0;
        int i =0;
        ShareHolders();
        while(i<ShareholdersList.Count)
        {
            if(ShareholdersList[i].Username== user)
            {
                userIndex = i;
            }
            i++;
        }
        return userIndex;
    }
    
    public void ShareHolders()
    {
        Init();
        
        while (this.transform.childCount > 0)
        {
            Transform c = this.transform.GetChild(0);
            c.SetParent(null);
            Destroy(c.gameObject);
        }
        
        int j =0;
        while (j < ShareholdersList.Count) //Show BuyOrders List in the UI
        {
            if(ShareholdersList[j].Shares == 0) //Añadimos un balance insignificante a todos los que no tengan acciones para corregir errores
            {
                ShareholdersList[j].Shares += Random.Range(0.000000000000000000001f, 0.0000000000000001f); ;
                ShareholdersList[j].AvailableShares+= Random.Range(0.000000000000000000001f, 0.0000000000000001f); ;
            }
            if (j<500) //Only users who are not "invalid" will be shown on screen
            {
                GameObject go = (GameObject)Instantiate(ShareHolderEntryPrefab);
                go.transform.SetParent(this.transform);
                go.transform.Find("Username").GetComponent<Text>().text = ShareholdersList[j].Username;                
                go.transform.Find("Shares").GetComponent<Text>().text = ShareholdersList[j].Shares.ToString("0.####");                 
                go.transform.Find("AvailableShares").GetComponent<Text>().text = ShareholdersList[j].AvailableShares.ToString("0.####");
                go.transform.Find("Funds").GetComponent<Text>().text = "$" + ShareholdersList[j].Funds.ToString("0.####");
                go.transform.Find("AvailableFunds").GetComponent<Text>().text = "$" + ShareholdersList[j].AvailableFunds.ToString("0.####");
                go.transform.Find("Positive Debt").GetComponent<Text>().text = "$" + ShareholdersList[j].DebtPos.ToString("0.##");
                go.transform.Find("Negative Debt").GetComponent<Text>().text = "$" + ShareholdersList[j].DebtNeg.ToString("0.##");
                go.transform.Find("$DividendsReceived").GetComponent<Text>().text = "$" + ShareholdersList[j].DividendsFiatReceived.ToString("0.######");
                go.transform.Find("DividendsReceived(shares)").GetComponent<Text>().text = ShareholdersList[j].DividendsSharesReceived.ToString("0.######");
                go.transform.Find("Percentage").GetComponent<Text>().text = (100*ShareholdersList[j].Shares/variables.totalShares).ToString("0.#####");
                j++;
            }
        }
        ShareholdersList.Sort((p1, p2) => -p1.Shares.CompareTo(p2.Shares));
    }
}
