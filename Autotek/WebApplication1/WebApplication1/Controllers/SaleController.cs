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

		[Authorize]
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

		[Authorize]
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

		[Authorize]
		[HttpGet]
		[Route("GetPartyTransactions")]
		public async Task<ActionResult> GetPartyTransactions([FromQuery] Int64 registeredphonenumber, [FromQuery] string customername)
		{
			GetPartyTransactionsRs oGetPartyTransactionsRs = new GetPartyTransactionsRs();
			SaleBL saleBl = new SaleBL(this.config);
			oGetPartyTransactionsRs = await saleBl.GetPartyTransactions(registeredphonenumber, customername);
			return Ok(oGetPartyTransactionsRs);
		}

		[Authorize]
		[HttpPost]
		[Route("GetPartyItemTransactionDetails")]
		public async Task<ActionResult> GetPartyTransactionDetails(GetPartyTransactionDetailsRq oGetPartyTransactionDetailsRq)
		{
			GetPartyTransactionDetailsRs oGetPartyTransactionDetailsRs = new GetPartyTransactionDetailsRs();
			SaleBL saleBl = new SaleBL(this.config);
			oGetPartyTransactionDetailsRs = await saleBl.GetPartyTransactionDetails(oGetPartyTransactionDetailsRq);
			return Ok(oGetPartyTransactionDetailsRs);
		}

		[Authorize]
		[HttpPost]
		[Route("GetPaymentInOutTransactionDetails")]
		public async Task<ActionResult> GetPaymentInOutTransactionDetails(GetPartyTransactionDetailsRq oGetPartyTransactionDetailsRq)
		{
			PaymentInOutTrnxRs oPaymentInOutTrnxRs = new PaymentInOutTrnxRs();
			SaleBL saleBl = new SaleBL(this.config);
			oPaymentInOutTrnxRs = await saleBl.GetPaymentInOutTransactionDetails(oGetPartyTransactionDetailsRq);
			return Ok(oPaymentInOutTrnxRs);
		}

		[Authorize]
		[HttpGet]
		[Route("GetItemTransactions")]
		public async Task<ActionResult> GetItemTransactions([FromQuery] Int64 registeredphonenumber, [FromQuery] string itemname)
		{
			GetItemTransactionsRs oGetItemTransactionsRs = new GetItemTransactionsRs();
			SaleBL saleBl = new SaleBL(this.config);
			oGetItemTransactionsRs = await saleBl.GetItemTransactions(registeredphonenumber, itemname);
			return Ok(oGetItemTransactionsRs);
		}

		[Authorize]
		[HttpGet]
		[Route("GetTypeOfPayTransactions")]
		public async Task<ActionResult> GetTypeOfPayTransactions([FromQuery] Int64 registeredphonenumber, [FromQuery] string typeofpay)
		{
			GetTypeOfPayTransactionsRs oGetTypeOfPayTransactionsRs = new GetTypeOfPayTransactionsRs();
			SaleBL saleBl = new SaleBL(this.config);
			oGetTypeOfPayTransactionsRs = await saleBl.GetTypeOfPayTransactions(registeredphonenumber, typeofpay);
			return Ok(oGetTypeOfPayTransactionsRs);
		}

		[Authorize]
		[HttpGet]
		[Route("GetLinkedPaymentTransaction")]
		public async Task<ActionResult> GetLinkedPaymentTransaction([FromQuery] Int64 registeredphonenumber, [FromQuery] string customername)
		{
			GetLinkedPaymentTransactionRs oGetLinkedPaymentTransactionRs = new GetLinkedPaymentTransactionRs();
			SaleBL saleBl = new SaleBL(this.config);
			oGetLinkedPaymentTransactionRs = await saleBl.GetLinkedPaymentTransaction(registeredphonenumber, customername);
			return Ok(oGetLinkedPaymentTransactionRs);
		}

		[Authorize]
		[HttpPost]
		[Route("UpdateLinkedPaymentTransaction")]
		public async Task<ActionResult> UpdateLinkedPaymentTransaction([FromBody] List<GetLinkedPaymentTransactionList> transactions)
		{
			SaleBL saleBl = new SaleBL(this.config);
			bool result = await saleBl.UpdateLinkedPaymentTransaction(transactions);
			if (result)
			{
				return Ok(new { status = "SUCCESS" });
			}
			else
			{
				return BadRequest(new { status = "FAILED" });
			}
		}

		[Authorize]
		[HttpPost]
		[Route("UpdatePaymentInOutTrnx")]
		public async Task<ActionResult> UpdatePaymentInOutTrnx(UpadatePaymentInOutTrnxRq oUpadatePaymentInOutTrnxRq)
		{
			UpadatePaymentInOutTrnxRs oUpadatePaymentInOutTrnxRs = new UpadatePaymentInOutTrnxRs();
			SaleBL saleBL = new SaleBL(this.config);
			oUpadatePaymentInOutTrnxRs = await saleBL.UpdatePaymentInOutTrnx(oUpadatePaymentInOutTrnxRq);
			return Ok(oUpadatePaymentInOutTrnxRs);
		}

		[Authorize]
		[HttpPost]
		[Route("GetUpdatedTrnxInOutVal")]
		public async Task<ActionResult> GetUpdatedTrnxInOutVal(GetUpdatedTrnxInOutValRq oGetUpdatedTrnxInOutValRq)
		{
			GetUpdatedTrnxInOutValRs oGetUpdatedTrnxInOutValRs = new GetUpdatedTrnxInOutValRs();
			SaleBL saleBL = new SaleBL(this.config);
			oGetUpdatedTrnxInOutValRs = await saleBL.GetUpdatedTrnxInOutVal(oGetUpdatedTrnxInOutValRq);
			return Ok(oGetUpdatedTrnxInOutValRs);
		}

		[Authorize]
		[HttpPost]
		[Route("SaveBankDetails")]
		public async Task<ActionResult> SaveBankDetails(BankFormRq oBankFormRq)
		{
			BankFormRs oBankFormRs = new BankFormRs();
			SaleBL saleBL = new SaleBL(this.config);
			oBankFormRs = await saleBL.SaveBankDetails(oBankFormRq);
			return Ok(oBankFormRs);
		}

		[Authorize]
		[HttpPost]
		[Route("GetBankDetails")]
		public async Task<ActionResult> GetBankDetails(GetBankDetailsRq oGetBankDetailsRq)
		{
			GetBankDetailsRs oGetBankDetailsRs = new GetBankDetailsRs();
			SaleBL saleBL = new SaleBL(this.config);
			oGetBankDetailsRs = await saleBL.GetBankDetails(oGetBankDetailsRq);
			return Ok(oGetBankDetailsRs);
		}

		[Authorize]
		[HttpPost]
		[Route("GetBanks")]
		public async Task<ActionResult> GetBanks(GetBanksRq oGetBanksRq)
		{
			GetBanksRs oGetBanksRs = new GetBanksRs();
			SaleBL saleBL = new SaleBL(this.config);
			oGetBanksRs = await saleBL.GetBanks(oGetBanksRq);
			return Ok(oGetBanksRs);
		}

		[Authorize]
		[HttpPost]
		[Route("GetBanksDetailsValues")]
		public async Task<ActionResult> GetBanksDetailsValues(GetBanksDetailsValuesRq oGetBanksDetailsValuesRq)
		{
			GetBanksDetailsValuesRs oGetBanksDetailsValuesRs = new GetBanksDetailsValuesRs();
			SaleBL saleBL = new SaleBL(this.config);
			oGetBanksDetailsValuesRs = await saleBL.GetBanksDetailsValues(oGetBanksDetailsValuesRq);
			return Ok(oGetBanksDetailsValuesRs);
		}

		[Authorize]
		[HttpPost]
		[Route("transfers")]
		public async Task<ActionResult> Transfers(TransfersRq oTransfersRq)
		{
			TransfersRs oTransfersRs = new TransfersRs();
			SaleBL saleBL = new SaleBL(this.config);
			oTransfersRs = await saleBL.Transfers(oTransfersRq);
			return Ok(oTransfersRs);
		}

		//no need to use this api as of nowS
		[Authorize]
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
