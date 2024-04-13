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

		public async Task<GetItemListRs> GetItemList(GetItemListRq oGetItemListRq)
		{
			GetItemListRs oGetItemListRs = new GetItemListRs();
			ItemDL itemDL = new ItemDL(this.config);
			oGetItemListRs = itemDL.GetItemList(oGetItemListRq);
			return oGetItemListRs;
		}

		public async Task<GetItemRs> GetItemDetails(GetItemRq oGetItemRq)
		{
			GetItemRs oGetItemRs = new GetItemRs();
			ItemDL itemDL = new ItemDL(this.config);
			oGetItemRs = itemDL.GetItemDetails(oGetItemRq);
			return oGetItemRs;
		}
	}
}
