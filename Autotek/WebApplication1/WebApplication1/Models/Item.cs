using System;

namespace WebApplication1.Models
{
	public class ItemRq
	{
		public string typeofpay { get; set; }
		public Int64 registeredphonenumber {  get; set; }
		public string itemname { get; set; }
		public string olditemname { get; set; }
		public string itemhsn { get; set; }
		public string baseunit { get; set; }
		public string secondaryunit { get; set; }
		public Decimal conversionrates { get; set; }
		public string category { get; set; }
		public string itemcode { get; set; }
		public Decimal saleprice { get; set; }
		public string salewithorwithouttax { get; set; }
		public Decimal discountonsaleprice { get; set; }
		public string percentageoramounttype { get; set; }
		public Decimal wholesaleprice { get; set; }
		public string wholesalewithorwithouttax { get; set; }
		public Decimal minimumwholesalequantity { get; set; }
		public Decimal purchaseprice { get; set; }
		public string purchasewithorwithouttax { get; set; }
		public string taxrate { get; set; }
		public Decimal openingquantity { get; set; }
		public Decimal remainingquantity { get; set; }
		public Decimal atprice { get; set; }
		public DateTime asofdate { get; set; }
		public Decimal minimumstocktomaintain { get; set; }
		public string _location { get; set; }
		public bool isitemupdate { get; set; }
	}

	public class ItemRs
	{
		public string status { get; set; }
		public string statusmessage { get; set; }
	}

	public class GetItemListRs
	{
		public GetItemListRs()
		{
			getItemList = new List<GetItemList>();
		}
		public List<GetItemList> getItemList { get; set; }
		public string status { get; set; }
	}

	public class GetItemList
	{
		public string itemname { get; set; }
		public string baseunit { get; set; }
		public Decimal remainingquantity { get; set; }
		public Decimal saleprice { get; set; }
		public Decimal purchaseprice { get; set; }
		public Decimal wholesaleprice { get; set; }
		public Decimal minimumwholesalequantity { get; set; }
		public Decimal discountonsaleprice { get; set; }
		public string percentageoramounttype { get; set; }
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
		public Decimal conversionrates { get; set; }
		public string category { get; set; }
		public string itemcode { get; set; }
		public Decimal saleprice { get; set; }
		public string salewithorwithouttax { get; set; }
		public Decimal discountonsaleprice { get; set; }
		public Decimal wholesaleprice { get; set; }
		public string wholesalewithorwithouttax { get; set; }
		public Decimal minimumwholesalequantity { get; set; }
		public Decimal purchaseprice { get; set; }
		public string purchasewithorwithouttax { get; set; }
		public string taxrate { get; set; }
		public Decimal openingquantity { get; set; }
		public Decimal remainingquantity { get; set; }
		public Decimal atprice { get; set; }
		public DateTime asofdate { get; set; }
		public Decimal minimumstocktomaintain { get; set; }
		public string _location { get; set; }
		public string percentageoramounttype { get; set; }
	}

	public class GetCategoryRs
	{
		public string status { get; set; }
		public GetCategoryRs()
		{
			getCateogoryList = new List<GetCategoryListtRs>();
		}
		public List<GetCategoryListtRs> getCateogoryList { get; set; }
	}
	public class GetCategoryListtRs
	{
		public string category { get; set; }
		public Int64 categorycount { get; set; }
	}

	public class GetItemByCategoryRs
	{
		public string status { get; set; }
		public GetItemByCategoryRs()
		{
			getItemList = new List<GetItemList>();
		}
		public List<GetItemList> getItemList { get; set; }
	}

	public class AddUpdateCategoryRq
	{
		public Int64 registeredphonenumber { get; set; }
		public string newcategory { get; set; }
		public string oldcategory { get; set; }
	}

	public class AddUpdateCategoryRs
	{
		public string status { get; set; }
		public string statusmessage { get; set; }
	}

	public class AssignCodeRq
	{
		public Int64 registeredphonenumber { get; set; }
	}
	
	public class AssignCodeRs
	{
		public string status { get; set; }
		public Int64 assignedcode { get; set; }
	}
}
