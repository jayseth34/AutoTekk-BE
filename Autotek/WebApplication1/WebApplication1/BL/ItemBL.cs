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

		public async Task<GetCategoryRs> GetCategory(Int64 registeredphonenumber)
		{
			GetCategoryRs oGetCategoryRs = new GetCategoryRs();
			ItemDL itemDL = new ItemDL(this.config);
			oGetCategoryRs = itemDL.GetCategory(registeredphonenumber);
			return oGetCategoryRs;
		}

		public async Task<GetItemByCategoryRs> GetItemByCategory(Int64 registeredphonenumber, string category)
		{
			GetItemByCategoryRs oGetItemByCategoryRs = new GetItemByCategoryRs();
			ItemDL itemDL = new ItemDL(this.config);
			oGetItemByCategoryRs = itemDL.GetItemByCategory(registeredphonenumber, category);
			return oGetItemByCategoryRs;
		}
		public async Task<AddUpdateCategoryRs> AddUpdateCategory(AddUpdateCategoryRq oAddUpdateCategoryRq)
		{
			AddUpdateCategoryRs oAddUpdateCategoryRs = new AddUpdateCategoryRs();
			ItemDL itemDL = new ItemDL(this.config);
			oAddUpdateCategoryRs = itemDL.AddUpdateCategory(oAddUpdateCategoryRq);
			return oAddUpdateCategoryRs;
		}
	}
}
