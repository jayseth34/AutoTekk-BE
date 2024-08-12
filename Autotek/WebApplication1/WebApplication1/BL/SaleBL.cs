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
			bool isconverted = false;
			if (otransactionRq.itemdetailslist == null || otransactionRq.itemdetailslist.Count == 0)
			{
				otransactionRs.status = "Kindly Insert Items.";
			}
			else
			{
				if ((otransactionRq.issaleconvert || otransactionRq.issaleorderconvert || otransactionRq.ispurchaseconvert) && otransactionRq.isupdate)
				{
					string typeofpay = string.Empty;
					isconverted = true;
					if (otransactionRq.issaleconvert)
					{
						typeofpay = "SALE";
					}
					else if (otransactionRq.issaleorderconvert)
					{
						typeofpay = "SALE ORDER";
					}
					else if (otransactionRq.ispurchaseconvert)
					{
						typeofpay = "PURCHASE";
					}

					result = saledl.FindOrInsertItem(otransactionRq);
					Int64 invoicecount = saledl.GetInvoiceNumberCountDLChallan(otransactionRq.registeredphonenumber, typeofpay);
					otransactionRs = saledl.SaveDeliveryChallan(otransactionRq, invoicecount, typeofpay, isconverted);
					saledl.UpdateDlChallan(otransactionRq.convertinvoicenumber, otransactionRq.registeredphonenumber,otransactionRq.typeofpay);
				}
				else
				{
					
					otransactionRs = saledl.SaveTransaction(otransactionRq);
					if(otransactionRq.typeofpay == "SALE ORDER" || otransactionRq.typeofpay == "PURCHASE ORDER" || otransactionRq.typeofpay == "DELIVERY CHALLAN" || otransactionRq.typeofpay == "ESTIMATE QUOTATION")
					{
						foreach(var i in otransactionRq.itemdetailslist)
						i.qty = 0;
					}
					result = saledl.FindOrInsertItem(otransactionRq);
				}
			}
			return otransactionRs;
		}

		public async Task<GetPartyTransactionsRs> GetPartyTransactions(Int64 registeredphonenumber, string customername)
		{
			GetPartyTransactionsRs oGetPartyTransactionsRqs = new GetPartyTransactionsRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetPartyTransactionsRqs = saledl.GetPartyTransactions(registeredphonenumber, customername);
			return oGetPartyTransactionsRqs;
		}

		public async Task<GetPartyTransactionDetailsRs> GetPartyTransactionDetails(GetPartyTransactionDetailsRq oGetPartyTransactionDetailsRq)
		{
			SaleDL saledl = new SaleDL(this.config);
			GetPartyTransactionDetailsRs oGetPartyTransactionDetailsRs = new GetPartyTransactionDetailsRs();

			oGetPartyTransactionDetailsRs = saledl.GetPartyTransactionDetails(oGetPartyTransactionDetailsRq);
			oGetPartyTransactionDetailsRs.invoicenumbercount = oGetPartyTransactionDetailsRq.invoicenumber;
			GetTypeOfPayTransactionsRq oGetTypeOfPayTransactionsRq = new GetTypeOfPayTransactionsRq();
			GetPartyAmounts oGetPartyAmounts = new GetPartyAmounts();
			

			oGetTypeOfPayTransactionsRq.registeredphonenumber = oGetPartyTransactionDetailsRq.registeredphonenumber;
			oGetPartyAmounts = await saledl.GetTopaypartyreceiveparty(oGetTypeOfPayTransactionsRq.registeredphonenumber, oGetPartyTransactionDetailsRs.customername);
			oGetPartyTransactionDetailsRs.topayparty = oGetPartyAmounts.topayparty;
			oGetPartyTransactionDetailsRs.toreceivefromparty = oGetPartyAmounts.toreceivefromparty;

			if (oGetPartyTransactionDetailsRq.issaleconvert)
			{
				oGetTypeOfPayTransactionsRq.typeofpay = "SALE";
				oGetPartyTransactionDetailsRs.invoicenumbercount = saledl.GetInvoiceNumberCount(oGetTypeOfPayTransactionsRq.registeredphonenumber, oGetTypeOfPayTransactionsRq.typeofpay);
			}
			else if (oGetPartyTransactionDetailsRq.issaleorderconvert)
			{
				oGetTypeOfPayTransactionsRq.typeofpay = "SALE ORDER";
				oGetPartyTransactionDetailsRs.invoicenumbercount = saledl.GetInvoiceNumberCount(oGetTypeOfPayTransactionsRq.registeredphonenumber, oGetTypeOfPayTransactionsRq.typeofpay);
			}
			return oGetPartyTransactionDetailsRs;
		}

		public async Task<PaymentInOutTrnxRs> GetPaymentInOutTransactionDetails(GetPartyTransactionDetailsRq oGetPartyTransactionDetailsRq)
		{
			PaymentInOutTrnxRs oPaymentInOutTrnxRs = new PaymentInOutTrnxRs();
			SaleDL saledl = new SaleDL(this.config);
			oPaymentInOutTrnxRs = saledl.GetPaymentInOutTransactionDetails(oGetPartyTransactionDetailsRq);
			return oPaymentInOutTrnxRs;
		}
		public async Task<GetItemTransactionsRs> GetItemTransactions(Int64 registeredphonenumber, string itemname)
		{
			GetItemTransactionsRs oGetItemTransactionsRs = new GetItemTransactionsRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetItemTransactionsRs.itemTransactionsList = saledl.GetItemTransactions(registeredphonenumber,itemname);
			oGetItemTransactionsRs = saledl.GetItemHeaderDetails(registeredphonenumber,itemname, oGetItemTransactionsRs.itemTransactionsList);
			return oGetItemTransactionsRs;
		}
		public async Task<GetLinkedPaymentTransactionRs> GetLinkedPaymentTransaction([FromQuery] Int64 registeredphonenumber, [FromQuery] string customername)
		{
			GetLinkedPaymentTransactionRs oGetLinkedPaymentTransactionRs = new GetLinkedPaymentTransactionRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetLinkedPaymentTransactionRs = saledl.GetLinkedPaymentTransaction(registeredphonenumber, customername);
			return oGetLinkedPaymentTransactionRs;
		}

		public async Task<GetTypeOfPayTransactionsRs> GetTypeOfPayTransactions(Int64 registeredphonenumber, string typeofpay)
		{
			GetTypeOfPayTransactionsRs oGetTypeOfPayTransactionsRs = new GetTypeOfPayTransactionsRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetTypeOfPayTransactionsRs = saledl.GetTypeOfPayTransactions(registeredphonenumber, typeofpay);
			if (typeofpay == "SALE" || typeofpay == "ESTIMATE QUOTATION" || typeofpay == "PAYMENT IN" ||
				typeofpay == "SALE ORDER" || typeofpay == "DELIVERY CHALLAN" || typeofpay == "SALE RETURN" || typeofpay == "PURCHASE ORDER")
			{
				oGetTypeOfPayTransactionsRs.invoicenumbercount = saledl.GetInvoiceNumberCount(registeredphonenumber, typeofpay);
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
				oConvertToSaleSaleOrderRs.invoicenumbercount = saledl.GetInvoiceNumberCount(oGetTypeOfPayTransactionsRq.registeredphonenumber, oGetTypeOfPayTransactionsRq.typeofpay);
			}
			return oConvertToSaleSaleOrderRs;
		}

		public async Task<TransactionRs> UpdateSale(TransactionRq otransactionRq)
		{
			TransactionRs otransactionRs = new TransactionRs();
			SaleDL saledl = new SaleDL(this.config);
			otransactionRs.status = saledl.UpdateTransactionDetails(otransactionRq);
			if (otransactionRs.status == "SUCCESS")
			{
				_ = saledl.FindOrInsertItem(otransactionRq);
				otransactionRs.status = saledl.UpdateInsertDeleteItemDetails(otransactionRq);
			}

			return otransactionRs;
		}

		public async Task<bool> UpdateLinkedPaymentTransaction(List<GetLinkedPaymentTransactionList> transactions)
		{
			SaleDL saledl = new SaleDL(this.config);
			bool val = await saledl.UpdateLinkedPaymentTransaction(transactions);
			return val;
		}

		public async Task<UpadatePaymentInOutTrnxRs> UpdatePaymentInOutTrnx(UpadatePaymentInOutTrnxRq oUpadatePaymentInOutTrnxRq)
		{
			UpadatePaymentInOutTrnxRs oUpadatePaymentInOutTrnxRs = new UpadatePaymentInOutTrnxRs();
			SaleDL saledl = new SaleDL(this.config);
			oUpadatePaymentInOutTrnxRs = await saledl.UpdatePaymentInOutTrnx(oUpadatePaymentInOutTrnxRq);
			return oUpadatePaymentInOutTrnxRs;
		}

	}
}
