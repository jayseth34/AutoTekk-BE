using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.BL;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SaleController : Controller
	{
		private readonly IConfiguration config;
		private readonly string dbConn;
		public SaleController(IConfiguration _config)
		{
			config = _config;
			dbConn = config.GetValue<string>("ConnectionStrings");
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("AddSale")]
		public async Task<ActionResult> AddSale(TransactionRq otransactionRq)
		{
			SaleBL saleBl = new SaleBL(this.config);
			TransactionRs otransactionRs = new TransactionRs();
			if (ModelState.IsValid)
			{
				otransactionRs = await saleBl.SaveTransaction(otransactionRq);
			}
			return Ok(otransactionRs);	
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("GetPartyTransactions")]
		public async Task<ActionResult> GetPartyTransactions(GetPartyTransactionsRq oGetPartyTransactionsRq)
		{
			GetPartyTransactionsRs oGetPartyTransactionsRs = new GetPartyTransactionsRs();
			SaleBL saleBl = new SaleBL(this.config);
			oGetPartyTransactionsRs = await saleBl.GetPartyTransactions(oGetPartyTransactionsRq);
			return Ok(oGetPartyTransactionsRs);
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("GetPartyTransactionDetails")]
		public async Task<ActionResult> GetPartyTransactionDetails(GetPartyTransactionDetailsRq oGetPartyTransactionDetailsRq)
		{
			GetPartyTransactionDetailsRs oGetPartyTransactionDetailsRs = new GetPartyTransactionDetailsRs();
			SaleBL saleBl = new SaleBL(this.config);
			oGetPartyTransactionDetailsRs = await saleBl.GetPartyTransactionDetails(oGetPartyTransactionDetailsRq);
			return Ok(oGetPartyTransactionDetailsRs);
		}
	}
}
