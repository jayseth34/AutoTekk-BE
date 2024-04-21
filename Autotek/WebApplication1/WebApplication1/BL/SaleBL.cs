using Microsoft.AspNetCore.Http.HttpResults;
using WebApplication1.DL;
using WebApplication1.Models;

namespace WebApplication1.BL
{
	public class SaleBL
	{
		private readonly IConfiguration config;
		private readonly string dbConn;
		public SaleBL(IConfiguration _config)
		{
			config = _config;
			dbConn = config.GetValue<string>("ConnectionStrings");
		}

		public async Task<TransactionRs> SaveTransaction(TransactionRq otransactionRq)
		{
			TransactionRs otransactionRs = new TransactionRs();
			SaleDL saledl = new SaleDL(this.config);
			string result = string.Empty;
			if (otransactionRq.itemdetailslist == null || otransactionRq.itemdetailslist.Count == 0)
			{
				otransactionRs.status = "Kindly Insert Items.";
			}
			else
			{
				if(otransactionRq.isconverttosale && otransactionRq.typeofpay == "DELIVERY CHALLAN")
				{
					string typeofpay = "SALE";
					result = saledl.FindOrInsertItem(otransactionRq);
					Int64 invoicecount = saledl.GetInvoiceNumberCountDLChallan(otransactionRq.registeredphonenumber, typeofpay);
					otransactionRs = saledl.SaveDeliveryChallan(otransactionRq, invoicecount);
					saledl.UpdateDlChallan(otransactionRq.invoicenumber,otransactionRq.registeredphonenumber);
				}
				else
				{
					result = saledl.FindOrInsertItem(otransactionRq);
					otransactionRs = saledl.SaveTransaction(otransactionRq);
				}
			}
			return otransactionRs;
		}

		public async Task<GetPartyTransactionsRs> GetPartyTransactions(GetPartyTransactionsRq oGetPartyTransactionsRq)
		{
			GetPartyTransactionsRs oGetPartyTransactionsRqs = new GetPartyTransactionsRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetPartyTransactionsRqs = saledl.GetPartyTransactions(oGetPartyTransactionsRq);
			return oGetPartyTransactionsRqs;
		}

		public async Task<GetPartyTransactionDetailsRs> GetPartyTransactionDetails(GetPartyTransactionDetailsRq oGetPartyTransactionDetailsRq)
		{
			GetPartyTransactionDetailsRs oGetPartyTransactionDetailsRs = new GetPartyTransactionDetailsRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetPartyTransactionDetailsRs = saledl.GetPartyTransactionDetails(oGetPartyTransactionDetailsRq);
			return oGetPartyTransactionDetailsRs;
		}

		public async Task<GetItemTransactionsRs> GetItemTransactions(GetItemTransactionsRq oGetItemTransactionsRq)
		{
			GetItemTransactionsRs oGetItemTransactionsRs = new GetItemTransactionsRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetItemTransactionsRs.itemTransactionsList = saledl.GetItemTransactions(oGetItemTransactionsRq);
			oGetItemTransactionsRs = saledl.GetItemHeaderDetails(oGetItemTransactionsRq, oGetItemTransactionsRs.itemTransactionsList);
			return oGetItemTransactionsRs;
		}
		public async Task<GetTypeOfPayTransactionsRs> GetTypeOfPayTransactions(GetTypeOfPayTransactionsRq oGetTypeOfPayTransactionsRq)
		{
			GetTypeOfPayTransactionsRs oGetTypeOfPayTransactionsRs = new GetTypeOfPayTransactionsRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetTypeOfPayTransactionsRs = saledl.GetTypeOfPayTransactions(oGetTypeOfPayTransactionsRq);
			if (oGetTypeOfPayTransactionsRq.typeofpay == "SALE" || oGetTypeOfPayTransactionsRq.typeofpay == "ESTIMATION/ QUOTATION" || oGetTypeOfPayTransactionsRq.typeofpay == "PAYMENT IN" ||
				oGetTypeOfPayTransactionsRq.typeofpay == "SALE ORDER" || oGetTypeOfPayTransactionsRq.typeofpay == "DELIVERY CHALLAN" || oGetTypeOfPayTransactionsRq.typeofpay == "SALE RETURN/ CR. NOTE")
			{
				oGetTypeOfPayTransactionsRs.invoicenumbercount = saledl.GetInvoiceNumberCount(oGetTypeOfPayTransactionsRq);
			}
			return oGetTypeOfPayTransactionsRs;
		}

	}
}
