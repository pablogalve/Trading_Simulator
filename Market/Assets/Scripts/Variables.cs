using UnityEngine;
using UnityEngine.UI;
using System;

public class Variables : MonoBehaviour {

    public float Amount;  //Estas son las variables que usaremos SOLO para el usuario, la cantidad, precio y dinero de los otros scripts podrán
    public float Price;   //ser accedidos tanto por el usuario como por inteligencias artifiales
    public float Funds;

    public float Balance;  //All the profit goes here and then is distributed between the other balances
    public float SafetyNet; //Used to pay debts in the queue  30% -----
    public float DevFunds;  //Used to expand the business     45%
    public float BuyBack;  //Used to buy-back shares from the market at a taker rate 5%
    float DividendsFiat = 0;   //Dividends in form of fiat waiting to be distributed  20%
    public float DividendsShare = 0;  //Dividends in form of shares waiting to be distributed

    public float Fee = 0.99f;  //Tengo que crear una función que cargue comisión

    public float loanFunds;
    public float Owed;
    public float toPay;
    public float totalDebtNeg;
    public float totalDebtPos;
    public float LoanInterest = 1.1f;
    public float DebtInterest = 1.1f; 

    int Hour;
    public int Day;
    public int timeContador;

    public float biggestBid;
    public float smallestAsk;
    public float lastPrice;
    public float totalShares;
    public float totalValuation;

    public int CompanyIndex;

    public InputField amountInputField;
    public InputField priceInputField;
    public InputField loanFundsInputField;

    public Text Cost;
    public Text lastPricetext;
    public Text totalSharestext;
    public Text totalValuationtext;

    public Text buyBackText;
    public Text safetyNetText;
    public Text devFundsText;

    public Text dividendsFiattext;
    public Text dividendsSharetext;
    public Text dividendsPerSharetext;

    public Text hourtext;
    public Text daytext;

    public Text loanInterestText;
    public Text debtInterestText;
    public Text owedText;
    public Text toPayText;
    public Text availableToRepay;

    LoanController loanController;
    BuyOrder buyOrder;
    SellOrder sellOrder;
    ShareHolder shareHolder;
    LastTradesList lastTradesList;
    MatchEngine matchEngine;

    void Start () {
        SetPriceText();
        SetAmountText();
        SetLoanFundsText();
        Hour = 0;
        Day = 1;
        buyOrder = GameObject.FindObjectOfType(typeof(BuyOrder)) as BuyOrder;  //Funciona!!!!! Sirve para poder llamar funciones de otros scripts
        sellOrder = GameObject.FindObjectOfType(typeof(SellOrder)) as SellOrder;
        loanController = GameObject.FindObjectOfType(typeof(LoanController)) as LoanController;
        shareHolder = GameObject.FindObjectOfType(typeof(ShareHolder)) as ShareHolder;        
        lastTradesList = GameObject.FindObjectOfType(typeof(LastTradesList)) as LastTradesList;
        matchEngine = GameObject.FindObjectOfType(typeof(MatchEngine))as MatchEngine;
        InvokeRepeating("DistributeBalance", 1, 1);	
	    InvokeRepeating("GetData", 5, 5);
        InvokeRepeating("SetMarketData", 1, 1);
        InvokeRepeating("GetTime", 1, 1);
        InvokeRepeating("TimeContador", 0.1f, 0.1f);
    }

