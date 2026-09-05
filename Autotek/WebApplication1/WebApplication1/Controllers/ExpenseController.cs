using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.BL;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ExpenseController : Controller
	{
		private readonly IConfiguration config;
		private readonly string dbConn;
		public ExpenseController(IConfiguration _config)
		{
			config = _config;
			dbConn = config.GetValue<string>("ConnectionStrings");
		}

		[Authorize]
		[HttpPost]
		[Route("SaveOrUpdateExpense")]
		public async Task<ActionResult> SaveOrUpdateExpense(ExpenseRq oExpenseRq)
		{
			ExpenseBL expenseBL = new ExpenseBL(this.config);
			ExpenseRs oExpenseRs = new ExpenseRs();
			if (ModelState.IsValid)
			{
				oExpenseRs = await expenseBL.AddOrUpdateExpense(oExpenseRq);
				return Ok(oExpenseRs);
			}
			return BadRequest("Please Provide Valid Details");
		}

		[Authorize]
		[HttpGet]
		[Route("GetExpenseList")]
		public async Task<ActionResult> GetExpenseList([FromQuery] Int64 registeredphonenumber)
		{
			GetExpenseListRs oGetExpenseListRs = new GetExpenseListRs();
			if (ModelState.IsValid)
			{
				ExpenseBL expenseBL = new ExpenseBL(this.config);
				oGetExpenseListRs = await expenseBL.GetExpenseList(registeredphonenumber);
				return Ok(oGetExpenseListRs);
			}
			return BadRequest("Please Provide Valid Details");
		}

		[Authorize]
		[HttpPost]
		[Route("DeleteExpense")]
		public async Task<ActionResult> DeleteExpense(DeleteExpenseRq oDeleteExpenseRq)
		{
			DeleteExpenseRs oDeleteExpenseRs = new DeleteExpenseRs();
			if (ModelState.IsValid)
			{
				ExpenseBL expenseBL = new ExpenseBL(this.config);
				oDeleteExpenseRs = await expenseBL.DeleteExpense(oDeleteExpenseRq);
				return Ok(oDeleteExpenseRs);
			}
			return BadRequest("Please Provide Valid Details");
		}
	}
}
