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
				result = saledl.FindOrInsertItem(otransactionRq);
				otransactionRs = saledl.SaveTransaction(otransactionRq);
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

	}
}
