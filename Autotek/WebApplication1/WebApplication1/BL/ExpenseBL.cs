using WebApplication1.DL;
using WebApplication1.Models;

namespace WebApplication1.BL
{
	public class ExpenseBL
	{
		private readonly IConfiguration config;
		private readonly string dbConn;
		public ExpenseBL(IConfiguration _config)
		{
			config = _config;
			dbConn = config.GetValue<string>("ConnectionStrings");
		}

		public async Task<ExpenseRs> AddOrUpdateExpense(ExpenseRq oExpenseRq)
		{
			ExpenseRs oExpenseRs = new ExpenseRs();
			if (string.IsNullOrWhiteSpace(oExpenseRq.category) || oExpenseRq.total <= 0)
			{
				oExpenseRs.status = "FAILED";
				oExpenseRs.statusmessage = "Please provide a category and an amount greater than 0.";
				return oExpenseRs;
			}

			ExpenseDL expenseDL = new ExpenseDL(this.config);
			SaleDL saleDL = new SaleDL(this.config);

			if (oExpenseRq.isexpenseupdate)
			{
				// Reverse the bank effect of the previous amountdetails before applying the new ones.
				List<AmountDetails> oldAmountDetails = expenseDL.GetExpenseAmountDetails(oExpenseRq.expense_id, oExpenseRq.registeredphonenumber);
				oExpenseRs = expenseDL.UpdateExpense(oExpenseRq);
				if (oExpenseRs.status == "SUCCESS")
				{
					await saleDL.UpdateBankAmountDetails(oExpenseRq.amountdetailslist ?? new List<AmountDetails>(), oExpenseRq.registeredphonenumber, oldAmountDetails, "EXPENSE");
				}
			}
			else
			{
				oExpenseRs = expenseDL.SaveExpense(oExpenseRq);
				if (oExpenseRs.status == "SUCCESS" && oExpenseRq.amountdetailslist != null && oExpenseRq.amountdetailslist.Count > 0)
				{
					await saleDL.UpdateBankAmount(oExpenseRq.amountdetailslist, oExpenseRq.registeredphonenumber, "EXPENSE");
				}
			}

			return oExpenseRs;
		}

		public async Task<GetExpenseListRs> GetExpenseList(Int64 registeredphonenumber)
		{
			ExpenseDL expenseDL = new ExpenseDL(this.config);
			return expenseDL.GetExpenseList(registeredphonenumber);
		}

		public async Task<DeleteExpenseRs> DeleteExpense(DeleteExpenseRq oDeleteExpenseRq)
		{
			ExpenseDL expenseDL = new ExpenseDL(this.config);
			SaleDL saleDL = new SaleDL(this.config);

			// Credit back whatever this expense had debited from cash/bank before hiding it.
			List<AmountDetails> oldAmountDetails = expenseDL.GetExpenseAmountDetails(oDeleteExpenseRq.expense_id, oDeleteExpenseRq.registeredphonenumber);
			DeleteExpenseRs oDeleteExpenseRs = expenseDL.HideExpense(oDeleteExpenseRq);
			if (oDeleteExpenseRs.status == "SUCCESS" && oldAmountDetails.Count > 0)
			{
				await saleDL.UpdateBankAmountDetails(new List<AmountDetails>(), oDeleteExpenseRq.registeredphonenumber, oldAmountDetails, "EXPENSE");
			}
			return oDeleteExpenseRs;
		}
	}
}
