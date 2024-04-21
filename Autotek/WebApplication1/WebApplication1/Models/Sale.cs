namespace WebApplication1.Models
{
	public class TransactionRq
	{
		public string typeofpay {  get; set; }
		public Int64 invoicenumber { get; set; }
		public DateTime invoicedate { get; set; }
		public string stateofsupply { get; set; }
		public string paymenttype { get; set; }
		public Int64 total {  get; set; }
		public Int64 received { get; set; }
		public Int64 balance { get; set; }
		public string customername { get; set; }
		public Int64 phonenumber { get; set; }
		public Int64 registeredphonenumber { get; set; }
		public Int64 partybalance { get; set; }
		public Int64 remainingquantity { get; set; }
		public string billingaddress { get; set; }
		public string shippingaddress { get; set; }
		public string paymentstatus { get; set; }
		public bool isconverttosale { get; set; }
		public TransactionRq()
		{
			itemdetailslist = new List<ItemDetailsListRq>();
		}
		public List<ItemDetailsListRq> itemdetailslist { get; set; }

	}

	public class ItemDetailsListRq
	{
		public string item {  get; set; }
		public Int64 qty { get; set; }
		public string unit { get; set; }
		public Int64 priceperunit { get; set; }
		public Int64 remainingquantity { get; set; }
	}

	public class TransactionRs
	{
		public string status { get; set; }
		public string statusmessage {  get; set; }
	}

	public class GetPartyTransactionsRq
	{
		public Int64 registeredphonenumber { get; set; }
		public string customername { get; set;}
	}

	public class GetPartyTransactionsRs
	{
		public string status { get; set;}
		public string emailid { get; set; }
		public string gst { get; set; }
		public string billingaddress { get; set; }
		public Int64 phonenumber { get; set; }
		public GetPartyTransactionsRs()
		{
			partyTransactionsList = new List<GetAllPartyTransactionsList>();
		}
		public List<GetAllPartyTransactionsList> partyTransactionsList { get; set; }
	}
	public class GetAllPartyTransactionsList
	{
		public string typeofpay { get; set; }
		public Int64 invoicenumber { get; set; }
		public Int64 creditlimit { get; set; }
		public DateTime invoicedate { get; set; }
		public Int64 total { get; set; }
		public Int64 balance { get; set; }
	}

	public class GetPartyTransactionDetailsRq
	{
		public Int64 registeredphonenumber { get; set; }
		public Int64 invoicenumber { get; set; }
	}

	public class GetPartyTransactionDetailsRs
	{
		public string typeofpay { get; set; }
		public DateTime invoicedate { get; set; }
		public string stateofsupply { get; set; }
		public string paymenttype { get; set; }
		public Int64 total { get; set; }
		public Int64 received { get; set; }
		public Int64 balance { get; set; }
		public string customername { get; set; }
		public Int64 phonenumber { get; set; }
		public string billingaddress { get; set; }
		public string shippingaddress { get; set; }
		public string status { get; set; }
		public GetPartyTransactionDetailsRs()
		{
			itemdetailslist = new List<ItemDetailsListRs>();
		}
		public List<ItemDetailsListRs> itemdetailslist { get; set; }
	}

	public class ItemDetailsListRs
	{
		public string item { get; set; }
		public Int64 qty { get; set; }
		public string unit { get; set; }
		public Int64 priceperunit { get; set; }
	}

	public class GetItemTransactionsRq
	{
		public Int64 registeredphonenumber { get; set; }
		public string itemname { get; set; }
	}

	public class GetItemTransactionsRs
	{
		public string status { get; set; }
		public Int64 saleprice { get; set; }
		public Int64 purchaseprice { get; set; }
		public Int64 wholesaleprice { get; set; }
		public Int64 remainingquantity { get; set; }

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
		public string partyName { get; set; }
		public DateTime invoicedate { get; set; }
		public Int64 qty { get; set; }
		public Int64 priceperunit { get; set; }
		public string paymentstatus { get; set; }
		public Int64 purchaseprice { get; set; }
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

	public class GetTypeOfPayTransactionsList
	{
		public Int64 invoicenumber { get; set; }
		public DateTime invoicedate { get; set; }
		public string customername { get; set; }
		public string typeofpay { get; set; }
		public string paymenttype { get; set; }
		public Int64 total { get; set; }
		public Int64 balance { get; set; }
		public string paymentstatus { get; set; }
	}
}
