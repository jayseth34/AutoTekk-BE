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

		[AllowAnonymous]
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

		[AllowAnonymous]
		[HttpGet]
		[Route("Getcategory")]
		public async Task<ActionResult> Getcategory(Int64 registeredphonenumber)
		{
			GetcategoryRs oGetcategoryRs = new GetcategoryRs();
			if (ModelState.IsValid)
			{
				ItemBL itemBL = new ItemBL(this.config);
				oGetcategoryRs = await itemBL.Getcategory(registeredphonenumber);
				return Ok(oGetcategoryRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("GetItemBycategory")]
		public async Task<ActionResult> GetItemBycategory([FromQuery] Int64 registeredphonenumber, [FromQuery] string category)
		{
			GetItemBycategoryRs oGetItemBycategoryRs = new GetItemBycategoryRs();
			if (ModelState.IsValid)
			{
				ItemBL itemBL = new ItemBL(this.config);
				oGetItemBycategoryRs = await itemBL.GetItemBycategory(registeredphonenumber, category);
				return Ok(oGetItemBycategoryRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("AddUpdatecategory")]
		public async Task<ActionResult> AddUpdatecategory(AddUpdatecategoryRq oAddUpdatecategoryRq)
		{
			AddUpdatecategoryRs oAddUpdatecategoryRs = new AddUpdatecategoryRs();
			if (ModelState.IsValid)
			{
				ItemBL itemBL = new ItemBL(this.config);
				oAddUpdatecategoryRs = await itemBL.AddUpdatecategory(oAddUpdatecategoryRq);
				return Ok(oAddUpdatecategoryRs);
			}

			return BadRequest("Please Provide Valid Details");
		}
	}
}
