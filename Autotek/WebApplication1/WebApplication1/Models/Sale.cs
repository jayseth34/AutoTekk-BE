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
		}
		public List<ItemDetailsListRq> itemdetailslist { get; set; }

	}

	public class ItemDetailsListRq
	{
		public string item {  get; set; }
		public Decimal qty { get; set; }
		public string unit { get; set; }
		public Decimal priceperunit { get; set; }
		public Int64 remainingquantity { get; set; }
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
}