    void TimeContador()
    {        
        timeContador++;
    }
    void GetTime()
    {
        Hour ++;        
        if (Hour >= 3)
        {
            Day++;
            if (Day % 5 == 0)
            {
                DistributeDividends();
            }
            if (Day % 3 == 0)
            {
                PayDebt();
            }
            if (Day % 4 == 0)
            {
                PayBuyBack();
            }
            if (Day % 3 == 0)
            {
                loanController.RepayInterest();
            }
            if (Day % 1 == 0)
            {
                loanController.RecaudarDeuda();
            }
            Hour = 0;
        }
    }
     void GetData()
    {        
        //biggestBid = BuyOrder.buyOrders[0].Price;
        //smallestAsk = SellOrder.sellOrders[0].Price;
        //lastPrice = lastTradesList.lastTradesList[0].Price; Esto se ejecuta en MatchEngine con cada trade
        totalShares = 0;
        for (int i = 0; i < shareHolder.ShareholdersList.Count; i++)
        {
            totalShares += shareHolder.ShareholdersList[i].Shares;
        }
        totalShares += DividendsShare;
        //totalValuation = totalShares * lastPrice; Esto se ejecuta en MatchEngine con cada trade
        Owed = loanFunds * LoanInterest;
        toPay = loanFunds * DebtInterest;
        DistributeBalance();
    }
    public void SetPriceText()
    {
        string pricestr = priceInputField.text;
        if (!float.TryParse(pricestr, out Price));
        Funds = Amount * Price;
        SetCost();
    }
    public void SetAmountText()
    {
        string amountstr = amountInputField.text;
        if (!float.TryParse(amountstr, out Amount));
        Funds = Amount * Price;
        SetCost();
    }
    public void SetLoanFundsText()
    {
        string loanFundsstr = loanFundsInputField.text;
        if (!float.TryParse(loanFundsstr, out loanFunds));
    }
    void SetCost()
    {
        Cost.text = (Price*Amount).ToString() + "€";
    }
    void SetMarketData()
    {
        lastPricetext.text = "Last price: " + lastPrice.ToString("0.######") + " €";
        totalValuationtext.text = "Company Valuation: " + totalValuation.ToString("#") + " €";
        totalSharestext.text = "Total Shares: " + totalShares.ToString("#");
        dividendsFiattext.text = "Next dividends: " + DividendsFiat.ToString("0.####") + " €";
        dividendsSharetext.text = "Next dividends: " + DividendsShare.ToString("0.####") + " shares";
        dividendsPerSharetext.text = "For holding 1000 shares, you will receive: " + (1000*DividendsFiat/totalShares).ToString("0.######") +"€ + " + (1000*DividendsShare/totalShares).ToString("0.######") + " shares";
        hourtext.text = "Hour: " + Hour.ToString("0") + "   (1 day = 24hours)";
        daytext.text = "Today is day #" + Day.ToString() + ".                                    Dividends are shared every 5 days";
        loanInterestText.text = "Loan Interest Rate: " + (100*LoanInterest-100).ToString() + "%";
        debtInterestText.text = "Debt Interest Rate: " + (100*DebtInterest-100).ToString() + "%";
        owedText.text = "You will receive: €" + Owed.ToString("0.##");
        toPayText.text = "You will have a debt of: €" + toPay.ToString("0.##");
        buyBackText.text = "Buy Back: €" + BuyBack.ToString("0.##");
        safetyNetText.text = "Safety Net Fund Pool: €" + SafetyNet.ToString("0.##");
        devFundsText.text = "Dev Funds: €" + DevFunds.ToString("0.##");
        availableToRepay.text = "Available for Loaners: €" + loanController.availableToRepay;
    }   
    void DistributeDividends() //Distribute dividends to all shareholders
    {
        CompanyIndex = shareHolder.GetIndexOfUser(shareHolder.companyName);
        for (int i = 0; i < shareHolder.ShareholdersList.Count; i++)
        {
            float FiatAmount = (DividendsFiat * shareHolder.ShareholdersList[i].Shares) / totalShares;
            float SharesAmount = (DividendsShare * shareHolder.ShareholdersList[i].Shares) / totalShares;
            shareHolder.ShareholdersList[i].Funds += FiatAmount;
            shareHolder.ShareholdersList[i].AvailableFunds += FiatAmount;
            shareHolder.ShareholdersList[i].DividendsFiatReceived += FiatAmount;

            shareHolder.ShareholdersList[i].Shares += SharesAmount;
            shareHolder.ShareholdersList[i].AvailableShares += SharesAmount;
            shareHolder.ShareholdersList[i].DividendsSharesReceived += SharesAmount;
            if(i == CompanyIndex)
            {
                Balance += FiatAmount;
            }
        }
        DividendsFiat = 0;
        DividendsShare = 0;
        PayBuyBack();
    }
    void DistributeBalance()
    {
        if (Balance > 0)
        {
            DividendsFiat += Balance * 0.2f;  //Adds funds to fiat dividends Balance
            BuyBack += Balance * 0.05f;  //Adds funds to buyback balance
            shareHolder.ShareholdersList[CompanyIndex].Funds += Balance * 0.05f;  //Adds funds to Company Account so it can make buy-backs
            shareHolder.ShareholdersList[CompanyIndex].AvailableFunds += Balance * 0.05f;
            DevFunds += Balance * 0.45f; //Adds funds to Development Funds Balance
            SafetyNet += Balance * 0.3f; //Adds funds to safety net balance to pay debts

            Balance = 0;  //Set balance to 0, because funds have already been distributed
        }
    }
    void PayBuyBack()
    {
        if(BuyBack >= 1)
        {
            buyOrder.Buy(shareHolder.companyName,sellOrder.sellOrders[0].Price,BuyBack, timeContador);
            BuyBack = 0;
            matchEngine.CheckMatch();            
        }
    }
    void PayDebt()
    {
        totalDebtNeg = 0;
        totalDebtPos = 0;
        for (int i = 0; i < shareHolder.ShareholdersList.Count; i++)
        {
            totalDebtNeg += shareHolder.ShareholdersList[i].DebtNeg;
            totalDebtPos += shareHolder.ShareholdersList[i].DebtPos;
        }
    }
    public float Round(float num, int decimals)
    {
        num = Mathf.Round(num*Mathf.Pow(10,decimals))/Mathf.Pow(10,decimals);
        return num;
    }
}