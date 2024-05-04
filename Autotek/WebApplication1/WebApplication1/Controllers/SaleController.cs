using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.BL;
using WebApplication1.DL;
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
		[Route("UpdateSale")]
		public async Task<ActionResult> UpdateSale(TransactionRq otransactionRq)
		{
			SaleBL saleBl = new SaleBL(this.config);
			TransactionRs otransactionRs = new TransactionRs();
			if (ModelState.IsValid)
			{
				otransactionRs = await saleBl.UpdateSale(otransactionRq);
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
		[Route("GetPartyItemTransactionDetails")]
		public async Task<ActionResult> GetPartyTransactionDetails(GetPartyTransactionDetailsRq oGetPartyTransactionDetailsRq)
		{
			GetPartyTransactionDetailsRs oGetPartyTransactionDetailsRs = new GetPartyTransactionDetailsRs();
			SaleBL saleBl = new SaleBL(this.config);
			oGetPartyTransactionDetailsRs = await saleBl.GetPartyTransactionDetails(oGetPartyTransactionDetailsRq);
			return Ok(oGetPartyTransactionDetailsRs);
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("GetItemTransactions")]
		public async Task<ActionResult> GetItemTransactions(GetItemTransactionsRq oGetItemTransactionsRq)
		{
			GetItemTransactionsRs oGetItemTransactionsRs = new GetItemTransactionsRs();
			SaleBL saleBl = new SaleBL(this.config);
			oGetItemTransactionsRs = await saleBl.GetItemTransactions(oGetItemTransactionsRq);
			return Ok(oGetItemTransactionsRs);
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("GetTypeOfPayTransactions")]
		public async Task<ActionResult> GetTypeOfPayTransactions(GetTypeOfPayTransactionsRq oGetTypeOfPayTransactionsRq)
		{
			GetTypeOfPayTransactionsRs oGetTypeOfPayTransactionsRs = new GetTypeOfPayTransactionsRs();
			SaleBL saleBl = new SaleBL(this.config);
			oGetTypeOfPayTransactionsRs = await saleBl.GetTypeOfPayTransactions(oGetTypeOfPayTransactionsRq);
			return Ok(oGetTypeOfPayTransactionsRs);
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("GetLinkedPaymentTransaction")]
		public async Task<ActionResult> GetLinkedPaymentTransaction([FromQuery] Int64 registeredphonenumber, [FromQuery] string customername)
		{
			GetLinkedPaymentTransactionRs oGetLinkedPaymentTransactionRs = new GetLinkedPaymentTransactionRs();
			SaleBL saleBl = new SaleBL(this.config);
			oGetLinkedPaymentTransactionRs = await saleBl.GetLinkedPaymentTransaction(registeredphonenumber, customername);
			return Ok(oGetLinkedPaymentTransactionRs);
		}

		//no need to use this api as of nowS
		[AllowAnonymous]
		[HttpPost]
		[Route("ConvertToSaleSaleOrder")]
		public async Task<ActionResult> ConvertToSaleSaleOrder(ConvertToSaleSaleOrderRq oConvertToSaleSaleOrderRq)
		{
			GetPartyTransactionDetailsRs oConvertToSaleSaleOrderRs = new GetPartyTransactionDetailsRs();
			SaleBL saleBl = new SaleBL(this.config);
			oConvertToSaleSaleOrderRs = await saleBl.ConvertToSaleSaleOrder(oConvertToSaleSaleOrderRq);
			return Ok(oConvertToSaleSaleOrderRs);
		}
	}
}
