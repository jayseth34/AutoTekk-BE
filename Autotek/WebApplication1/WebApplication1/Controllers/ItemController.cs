using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.BL;
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
		[Route("AddItem")]
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
	}
}
