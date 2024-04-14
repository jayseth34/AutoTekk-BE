using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace WebApplication1.Models
{
	public class LoginRq
	{
		[Key]
		[Required]
		public Int64 phoneNumber { get; set; }
		public string password { get; set; }
	}

	public class LoginRs
	{
		public string status { get; set; }
		public string statusMessage { get; set; }
		public string accessToken { get; set; }
		public DateTime expiryDate { get; set; }
	}

	public class RegisterRq
	{
		public Int64 phoneNumber { get; set;}
		public string password { get; set; }
		public string state { get; set; }
		public string address { get; set; }
	}

	public class RegisterRs
	{
		public string status { get; set; }
		public Boolean exist { get; set; }
	}

	public class PartyRq
	{
		public string typeofpay { get; set; }
		public Int64? registeredPhoneNumber { get; set; }
		public string? partyName { get; set; }
		public string? GST { get; set; } = "";
		public Int64 phoneNumber { get; set;}
		public string? partyGroup { get; set; }
		public string? gstType { get; set; }
		public string? _state { get; set;}
		public string? emailId { get; set; }
		public string? billingAddress { get; set; }
		public string? shippingAddress { get; set; }
		public Int64 openingBalance { get; set; }
		public string toPayOrReceive { get; set; }
		public DateTime asOfDate { get; set;}
		public Int64 creditLimit { get; set; }
		public string? additionalFieldName1 { get; set; }
		public string? additionalFieldName2 { get; set; }
		public string? additionalFieldName3 { get; set;}
		public string? additionalFieldName4 { get; set; }
		public Int64 partybalance { get; set; }
	}

	public class PartyRs
	{
		public string status { get; set; }
	}

	public class GetPartyRq
	{
		public Int64 registeredPhoneNumber { get; set; }
		public string partyName { get; set;}
	}

	public class GetPartyRs
	{
		public GetPartyRs()
		{
			partyList = new List<GetAllPartyList>();
		}
		public List<GetAllPartyList> partyList { get; set; }
		public string status { get; set; }
		public string statusMessage { get; set; }
		
	}

	public class GetAllPartyList
	{
		public string GST { get; set; }
		public Int64 phoneNumber { get; set; }
		public string partyGroup { get; set; }
		public string gstType { get; set; }
		public string _state { get; set; }
		public string emailId { get; set; }
		public string billingAddress { get; set; }
		public string shippingAddress { get; set; }
		public Int64 openingBalance { get; set; }
		public DateTime asOfDate { get; set; }
		public Int64 creditLimit { get; set; }
		public string additionalFieldName1 { get; set; }
		public string additionalFieldName2 { get; set; }
		public string additionalFieldName3 { get; set; }
		public string additionalFieldName4 { get; set; }
		public string typeofpay { get; set; }
		public string toPayOrReceive { get; set; }
		public Int64 partybalance { get; set; }
	}

	public class GetPartyListRq
	{
		public Int64 registeredPhoneNumber { get; set; }
	}

	public class GetPartyListRs
	{
		public GetPartyListRs()
		{
			getPartyList = new List<GetPartyList>();
		}
		public List<GetPartyList> getPartyList { get; set; }
	}

	public class GetPartyList
	{
		public string partyname { get; set; }
		public Int64 partybalance { get; set; }
	}
}
