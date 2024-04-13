namespace WebApplication1.Models
{
	public class ItemRq
	{
		public string typeofpay { get; set; }
		public Int64 registeredphonenumber {  get; set; }
		public string itemname { get; set; }
		public string itemhsn { get; set; }
		public string baseunit { get; set; }
		public string secondaryunit { get; set; }
		public Int64 conversionrates { get; set; }
		public string category { get; set; }
		public string itemcode { get; set; }
		public Int64 saleprice { get; set; }
		public string salewithorwithouttax { get; set; }
		public Int64 discountonsaleprice { get; set; }
		public Int64 percentageoramount { get; set; }
		public Int64 wholesaleprice { get; set; }
		public string wholesalewithorwithouttax { get; set; }
		public Int64 minimumwholesalequantity { get; set; }
		public Int64 purchaseprice { get; set; }
		public string purchasewithorwithouttax { get; set; }
		public string taxrate { get; set; }
		public Int64 openingquantity { get; set; }
		public Int64 remainingquantity { get; set; }
		public Int64 atprice { get; set; }
		public DateTime asofdate { get; set; }
		public Int64 minimumstocktomaintain { get; set; }
		public string _location { get; set; }
	}

	public class ItemRs
	{
		public string status { get; set; }
	}

	public class GetItemListRq
	{
		public Int64 registeredphonenumber { get; set; }
	}

	public class GetItemListRs
	{
		public GetItemListRs()
		{
			getItemList = new List<GetItemList>();
		}
		public List<GetItemList> getItemList { get; set; }
	}

	public class GetItemList
	{
		public string itemname { get; set; }
		public Int64 remainingquantity { get; set; }
	}

	public class GetItemRq
	{
		public Int64 registeredPhoneNumber { get; set; }
		public string itemName { get; set; }
	}

	public class GetItemRs
	{
		public GetItemRs()
		{
			itemList = new List<GetAllItemList>();
		}
		public List<GetAllItemList> itemList { get; set; }
		public string status { get; set; }
		public string statusMessage { get; set; }
	}

	public class GetAllItemList
	{
		public string typeofpay { get; set; }
		public string itemhsn { get; set; }
		public string baseunit { get; set; }
		public string secondaryunit { get; set; }
		public Int64 conversionrates { get; set; }
		public string category { get; set; }
		public string itemcode { get; set; }
		public Int64 saleprice { get; set; }
		public string salewithorwithouttax { get; set; }
		public Int64 discountonsaleprice { get; set; }
		public Int64 percentageoramount { get; set; }
		public Int64 wholesaleprice { get; set; }
		public string wholesalewithorwithouttax { get; set; }
		public Int64 minimumwholesalequantity { get; set; }
		public Int64 purchaseprice { get; set; }
		public string purchasewithorwithouttax { get; set; }
		public string taxrate { get; set; }
		public Int64 openingquantity { get; set; }
		public Int64 remainingquantity { get; set; }
		public Int64 atprice { get; set; }
		public DateTime asofdate { get; set; }
		public Int64 minimumstocktomaintain { get; set; }
		public string _location { get; set; }
	}
}
