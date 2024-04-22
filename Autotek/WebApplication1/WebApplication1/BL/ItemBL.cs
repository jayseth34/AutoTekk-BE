using Microsoft.AspNetCore.Mvc;
using WebApplication1.DL;
using WebApplication1.Models;

namespace WebApplication1.BL
{
	public class ItemBL
	{
		private readonly IConfiguration config;
		private readonly string dbConn;
		public ItemBL(IConfiguration _config)
		{
			config = _config;
			dbConn = config.GetValue<string>("ConnectionStrings");
		}

		public async Task<ItemRs> AddItem(ItemRq oitemRq)
		{
			ItemRs oitemRs = new ItemRs();
			ItemDL itemDL = new ItemDL(this.config);
			oitemRs = itemDL.AddItem(oitemRq);
			return oitemRs;
		}

		public async Task<GetItemListRs> GetItemList(Int64 registeredphonenumber)
		{
			GetItemListRs oGetItemListRs = new GetItemListRs();
			ItemDL itemDL = new ItemDL(this.config);
			oGetItemListRs = itemDL.GetItemList(registeredphonenumber);
			return oGetItemListRs;
		}

		public async Task<GetItemRs> GetItemDetails(Int64 registeredphonenumber, string itemname)
		{
			GetItemRs oGetItemRs = new GetItemRs();
			ItemDL itemDL = new ItemDL(this.config);
			oGetItemRs = itemDL.GetItemDetails(registeredphonenumber, itemname);
			return oGetItemRs;
		}

		public async Task<GetcategoryRs> Getcategory(Int64 registeredphonenumber)
		{
			GetcategoryRs oGetcategoryRs = new GetcategoryRs();
			ItemDL itemDL = new ItemDL(this.config);
			oGetcategoryRs = itemDL.Getcategory(registeredphonenumber);
			return oGetcategoryRs;
		}

		public async Task<GetItemBycategoryRs> GetItemBycategory(Int64 registeredphonenumber, string category)
		{
			GetItemBycategoryRs oGetItemBycategoryRs = new GetItemBycategoryRs();
			ItemDL itemDL = new ItemDL(this.config);
			oGetItemBycategoryRs = itemDL.GetItemBycategory(registeredphonenumber, category);
			return oGetItemBycategoryRs;
		}
		public async Task<AddUpdatecategoryRs> AddUpdatecategory(AddUpdatecategoryRq oAddUpdatecategoryRq)
		{
			AddUpdatecategoryRs oAddUpdatecategoryRs = new AddUpdatecategoryRs();
			ItemDL itemDL = new ItemDL(this.config);
			oAddUpdatecategoryRs = itemDL.AddUpdatecategory(oAddUpdatecategoryRq);
			return oAddUpdatecategoryRs;
		}
	}
}
