using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.BL;
using WebApplication1.DL;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ItemController : Controller
	{
		private readonly IConfiguration config;
		private readonly string dbConn;
		public ItemController(IConfiguration _config)
		{
			config = _config;
			dbConn = config.GetValue<string>("ConnectionStrings");
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("SaveOrUpdateItem")]
		public async Task<ActionResult> AddItem(ItemRq oitemRq)
		{
			ItemBL itemBL = new ItemBL(this.config);
			ItemRs oitemRs = new ItemRs();
			if (ModelState.IsValid)
			{
				oitemRs = await itemBL.AddItem(oitemRq);
				return Ok(oitemRs);
			}
			return BadRequest("");
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("GetItemList")]
		public async Task<ActionResult> GetItemList(GetItemListRq oGetItemListRq)
		{
			GetItemListRs oGetItemListRs = new GetItemListRs();
			if (ModelState.IsValid)
			{
				ItemBL itemBL = new ItemBL(this.config);
				oGetItemListRs = await itemBL.GetItemList(oGetItemListRq);
				return Ok(oGetItemListRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("GetItemDetails")]
		public async Task<ActionResult> GetItemDetails(GetItemRq oGetItemRq)
		{
			GetItemRs oGetItemRs = new GetItemRs();
			if (ModelState.IsValid)
			{
				ItemBL itemBL = new ItemBL(this.config);
				oGetItemRs = await itemBL.GetItemDetails(oGetItemRq);
				return Ok(oGetItemRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("GetCategory")]
		public async Task<ActionResult> GetCategory(GetCategoryRq oGetCategoryRq)
		{
			GetCategoryRs oGetCategoryRs = new GetCategoryRs();
			if (ModelState.IsValid)
			{
				ItemBL itemBL = new ItemBL(this.config);
				oGetCategoryRs = await itemBL.GetCategory(oGetCategoryRq);
				return Ok(oGetCategoryRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("GetItemByCategory")]
		public async Task<ActionResult> GetItemByCategory(GetItemByCategoryRq oGetItemByCategoryRq)
		{
			GetItemByCategoryRs oGetItemByCategoryRs = new GetItemByCategoryRs();
			if (ModelState.IsValid)
			{
				ItemBL itemBL = new ItemBL(this.config);
				oGetItemByCategoryRs = await itemBL.GetItemByCategory(oGetItemByCategoryRq);
				return Ok(oGetItemByCategoryRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("AddUpdateCategory")]
		public async Task<ActionResult> AddUpdateCategory(AddUpdateCategoryRq oAddUpdateCategoryRq)
		{
			AddUpdateCategoryRs oAddUpdateCategoryRs = new AddUpdateCategoryRs();
			if (ModelState.IsValid)
			{
				ItemBL itemBL = new ItemBL(this.config);
				oAddUpdateCategoryRs = await itemBL.AddUpdateCategory(oAddUpdateCategoryRq);
				return Ok(oAddUpdateCategoryRs);
			}

			return BadRequest("Please Provide Valid Details");
		}
	}
}
