using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace WebApplication1.Models
{
	public class LoginRq
	{
		[Key]
		[Required]
		public Int64 phonenumber { get; set; }
		public string password { get; set; }
	}

	public class OtpRs
	{
		public string status { get; set; }
		public string statusmessage { get; set; }
		public string accessToken { get; set; }
		public DateTime expiryDate { get; set; }
		public string plantype { get; set; }
	}

	public class LoginRs
	{
		public string status { get; set; }
		public string statusMessage { get; set; }
		public string accessToken { get; set; }
		public DateTime expiryDate { get; set; }
		public string plantype { get; set; }
	}

	public class RegisterRq
	{
		public Int64 phonenumber { get; set;}
		public string password { get; set; }
		public string state { get; set; }
		public string address { get; set; }
	}

	public class RegisterRs
	{
		public string status { get; set; }
		public string stat { get; set; }
		public Boolean exist { get; set; }
	}

	public class VerifyOtpRequest
	{
		public string SessionId { get; set; }
		public string Otp { get; set; }
		public string registeredphonenumber { get; set; }
	}

	public class OrderRequest
	{
		public int Amount { get; set; }
		public string Currency { get; set; }
		public string PlanType { get; set; }
		public Int64 registeredphonenumber { get; set; }
	}

	public class UpdateExpDate
	{
		public string PlanType { get; set; }
		public Int64 registeredphonenumber { get; set; }
	}

	public class PartyRq
	{
		public string typeofpay { get; set; } = "";
		public Int64? registeredphonenumber { get; set; }
		public string? partyname { get; set; }
		public string? oldpartyname { get; set; } = "";
		public string? GST { get; set; } = "";
		public Int64 phonenumber { get; set;}
		public string? partygroup { get; set; }
		public string? gsttype { get; set; }
		public string? _state { get; set;}
		public string? emailid { get; set; }
		public string? billingaddress { get; set; }
		public string? shippingaddress { get; set; }
		public Decimal openingbalance { get; set; }
		public string topayorreceive { get; set; }
		public DateTime asofdate { get; set;}
		public Decimal creditlimit { get; set; }
		public string? additionalfieldname1 { get; set; }
		public string? additionalfieldname2 { get; set; }
		public string? additionalfieldname3 { get; set;}
		public string? additionalfieldname4 { get; set; }
		public string? additionalfieldname1value { get; set; }
		public string? additionalfieldname2value { get; set; }
		public string? additionalfieldname3value { get; set; }
		public string? additionalfieldname4value { get; set; }
		public bool ispartyupdate { get; set; }
		public Decimal topayparty {  get; set; }
		public Decimal toreceivefromparty {  get; set; }
	}

	public class PartyRs
	{
		public string status { get; set; }
		public string statusmessage { get; set; }
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
		public Int64 phonenumber { get; set; }
		public string partygroup { get; set; }
		public string gsttype { get; set; }
		public string _state { get; set; }
		public string emailid { get; set; }
		public string billingaddress { get; set; }
		public string shippingaddress { get; set; }
		public Decimal openingbalance { get; set; }
		public DateTime asofdate { get; set; }
		public Decimal creditlimit { get; set; }
		public string additionalfieldname1 { get; set; }
		public string additionalfieldname2 { get; set; }
		public string additionalfieldname3 { get; set; }
		public string additionalfieldname4 { get; set; }
		public string additionalfieldname1value { get; set; }
		public string additionalfieldname2value { get; set; }
		public string additionalfieldname3value { get; set; }
		public string additionalfieldname4value { get; set; }
		public string typeofpay { get; set; }
		public string topayorreceive { get; set; }
		public Decimal topayparty { get; set; }
		public Decimal toreceivefromparty { get; set; }

	}

	public class GetPartyListRs
	{
		public string status {  get; set; }
		public GetPartyListRs()
		{
			getPartyList = new List<GetPartyList>();
		}
		public List<GetPartyList> getPartyList { get; set; }
	}

	public class GetPartyList
	{
		public string partyname { get; set; }
		public Int64 phonenumber { get; set; }
		public string billingaddress { get; set; }
		public string shippingaddress { get; set; }
		public Decimal creditlimit { get; set; }
		public Decimal topayparty { get; set; }
		public Decimal toreceivefromparty { get; set; }

	}

	public class GetPartyGroupRs
	{
		public string status { get; set; }
		public GetPartyGroupRs()
		{
			getPartyGroupList = new List<GetPartyGroupListtRs>();
		}
		public List<GetPartyGroupListtRs> getPartyGroupList { get; set; }
	}
	public class GetPartyGroupListtRs
	{
		public string partygroup { get; set; }
		public Int64 partygroupcount { get; set; }
	}

	public class GetPartyByGroupRs
	{
		public GetPartyByGroupRs()
		{
			getPartyList = new List<GetPartyList>();
		}
		public List<GetPartyList> getPartyList { get; set; }
	}

	public class AddUpdatePartyGropRq
	{
		public Int64 registeredphonenumber { get; set; }
		public string newgroupname {  get; set; }
		public string oldgroupname {  get; set; }
	}

	public class AddUpdatePartyGropRs
	{
		public string status {  get; set; }
		public string statusmessage {  get; set; }
	}

    public class GetBusinessInfoRs
    {
        public BusinessInfo businessInfo { get; set; }
        public string status { get; set; }
        public string statusmsg { get; set; }
    }

    public class BusinessInfo
    {
        public string businessName { get; set; }
        public string gstin { get; set; }
        public Int64 phoneNumber { get; set; }
        public string emailId { get; set; }
        public string businessAddress { get; set; }
        public string businessType { get; set; }
        public string businessCategory { get; set; }
        public Decimal pincode { get; set; }
        public string state { get; set; }
        public string businessDescription { get; set; }

    }

    public class AddBusinessInformationRq
    {
        public Int64 registeredphonenumber { get; set; }
        public string businessName { get; set; }
        public string gstin { get; set; }
        public Int64 phoneNumber { get; set; }
        public string emailId { get; set; }
        public string businessAddress { get; set; }
        public string businessType { get; set; }
        public string businessCategory { get; set; }
        public Decimal pincode { get; set; }
        public string state { get; set; }
        public string businessDescription { get; set; }
    }


    public class AddBusinessInformationRs
    {
        public string status { get; set; }
        public string statusmsg { get; set; }
        public Boolean exist { get; set; }

    }

}
