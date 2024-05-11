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

		[Authorize]
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

		[Authorize]
		[HttpGet]
		[Route("GetItemList")]
		public async Task<ActionResult> GetItemList([FromQuery] Int64 registeredphonenumber)
		{
			GetItemListRs oGetItemListRs = new GetItemListRs();
			if (ModelState.IsValid)
			{
				ItemBL itemBL = new ItemBL(this.config);
				oGetItemListRs = await itemBL.GetItemList(registeredphonenumber);
				return Ok(oGetItemListRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[Authorize]
		[HttpGet]
		[Route("GetItemDetails")]
		public async Task<ActionResult> GetItemDetails([FromQuery] Int64 registeredphonenumber, [FromQuery] string itemname)
		{
			GetItemRs oGetItemRs = new GetItemRs();
			if (ModelState.IsValid)
			{
				ItemBL itemBL = new ItemBL(this.config);
				oGetItemRs = await itemBL.GetItemDetails(registeredphonenumber, itemname);
				return Ok(oGetItemRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[Authorize]
		[HttpGet]
		[Route("GetCategory")]
		public async Task<ActionResult> GetCategory(Int64 registeredphonenumber)
		{
			GetCategoryRs oGetCategoryRs = new GetCategoryRs();
			if (ModelState.IsValid)
			{
				ItemBL itemBL = new ItemBL(this.config);
				oGetCategoryRs = await itemBL.GetCategory(registeredphonenumber);
				return Ok(oGetCategoryRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[Authorize]
		[HttpGet]
		[Route("GetItemByCategory")]
		public async Task<ActionResult> GetItemByCategory([FromQuery] Int64 registeredphonenumber, [FromQuery] string category)
		{
			GetItemByCategoryRs oGetItemByCategoryRs = new GetItemByCategoryRs();
			if (ModelState.IsValid)
			{
				ItemBL itemBL = new ItemBL(this.config);
				oGetItemByCategoryRs = await itemBL.GetItemByCategory(registeredphonenumber, category);
				return Ok(oGetItemByCategoryRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[Authorize]
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
