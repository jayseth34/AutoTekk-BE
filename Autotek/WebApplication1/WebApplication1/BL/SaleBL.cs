using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
				if(otransactionRq.isconvert && otransactionRq.isupdate && (otransactionRq.typeofpay == "SALE" || otransactionRq.typeofpay == "SALE ORDER"))
				{
					string typeofpay = string.Empty;
					if (otransactionRq.typeofpay == "SALE ORDER")
					{
						typeofpay = "SALE ORDER";
					}
					else if (otransactionRq.typeofpay == "SALE")
					{
						typeofpay = "SALE";
					}
					
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
			SaleDL saledl = new SaleDL(this.config);
			GetPartyTransactionDetailsRs oGetPartyTransactionDetailsRs = new GetPartyTransactionDetailsRs();
			
			
			oGetPartyTransactionDetailsRs = saledl.GetPartyTransactionDetails(oGetPartyTransactionDetailsRq);
			GetTypeOfPayTransactionsRq oGetTypeOfPayTransactionsRq = new GetTypeOfPayTransactionsRq();
			
			oGetTypeOfPayTransactionsRq.registeredphonenumber = oGetPartyTransactionDetailsRq.registeredphonenumber;
			if (oGetPartyTransactionDetailsRq.issaleconvert)
			{
				oGetTypeOfPayTransactionsRq.typeofpay = "SALE";
				oGetPartyTransactionDetailsRs.invoicenumbercount = saledl.GetInvoiceNumberCount(oGetTypeOfPayTransactionsRq);
			}
			else if (oGetPartyTransactionDetailsRq.issaleorderconvert)
			{
				oGetTypeOfPayTransactionsRq.typeofpay = "SALE ORDER";
				oGetPartyTransactionDetailsRs.invoicenumbercount = saledl.GetInvoiceNumberCount(oGetTypeOfPayTransactionsRq);
			}
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
		public async Task<GetLinkedPaymentTransactionRs> GetLinkedPaymentTransaction([FromQuery] Int64 registeredphonenumber, [FromQuery] string customername)
		{
			GetLinkedPaymentTransactionRs oGetLinkedPaymentTransactionRs = new GetLinkedPaymentTransactionRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetLinkedPaymentTransactionRs = saledl.GetLinkedPaymentTransaction(registeredphonenumber, customername);
			return oGetLinkedPaymentTransactionRs;
		}

		public async Task<GetTypeOfPayTransactionsRs> GetTypeOfPayTransactions(GetTypeOfPayTransactionsRq oGetTypeOfPayTransactionsRq)
		{
			GetTypeOfPayTransactionsRs oGetTypeOfPayTransactionsRs = new GetTypeOfPayTransactionsRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetTypeOfPayTransactionsRs = saledl.GetTypeOfPayTransactions(oGetTypeOfPayTransactionsRq);
			if (oGetTypeOfPayTransactionsRq.typeofpay == "SALE" || oGetTypeOfPayTransactionsRq.typeofpay == "ESTIMATION/ QUOTATION" || oGetTypeOfPayTransactionsRq.typeofpay == "PAYMENT IN" ||
				oGetTypeOfPayTransactionsRq.typeofpay == "SALE ORDER" || oGetTypeOfPayTransactionsRq.typeofpay == "DELIVERY CHALLAN" || oGetTypeOfPayTransactionsRq.typeofpay == "SALE RETURN/ CR. NOTE" || oGetTypeOfPayTransactionsRq.typeofpay == "PURCHASE ORDER")
			{
				oGetTypeOfPayTransactionsRs.invoicenumbercount = saledl.GetInvoiceNumberCount(oGetTypeOfPayTransactionsRq);
			}
			return oGetTypeOfPayTransactionsRs;
		}

		public async Task<GetPartyTransactionDetailsRs> ConvertToSaleSaleOrder(ConvertToSaleSaleOrderRq oConvertToSaleSaleOrderRq)
		{
			GetPartyTransactionDetailsRs oConvertToSaleSaleOrderRs = new GetPartyTransactionDetailsRs();
			GetTypeOfPayTransactionsRq oGetTypeOfPayTransactionsRq = new GetTypeOfPayTransactionsRq();
			oGetTypeOfPayTransactionsRq.typeofpay = oConvertToSaleSaleOrderRq.typeofpay;
			oGetTypeOfPayTransactionsRq.registeredphonenumber = oConvertToSaleSaleOrderRq.registeredphonenumber;
			SaleDL saledl = new SaleDL(this.config);
			if (oConvertToSaleSaleOrderRq.isconvert && (oConvertToSaleSaleOrderRq.typeofpay == "SALE" || oConvertToSaleSaleOrderRq.typeofpay == "SALE ORDER"))
			{
				oConvertToSaleSaleOrderRs = saledl.ConvertToSaleSaleOrder(oConvertToSaleSaleOrderRq);
				oConvertToSaleSaleOrderRs.invoicenumbercount = saledl.GetInvoiceNumberCount(oGetTypeOfPayTransactionsRq);
			}
			return oConvertToSaleSaleOrderRs;
		}

	}
}
