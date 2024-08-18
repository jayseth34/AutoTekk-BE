namespace WebApplication1.Models
{
	public class TransactionRq
	{
		public string typeofpay {  get; set; }
		public Int64 invoicenumber { get; set; }
		public Int64 convertinvoicenumber { get; set; }
		public DateTime invoicedate { get; set; }
		public string stateofsupply { get; set; }
		public string paymenttype { get; set; }
		public Decimal total {  get; set; }
		public Decimal received { get; set; }
		public Decimal balance { get; set; }
		public string customername { get; set; }
		public Int64 phonenumber { get; set; }
		public Int64 registeredphonenumber { get; set; }
		public Decimal topayparty { get; set; }
		public Decimal toreceivefromparty { get; set; }
		//public Int64 remainingquantity { get; set; }
		public string billingaddress { get; set; }
		public string shippingaddress { get; set; }
		public string paymentstatus { get; set; }
		public bool ispurchaseconvert { get; set; }
		public bool issaleconvert { get; set; }
		public bool issaleorderconvert { get; set; }
		public bool isupdate { get; set; }
		public TransactionRq()
		{
			itemdetailslist = new List<ItemDetailsListRq>();
			amountdetailslist = new List<AmountDetails>();
		}
		public List<ItemDetailsListRq> itemdetailslist { get; set; }
		public List<AmountDetails>? amountdetailslist { get; set; }

	}

	public class AmountDetails
	{
		public string type { get; set; }
		public decimal amount { get; set; }
		public string? refno { get; set; }
	}

	public class ItemDetailsListRq
	{
		public string item {  get; set; }
		public Decimal qty { get; set; }
		public string unit { get; set; }
		public Decimal priceperunit { get; set; }
		public Decimal remainingquantity { get; set; }
		public string queryoperationtype { get; set; } = "";
		public Decimal taxrate { get; set; } = 0;
		public Decimal taxrateamount { get; set; } = 0;
		public Int64 transactionid { get; set; }
		public Decimal discountpercent { get; set; }
		public Decimal discountamount { get; set; }
	}

	public class TransactionRs
	{
		public string status { get; set; }
		public string statusmessage {  get; set; }
	}

	public class GetPartyTransactionsRs
	{
		public string? status { get; set;}
		public string? statusmessage { get; set;}
		public string? emailid { get; set; } = "";
		public string? gst { get; set; } = "";
		public string? billingaddress { get; set; } = "";
		public Int64? phonenumber { get; set; } = 0;
		public Decimal? creditlimit { get; set; } = 0;
		public GetPartyTransactionsRs()
		{
			partyTransactionsList = new List<GetAllPartyTransactionsList>();
		}
		public List<GetAllPartyTransactionsList> partyTransactionsList { get; set; }
	}
	public class GetAllPartyTransactionsList
	{
		public string typeofpay { get; set; }
		public string paymentstatus { get; set; }
		public Int64 invoicenumber { get; set; }
		public Decimal creditlimit { get; set; }
		public DateTime invoicedate { get; set; }
		public Decimal total { get; set; }
		public Decimal balance { get; set; }
	}

	public class GetPartyTransactionDetailsRq
	{
		public Int64 registeredphonenumber { get; set; }
		public Int64 invoicenumber { get; set; }
		public string typeofpay { get; set; }
		public bool issaleconvert {  get; set; }
		public bool issaleorderconvert {  get; set; }
	}

	public class GetPartyTransactionDetailsRs
	{
		public string typeofpay { get; set; }
		public DateTime invoicedate { get; set; }
		public string stateofsupply { get; set; }
		public string paymenttype { get; set; }
		public Decimal total { get; set; }
		public Decimal received { get; set; }
		public Decimal balance { get; set; }
		public string customername { get; set; }
		public Int64 phonenumber { get; set; }
		public string billingaddress { get; set; }
		public string shippingaddress { get; set; }
		public string status { get; set; }
		public Int64 invoicenumbercount { get; set; }
		public Decimal topayparty { get; set; }
		public Decimal toreceivefromparty { get; set; }

		public GetPartyTransactionDetailsRs()
		{
			itemdetailslist = new List<ItemDetailsListRs>();
		}
		public List<ItemDetailsListRs> itemdetailslist { get; set; }
	}

	public class PaymentInOutTrnxRs
	{
		public string status { get; set; }
		public PaymentInOutTrnxRs()
		{
			inouttrnxlist = new List<ListPaymentInOutTrnxRs>();
		}
		public List<ListPaymentInOutTrnxRs> inouttrnxlist { get; set; }

	}

	public class ListPaymentInOutTrnxRs
	{
		public DateTime invoicedate { get; set; }
		public Int64 invoicenumber { get; set; }
		public Decimal linkedamount { get; set; }
		public string typeofpay { get; set; }
	}

	public class ItemDetailsListRs
	{
		public string item { get; set; }
		public Decimal qty { get; set; }
		public string unit { get; set; }
		public Decimal priceperunit { get; set; }
		public Int64 transactionid { get; set; }
		public string taxrate { get; set; }
		public Decimal taxrateamount { get; set; }
		public Decimal discountpercent { get; set; }
		public Decimal discountamount { get; set; }
	}

	public class GetItemTransactionsRq
	{
		public Int64 registeredphonenumber { get; set; }
		public string itemname { get; set; }
	}

	public class GetItemTransactionsRs
	{
		public string status { get; set; }
		public Decimal saleprice { get; set; }
		public Decimal purchaseprice { get; set; }
		public Decimal wholesaleprice { get; set; }
		public Decimal remainingquantity { get; set; }

		public GetItemTransactionsRs()
		{
			itemTransactionsList = new List<GetAllItemTransactionsList>();
		}
		public List<GetAllItemTransactionsList> itemTransactionsList { get; set; }
	}

	public class GetAllItemTransactionsList
	{
		public string typeofpay { get; set; }
		public Int64 invoicenumber { get; set; }
		public string partyname { get; set; }
		public DateTime invoicedate { get; set; }
		public Decimal qty { get; set; }
		public Decimal priceperunit { get; set; }
		public string paymentstatus { get; set; }
		public Decimal purchaseprice { get; set; }
	}

	public class GetTypeOfPayTransactionsRq
	{
		public Int64 registeredphonenumber { get; set; }
		public string typeofpay { get; set; }
	}
	public class GetTypeOfPayTransactionsRs
	{
		public string status { get; set; }
		public Int64 invoicenumbercount {  get; set; }
		public GetTypeOfPayTransactionsRs()
		{
			typeofpaytransactionlist = new List<GetTypeOfPayTransactionsList>();
		}
		public List<GetTypeOfPayTransactionsList> typeofpaytransactionlist { get; set; }
	} 

	public class GetLinkedPaymentTransactionRs
	{
		public string status { get; set; }
		public GetLinkedPaymentTransactionRs()
		{
			getLinkedPaymentTransactionList = new List<GetLinkedPaymentTransactionList>();
		}
		public List<GetLinkedPaymentTransactionList> getLinkedPaymentTransactionList { get; set; }
	}

	public class GetLinkedPaymentTransactionList
	{
		public Int64 invoicenumber { get; set; }
		public DateTime invoicedate { get; set; }
		public string typeofpay { get; set; }
		public Decimal total { get; set; }
		public Decimal balance { get; set; }
		public Decimal linkedamount { get; set; }
		public Int64 registeredphonenumber { get; set; }
		public Decimal topayparty { get; set; }
		public Decimal toreceivefromparty { get; set; }
		public string customername {  get; set; }
		public Decimal unused {  get; set; }
		public Int64 paymentininvoicenumber {  get; set; }
	}

	public class GetTypeOfPayTransactionsList
	{
		public Int64 invoicenumber { get; set; }
		public DateTime invoicedate { get; set; }
		public string customername { get; set; }
		public string typeofpay { get; set; }
		public string paymenttype { get; set; }
		public Decimal total { get; set; }
		public Decimal balance { get; set; }
		public string paymentstatus { get; set; }
		public bool isconverted { get; set; }
	}

	public class UpadatePaymentInOutTrnxRq
	{
		public Int64 invoicenumber { get; set; }
		public DateTime invoicedate { get; set; }
		public Decimal received { get; set; }
		public Int64 registeredphonenumber { get; set; }
		public string paymenttype { get; set; }
		public string customername { get; set; }
		public string typeofpay { get; set; }
	}

	public class UpadatePaymentInOutTrnxRs
	{
		public string status { get; set; }
	}

	public class ConvertToSaleSaleOrderRq
	{
		public string typeofpay { get; set; }
		public Int64 registeredphonenumber { get; set; }
		public string customername { get; set; }
		public Int64 invoicenumber { get; set; }
		public bool isconvert { get; set; }
	}

	public class ConvertToSaleSaleOrderRs
	{
		public string status { get; set; }
		public Int64 invoicenumbercount { get; set; }
	}

	public class GetPartyAmounts
	{
		public Decimal topayparty { get; set; }
		public Decimal toreceivefromparty { get; set; }
	}

	public class BankFormRq
	{
		public string oldaccountdisplayname { get; set; }
		public string newaccountdisplayname { get; set; }
		public decimal newopeningbalance { get; set; }
		public decimal oldopeningbalance { get; set; }
		public DateTime asofdate { get; set; }
		public decimal? amount { get; set; }
		public string? typeofpay { get; set; }
		public Int64 registeredphonenumber { get; set; }
		public Boolean isbanksupdate { get; set; }
	}

	public class BankAdjustments
	{
		public string? transfertype { get; set; }
		public string? fromaccount { get; set; }
		public string? toaccount { get; set; }
		public decimal? amount { get; set; }
		public DateTime? adjustmentdate { get; set; }
		public string? accountname { get; set; }
		public string? adjustmenttype { get; set; }
		public string? description { get; set; }
		public string? typeofpay { get; set; }
		public Int64 resgisteredphonenumber { get; set; }
	}

	public class BankFormRs
	{
		public string status { get; set; }
		public string statusmessage { get; set; }
	}

	public class GetBankDetailsRq
	{
		public string accountdisplayname { get; set; }
		public Int64 registeredphonenumber { get; set; }
	}

	public class GetBankDetailsRs
	{
		public string status { get; set; }
		public string statusmessage { get; set; }
		public GetBankDetailsRs()
		{
			bankTrnxDetails = new List<BankTrnxDetails>();
		}
		public List<BankTrnxDetails> bankTrnxDetails { get; set; }

	}

	public class BankTrnxDetails
	{
		public string? typeofpay { get; set; }
		public string? customername { get; set; }
		public DateTime? invoicedate { get; set; }
		public Decimal amount { get; set; }
		public Decimal transactionid { get; set; }
	}

	public class GetBanksRq
	{
		public Int64 registeredphonenumber { get; set; }
	}

	public class GetBanksRs
	{
		public string status { get; set; }
		public string statusmessage { get; set; }
		public GetBanksRs()
		{
			bankslist = new List<BanksList>();
		}
		public List<BanksList> bankslist { get; set; }
	}

	public class BanksList
	{
		public string accountdisplayname { get; set; }
		public Decimal amount { get; set; }
	}

	public class GetBanksDetailsValuesRq
	{
		public Int64 registeredphonenumber { get; set; }
		public string newaccountdisplayname {  get; set; }
	}

	public class GetBanksDetailsValuesRs
	{
		public string status { get; set; }
		public string newaccountdisplayname { get; set; }
		public Decimal newopeningbalance { get; set; }
		public Decimal amount { get; set; }
		public DateTime? asofDate { get; set; }
	}

	public class TransfersRq
	{
		public Int64 registeredphonenumber { get; set; }
		public string type {  get; set; }
		public string fromAccount {  get; set; }
		public string toAccount {  get; set; }
		public Decimal amount {  get; set; }
		public DateTime adjustmentDate {  get; set; }
		public string description { get; set; }
		public string adjustmentType { get; set; }
		public string accountName { get; set; }
	}

	public class TransfersRs
	{
		public string status { get; set; }
		public string statusmessage { get; set; }
	}
}
