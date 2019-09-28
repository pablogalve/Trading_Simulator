using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LoanController : MonoBehaviour {

    public GameObject loanEntryPrefab; 
        
    string loanFundsstr;

    float totalAmountLoaned;
    public float availableToRepay;
    bool canInvest = false;
    bool canAskDebt = false;

    public string Username { get; set; }
    public float Investment { get; set; }
    public float Owed { get; set; }
    public float Return { get; set; }
    public int Day { get; set; }

    public List<string> loanNames = new List<string>();
    public List<LoanController> loanList = new List<LoanController>();

    SellOrder sellOrder;
    ShareHolder shareHolder;
    Variables variables;
    
    public LoanController(string Username, float Investment, float Owed, float Return, int Day)
    {
        this.Username = Username;
        this.Investment = Investment;
        this.Owed = Owed;
        this.Return = Return;
        this.Day = Day;
    }
    
    void Start () {
        Init();
        sellOrder = GameObject.FindObjectOfType(typeof(SellOrder)) as SellOrder;
        shareHolder = GameObject.FindObjectOfType(typeof(ShareHolder)) as ShareHolder;
        variables = GameObject.FindObjectOfType(typeof(Variables)) as Variables;
        var order = new LoanController(shareHolder.companyName, 15, 15, 1.07f, 15);        
        loanList.Add(order);
        InvokeRepeating("DisplayLoans", 1, 1);
        
    }
    void Init()
    {
        if (loanNames == null)
        {
            loanNames = new List<string>();
        }
    }
    public void SetMeBorrower()
    {
        AskDebt(shareHolder.playerName,variables.loanFunds);
    }
    public void AskDebt(string borrower, float loanFunds)
    {
        CheckEnough2(borrower);
        if (canAskDebt)
        {
            Owed = variables.DebtInterest * loanFunds;
            int index = shareHolder.GetIndexOfUser(borrower);
            shareHolder.ShareholdersList[index].Funds += loanFunds;
            shareHolder.ShareholdersList[index].AvailableFunds += loanFunds;
            shareHolder.ShareholdersList[index].DebtNeg += Owed;            
        }
    }
    public void SetMeLoaner()
    {
        GiveLoan(shareHolder.playerName, variables.loanFunds, 1.05f);
    }
    public void GiveLoan(string loaner, float loanFunds, float interestRate) 
    {
        CheckEnough(loaner, loanFunds);
        if (canInvest)
        {
            //We change the interest rate from number (12%) to multiplier (1.12f)
            interestRate /= 100;
            interestRate += 1;

            Owed = interestRate * loanFunds;
            var newLoan = new LoanController(loaner, loanFunds, Owed, interestRate, variables.Day);
            loanList.Add(newLoan);
            int index = shareHolder.GetIndexOfUser(loaner);
            shareHolder.ShareholdersList[index].Funds -= loanFunds;
            shareHolder.ShareholdersList[index].AvailableFunds -= loanFunds;
            shareHolder.ShareholdersList[index].DebtPos += Owed;         
            totalAmountLoaned += loanFunds;            
        }
    }
    void CheckEnough(string loaner, float loanFunds)
    {
        Init();
        int index = shareHolder.GetIndexOfUser(loaner);  //Checks the index of the loaner in names list from ShareHolder script        
        if (shareHolder.ShareholdersList[index].AvailableFunds >= loanFunds)  //Are there available funds for this user?
        {
            canInvest = true;
        }
        else
        {
            canInvest = false;
            print("Error! There are not enough available funds in your account to give a loan");
        }
    }
    void CheckEnough2(string borrower) //This is to check BORROWER'S availability
    {
        Init();
        int index = shareHolder.GetIndexOfUser(borrower);  //Checks the index of the borrower in names list from ShareHolder script        
        if (1==1)  //Are there available funds for this user?
        {
            canAskDebt = true;
        }
        else
        {
            canAskDebt = false;
            print("Error! There are not enough available funds in your account to give a loan");
        }
    }
    void CalculateInterest()
    {
        if(totalAmountLoaned <= 1000)
        {
            variables.LoanInterest = 1.5f;
            variables.DebtInterest = 1.55f;
        }
        else if (totalAmountLoaned <= 2000)
        {
            variables.LoanInterest = 1.3f;
            variables.DebtInterest = 1.34f;
        }
        else if (totalAmountLoaned <= 5000)
        {
            variables.LoanInterest = 1.2f;
            variables.DebtInterest = 1.23f;
        }
        else if (totalAmountLoaned <= 10000)
        {
            variables.LoanInterest = 1.15f;
            variables.DebtInterest = 1.165f;
        }
        else if (totalAmountLoaned <= 30000)
        {
            variables.LoanInterest = 1.05f;
            variables.DebtInterest = 1.062f;
        }
        else
        {
            variables.LoanInterest = 1.02f;
            variables.DebtInterest = 1.049f;
        }
    }
    public void RecaudarDeuda()
    {
        for(int i=0; i < shareHolder.ShareholdersList.Count;i++) //Esto sirve para quitarle el dinero a los deudores
        {
            if (shareHolder.ShareholdersList[i].DebtNeg >= 10000) //Se paga un 5% aprox en cada ronda. Cuanto menos tengas prestado, más % pagas
            {
                if(shareHolder.ShareholdersList[i].AvailableFunds >= 500)
                {
                    shareHolder.ShareholdersList[i].DebtNeg -= 500;
                    shareHolder.ShareholdersList[i].Funds -= 500;
                    shareHolder.ShareholdersList[i].AvailableFunds -= 500;
                    availableToRepay += 500;
                }
                else if(shareHolder.ShareholdersList[i].AvailableShares >= 100)
                {
                    sellOrder.Sell(shareHolder.ShareholdersList[i].Username,0,100,variables.timeContador);
                }
            }
            else if(shareHolder.ShareholdersList[i].DebtNeg >= 2000)
            {
                if (shareHolder.ShareholdersList[i].AvailableFunds >= 105)
                {
                    shareHolder.ShareholdersList[i].DebtNeg -= 105;
                    shareHolder.ShareholdersList[i].Funds -= 105;
                    shareHolder.ShareholdersList[i].AvailableFunds -= 105;
                    availableToRepay += 105;
                }
                else if (shareHolder.ShareholdersList[i].AvailableShares >= 50)
                {
                    sellOrder.Sell(shareHolder.ShareholdersList[i].Username,0,50,variables.timeContador);
                }
            }
            else if (shareHolder.ShareholdersList[i].DebtNeg >= 500)
            {
                if (shareHolder.ShareholdersList[i].AvailableFunds >= 27)
                {
                    shareHolder.ShareholdersList[i].DebtNeg -= 27;
                    shareHolder.ShareholdersList[i].Funds -= 27;
                    shareHolder.ShareholdersList[i].AvailableFunds -= 27;
                    availableToRepay += 27;
                }
                else if (shareHolder.ShareholdersList[i].AvailableShares >= 20)
                {
                    sellOrder.Sell(shareHolder.ShareholdersList[i].Username,0,20,variables.timeContador);
                }
            }
            else if (shareHolder.ShareholdersList[i].DebtNeg > 100)
            {
                if (shareHolder.ShareholdersList[i].AvailableFunds >= 12)
                {
                    shareHolder.ShareholdersList[i].DebtNeg -= 12;
                    shareHolder.ShareholdersList[i].Funds -= 12;
                    shareHolder.ShareholdersList[i].AvailableFunds -= 12;
                    availableToRepay += 12;
                }
                else if (shareHolder.ShareholdersList[i].AvailableShares >= 10)
                {
                    sellOrder.Sell(shareHolder.ShareholdersList[i].Username,0,10,variables.timeContador);
                }
            }
            else if (shareHolder.ShareholdersList[i].DebtNeg > 0)
            {
                if (shareHolder.ShareholdersList[i].AvailableFunds >= 7)
                {
                    shareHolder.ShareholdersList[i].DebtNeg -= 7;
                    shareHolder.ShareholdersList[i].Funds -= 7;
                    shareHolder.ShareholdersList[i].AvailableFunds -= 7;
                    availableToRepay += 7;
                }
                else if (shareHolder.ShareholdersList[i].AvailableShares >= 10)
                {
                    sellOrder.Sell(shareHolder.ShareholdersList[i].Username,0,50,variables.timeContador);
                }
            }
        }        
    }
    public void RepayInterest()
    {
        if (availableToRepay > 0)
        {
            int index = 0;
            float amount = 0;
            float fee = 0.85f; //15% fee
            if (loanList[0].Owed <= availableToRepay)
            {
                amount = loanList[0].Owed;
            }
            else if (loanList[0].Owed > availableToRepay)
            {
                amount = availableToRepay;
            }
            loanList[0].Owed -= amount;
            availableToRepay -= amount;
            string userIndex = loanList[0].Username;
            index = shareHolder.GetIndexOfUser(userIndex);
            shareHolder.ShareholdersList[index].Funds += fee* amount;
            shareHolder.ShareholdersList[index].AvailableFunds += fee* amount;
            shareHolder.ShareholdersList[index].DebtPos -= fee * amount;
            variables.Balance += (1 - fee) * amount;
            if(loanList[0].Owed <= 0)
            {
                loanList.RemoveAt(0);
            }
        }
    }
    void DisplayLoans()
    {
        loanNames.Clear();
        int i = 0;
        foreach (var loan in loanList) 
        {
            loanNames.Add("invalid");
            loanNames[i] = loanList[i].Username;
            i++;
        }  
        while (this.transform.childCount > 0)
        {
            Transform c = this.transform.GetChild(0);
            c.SetParent(null);
            Destroy(c.gameObject);
        }
        int j = 0;
        foreach (string loanName in loanNames) 
        {
            if (loanName != "invalid") 
            {
                GameObject go = (GameObject)Instantiate(loanEntryPrefab);
                go.transform.SetParent(this.transform);
                go.transform.Find("Username").GetComponent<Text>().text = loanName;
                go.transform.Find("Invested").GetComponent<Text>().text = loanList[j].Investment.ToString("0.####");
                go.transform.Find("Owed").GetComponent<Text>().text = loanList[j].Owed.ToString("0.####");
                go.transform.Find("Return%").GetComponent<Text>().text = (100 * loanList[j].Return -100).ToString("0.##") + "%";
                go.transform.Find("Day").GetComponent<Text>().text = loanList[j].Day.ToString();
                j++;
            }
        }
    }
}