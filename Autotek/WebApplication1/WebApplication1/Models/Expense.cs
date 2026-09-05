using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
	public class ExpenseRq
	{
		public Int64 expense_id { get; set; }
		public Int64 registeredphonenumber { get; set; }
		public DateTime expensedate { get; set; }
		public string category { get; set; } = "";
		public string partyname { get; set; } = "";
		public string description { get; set; } = "";
		public Decimal total { get; set; } = 0;
		public string paymenttype { get; set; } = "";
		public bool isexpenseupdate { get; set; }
		public ExpenseRq()
		{
			amountdetailslist = new List<AmountDetails>();
		}
		public List<AmountDetails>? amountdetailslist { get; set; }
	}

	public class ExpenseRs
	{
		public string status { get; set; }
		public string statusmessage { get; set; }
		public Int64 expense_id { get; set; }
	}

	public class GetExpenseListRs
	{
		public GetExpenseListRs()
		{
			expenselist = new List<ExpenseListItem>();
		}
		public List<ExpenseListItem> expenselist { get; set; }
		public string status { get; set; }
		public string statusmessage { get; set; }
	}

	public class ExpenseListItem
	{
		public Int64 expense_id { get; set; }
		public DateTime expensedate { get; set; }
		public string category { get; set; }
		public string partyname { get; set; }
		public string description { get; set; }
		public Decimal total { get; set; }
		public string paymenttype { get; set; }
		public List<AmountDetails> amountdetailslist { get; set; }
	}

	public class DeleteExpenseRq
	{
		public Int64 expense_id { get; set; }
		public Int64 registeredphonenumber { get; set; }
	}

	public class DeleteExpenseRs
	{
		public string status { get; set; }
		public string statusmessage { get; set; }
	}
}
