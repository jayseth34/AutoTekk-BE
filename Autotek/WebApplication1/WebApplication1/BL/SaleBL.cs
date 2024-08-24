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
			string amtdeatils = string.Empty;
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
				amtdeatils = await saledl.UpdateBankAmount(otransactionRq.amountdetailslist, otransactionRq.registeredphonenumber);
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
				typeofpay == "SALE ORDER" || typeofpay == "DELIVERY CHALLAN" || typeofpay == "SALE RETURN" || typeofpay == "PURCHASE ORDER" || typeofpay == "PURCHASE")
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
			string amtdeatils = string.Empty;
			otransactionRs.status = saledl.UpdateTransactionDetails(otransactionRq);
			if (otransactionRs.status == "SUCCESS")
			{
				_ = saledl.FindOrInsertItem(otransactionRq);
				otransactionRs.status = saledl.UpdateInsertDeleteItemDetails(otransactionRq);
				amtdeatils = await saledl.UpdateBankAmount(otransactionRq.amountdetailslist, otransactionRq.registeredphonenumber);
			}

			return otransactionRs;
		}

		public async Task<bool> UpdateLinkedPaymentTransaction(List<GetLinkedPaymentTransactionList> transactions)
		{
			SaleDL saledl = new SaleDL(this.config);
			bool val = await saledl.UpdateLinkedPaymentTransaction(transactions);
			if (val)
			{
				bool value = await saledl.InsertPaymentInOutTrnx(transactions);
			}
			return val;
		}

		public async Task<UpadatePaymentInOutTrnxRs> UpdatePaymentInOutTrnx(UpadatePaymentInOutTrnxRq oUpadatePaymentInOutTrnxRq)
		{
			UpadatePaymentInOutTrnxRs oUpadatePaymentInOutTrnxRs = new UpadatePaymentInOutTrnxRs();
			SaleDL saledl = new SaleDL(this.config);
			oUpadatePaymentInOutTrnxRs = await saledl.UpdatePaymentInOutTrnx(oUpadatePaymentInOutTrnxRq);
			return oUpadatePaymentInOutTrnxRs;
		}

		public async Task<GetUpdatedTrnxInOutValRs> GetUpdatedTrnxInOutVal(GetUpdatedTrnxInOutValRq oGetUpdatedTrnxInOutValRq)
		{
			GetUpdatedTrnxInOutValRs oGetUpdatedTrnxInOutValRs = new GetUpdatedTrnxInOutValRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetUpdatedTrnxInOutValRs = await saledl.GetUpdatedTrnxInOutVal(oGetUpdatedTrnxInOutValRq);
			return oGetUpdatedTrnxInOutValRs;
		}

		public async Task<BankFormRs> SaveBankDetails(BankFormRq oBankFormRq)
		{
			BankFormRs oBankFormRs = new BankFormRs();
			SaleDL saledl = new SaleDL(this.config);
			bool inserttransaction = false;

			bool bankExists = await saledl.IfBankExists(oBankFormRq);
			if(bankExists && oBankFormRq.isbanksupdate && (oBankFormRq.newaccountdisplayname == oBankFormRq.oldaccountdisplayname))
			{
				bankExists = false;
			}
			if (bankExists && !oBankFormRq.isbanksupdate)
			{
				oBankFormRs.statusmessage = "Account Display Name already exists.";
				oBankFormRs.status = "FAILED";
			}
			else if (bankExists && oBankFormRq.isbanksupdate)
			{
				oBankFormRs.statusmessage = "Account Display Name already exists.";
				oBankFormRs.status = "FAILED";
			}
			else if (!bankExists && oBankFormRq.isbanksupdate)
			{
				oBankFormRs = await saledl.UpdateBankDetails(oBankFormRq);
				inserttransaction = await saledl.UpdatetTrnx(oBankFormRq);

			}
			else if (!bankExists && !oBankFormRq.isbanksupdate)
			{
				oBankFormRs = await saledl.SaveBankDetails(oBankFormRq);
				inserttransaction = await saledl.InsertTrnx(oBankFormRq);
			}

			return oBankFormRs;
		}

		public async Task<GetBankDetailsRs> GetBankDetails(GetBankDetailsRq oGetBankDetailsRq)
		{
			GetBankDetailsRs oGetBankDetailsRs = new GetBankDetailsRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetBankDetailsRs = await saledl.GetBankDetailsAsync(oGetBankDetailsRq);
			return oGetBankDetailsRs;
		}

		public async Task<GetBanksRs> GetBanks(GetBanksRq oGetBanksRq)
		{
			GetBanksRs oGetBanksRs = new GetBanksRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetBanksRs = await saledl.GetBanks(oGetBanksRq);
			return oGetBanksRs;
		}

		public async Task<GetBanksDetailsValuesRs> GetBanksDetailsValues(GetBanksDetailsValuesRq oGetBanksDetailsValuesRq)
		{
			GetBanksDetailsValuesRs oGetBanksDetailsValuesRs = new GetBanksDetailsValuesRs();
			SaleDL saledl = new SaleDL(this.config);
			oGetBanksDetailsValuesRs = await saledl.GetBanksDetailsValues(oGetBanksDetailsValuesRq);
			return oGetBanksDetailsValuesRs;
		}

		public async Task<TransfersRs> Transfers(TransfersRq oTransfersRq)
		{
			TransfersRs oTransfersRs = new TransfersRs();
			SaleDL saledl = new SaleDL(this.config);
			string sqlquery = string.Empty;
			string sqlquery1 = string.Empty;
			List<AmountDetails> amounts = new List<AmountDetails>();
			if(oTransfersRq.type == "bankToCash")
			{
				sqlquery = "UPDATE BankForm set amount = amount - @amount where accountdisplayname = @accountdisplayname and registeredphonenumber = @registeredphonenumber";
				oTransfersRs = await saledl.Transfers(oTransfersRq,sqlquery,1);
				sqlquery1 = "INSERT INTO transactions (typeofpay, customername, invoicedate, registeredphonenumber, showtransaction, total, paymenttype, amountdetails) " +
							  "VALUES ('CASH WITHDRAW', '', @invoicedate, @registeredphonenumber, 'DONT SHOW', @amount, @paymenttype, @amountdetails)";
				AmountDetails amountDetails = new AmountDetails();
				amountDetails.type = oTransfersRq.fromAccount;
				amountDetails.amount = oTransfersRq.amount;
				amounts.Add(amountDetails);
				oTransfersRs = await saledl.InsertTransfers(oTransfersRq, sqlquery1,amounts, 1);
			}
			else if(oTransfersRq.type == "cashToBank")
			{
				sqlquery = "UPDATE BankForm set amount = amount + @amount where accountdisplayname = @accountdisplayname and registeredphonenumber = @registeredphonenumber";
				oTransfersRs = await saledl.Transfers(oTransfersRq, sqlquery, 1);
				sqlquery1 = "INSERT INTO transactions (typeofpay, customername, invoicedate, registeredphonenumber, showtransaction, total, paymenttype, amountdetails) " +
							  "VALUES ('CASH DEPOSIT', '', @invoicedate, @registeredphonenumber, 'DONT SHOW', @amount, @paymenttype, @amountdetails)";
				AmountDetails amountDetails = new AmountDetails();
				amountDetails.type = oTransfersRq.toAccount;
				amountDetails.amount = oTransfersRq.amount;
				amounts.Add(amountDetails);
				oTransfersRs = await saledl.InsertTransfers(oTransfersRq, sqlquery1, amounts, 1);
			}
			else if(oTransfersRq.type == "bankToBank")
			{
				sqlquery = "UPDATE BankForm set amount = amount - @amount where accountdisplayname = @accountdisplayname and registeredphonenumber = @registeredphonenumber";
				oTransfersRs = await saledl.Transfers(oTransfersRq, sqlquery, 1);
				sqlquery = "UPDATE BankForm set amount = amount + @amount where accountdisplayname = @accountdisplayname and registeredphonenumber = @registeredphonenumber";
				oTransfersRs = await saledl.Transfers(oTransfersRq, sqlquery, 2);
				sqlquery1 = "INSERT INTO transactions (typeofpay, customername, invoicedate, registeredphonenumber, showtransaction, total, paymenttype, amountdetails) " +
							  "VALUES ('BANK TO BANK', @customername, @invoicedate, @registeredphonenumber, 'DONT SHOW', @amount, @paymenttype, @amountdetails)";
				AmountDetails amountDetails = new AmountDetails();
				amountDetails.type = oTransfersRq.fromAccount;
				amountDetails.amount = oTransfersRq.amount;
				amounts.Add(amountDetails);
				oTransfersRs = await saledl.InsertTransfers(oTransfersRq, sqlquery1, amounts, 1);
				List<AmountDetails> amounts1 = new List<AmountDetails>();
				AmountDetails amountDetails1 = new AmountDetails();
				amountDetails1.type = oTransfersRq.toAccount;
				amountDetails1.amount = oTransfersRq.amount;
				amounts1.Add(amountDetails1);
				oTransfersRs = await saledl.InsertTransfers(oTransfersRq, sqlquery1, amounts1, 2);
			}
			else if (oTransfersRq.type == "adjustBalance")
			{
				if(oTransfersRq.adjustmentType == "decrease")
				{
					sqlquery = "UPDATE BankForm set amount = amount - @amount where accountdisplayname = @accountdisplayname and registeredphonenumber = @registeredphonenumber";
				}
				else if (oTransfersRq.adjustmentType == "increase")
				{
					sqlquery = "UPDATE BankForm set amount = amount + @amount where accountdisplayname = @accountdisplayname and registeredphonenumber = @registeredphonenumber";
				}
				oTransfersRs = await saledl.Transfers(oTransfersRq, sqlquery, 1);
				if (oTransfersRq.adjustmentType == "decrease")
				{
					sqlquery1 = "INSERT INTO transactions (typeofpay, customername, invoicedate, registeredphonenumber, showtransaction, total, paymenttype, amountdetails) " +
							  "VALUES ('BANK ADJ DECREASE', '', @invoicedate, @registeredphonenumber, 'DONT SHOW', @amount, @paymenttype, @amountdetails)";
				}
				else if (oTransfersRq.adjustmentType == "increase")
				{
					sqlquery1 = "INSERT INTO transactions (typeofpay, customername, invoicedate, registeredphonenumber, showtransaction, total, paymenttype, amountdetails) " +
							  "VALUES ('BANK ADJ INCREASE', '', @invoicedate, @registeredphonenumber, 'DONT SHOW', @amount, @paymenttype, @amountdetails)";
				}
				AmountDetails amountDetails = new AmountDetails();
				amountDetails.type = oTransfersRq.accountName;
				amountDetails.amount = oTransfersRq.amount;
				amounts.Add(amountDetails);
				oTransfersRs = await saledl.InsertTransfers(oTransfersRq, sqlquery1, amounts, 2);
			}
			return oTransfersRs;
		}
	}
}
