using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using RestSharp;
using WebApplication1.BL;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : Controller
	{
		private readonly IConfiguration config;
		private readonly string dbConn;
		private readonly string _apiKey = "3cb8a053-8404-11ec-b9b5-0200cd936042";

		public LoginController(IConfiguration _config)
		{
			config = _config;
			dbConn = config.GetValue<string>("ConnectionStrings");
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("AuthenticateUser")]
		public async Task<ActionResult> AuthenticateUser(LoginRq ologinRq)
		{
			LoginRs ologinRs = new LoginRs();
			if (ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				ologinRs = await loginBL.ValidateLogin(ologinRq);
				return Ok(ologinRs);
			}
			else
			{
				return BadRequest("Please Provide Valid Details");
			}
		}

		[HttpGet("send-otp")]
		public async Task<IActionResult> SendOtp([FromQuery] string phoneNumber)
		{
			OtpRs otpRs = new OtpRs();
			LoginBL loginBL = new LoginBL(this.config);

			bool exist = await loginBL.ValidateOtpUser(phoneNumber);
			if (!exist)
			{
				otpRs.status = "FAILED";
				otpRs.statusmessage = "Kindly Register before Login";
				return Ok(otpRs);
			}

			string apiKey = "3cb8a053-8404-11ec-b9b5-0200cd936042";
			var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Get, $"https://2factor.in/API/V1/{apiKey}/SMS/+91{phoneNumber}/AUTOGEN/OTP FOR LOGIN VERIFICATION");
			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();
			return Ok(response.Content.ReadAsStringAsync());
		}

		[HttpPost("verify-otp")]
		public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
		{
			OtpRs otpRs = new OtpRs();
			LoginBL loginBL = new LoginBL(this.config);
			otpRs = await loginBL.VerifyOtpUser(request);
			if (otpRs.status == "FAILED")
			{
				otpRs.status = "FAILED";
				otpRs.statusmessage = "Kindly Register before Login";
				otpRs.accessToken = "";
				return Ok(otpRs);
			}
			var client = new RestClient();
			var url = $"https://2factor.in/API/V1/{_apiKey}/SMS/VERIFY/{request.SessionId}/{request.Otp}";
			var restRequest = new RestRequest(url, Method.Get);

			var response = await client.ExecuteAsync(restRequest);

			if (response.IsSuccessful)
			{
				return Ok(otpRs);
			}
			otpRs.status = "FAILED";
			otpRs.statusmessage = "Failed To Verify OTP";
			return Ok(otpRs);
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("RegisterUser")]
		public async Task<ActionResult> RegisterUser(RegisterRq oregisterRq)
		{
			RegisterRs oregisterRs = new RegisterRs();
			if(ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				oregisterRs = await loginBL.RegisterUser(oregisterRq);
				return Ok(oregisterRs);
			}
			return BadRequest("Please Provide Valid Details");
		}

		[AllowAnonymous]
		[HttpPost("create-order")]
		public IActionResult CreateOrder([FromBody] OrderRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			LoginBL loginBL = new LoginBL(this.config);
			if (request.PlanType == "free")
			{
				if (!User.Identity.IsAuthenticated)
				{
					return Unauthorized();
				}
				var val = loginBL.UpdateExpiryDate(DateTime.UtcNow.AddDays(7),request.registeredphonenumber, request.PlanType);
				if (!val)
				{
					return Ok("Something went wrong");
				}
				return Ok(new { status = "SUCCESS", message = "Free trial activated" });
			}

			int validatedAmount = loginBL.ValidatePlanAmount(request.PlanType);
			if (validatedAmount == -1)
			{
				return BadRequest("Invalid plan type.");
			}

			var order = loginBL.CreateOrder(validatedAmount, request.Currency);
			if (order == null || string.IsNullOrEmpty(order.Attributes["id"].ToString()) || order.Attributes["amount"] == null)
			{
				return StatusCode(500, "Failed to create Razorpay order.");
			}

			int amount;
			if (!int.TryParse(order.Attributes["amount"].ToString(), out amount))
			{
				return StatusCode(500, "Invalid amount in Razorpay order.");
			}

			return Ok(new { id = order.Attributes["id"].ToString(), amount = amount });
		}

		[HttpPost]
		[Route("UpdateExpiryDate")]
		public IActionResult UpdateExpiryDate([FromBody] UpdateExpDate request)
		{
			DateTime date = DateTime.UtcNow;
			string statusm = "SUCCESS";
			LoginBL loginBL = new LoginBL(this.config);
			if(request.PlanType == "silver")
			{
				date = DateTime.UtcNow.AddMonths(6);
			} 
			else if (request.PlanType == "gold")
			{
				 date = DateTime.UtcNow.AddYears(1);
			}
			var val = loginBL.UpdateExpiryDate(date, request.registeredphonenumber, request.PlanType);
			if (!val)
			{
				statusm = "FAILED";
			}
			return Ok( new { status = statusm, expiryDate = date });
		}

		[HttpPost]
		[Route("SaveOrUpdateParty")]
		public async Task<ActionResult> Party(PartyRq opartyRq)
		{
			PartyRs opartyRs = new PartyRs();
			if (ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				opartyRs = await loginBL.Party(opartyRq);
				return Ok(opartyRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[Authorize]
		[HttpGet]
		[Route("GetPartyDetails")]
		public async Task<ActionResult> GetPartyDetails([FromQuery] Int64 registeredphonenumber, [FromQuery] string partyname)
		{
			GetPartyRs ogetPartyRs = new GetPartyRs();
			if (ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				ogetPartyRs = await loginBL.GetPartyDetails(registeredphonenumber, partyname);
				return Ok(ogetPartyRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[Authorize]
		[HttpGet]
		[Route("GetPartyList")]
		public async Task<ActionResult> GetPartyList([FromQuery] Int64 registeredphonenumber)
		{
			GetPartyListRs oGetPartyListRs = new GetPartyListRs();
			if (ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				oGetPartyListRs = await loginBL.GetPartyList(registeredphonenumber);
				return Ok(oGetPartyListRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[Authorize]
		[HttpGet]
		[Route("GetPartyGroup")]
		public async Task<ActionResult> GetPartyGroup([FromQuery] Int64 registeredphonenumber)
		{
			GetPartyGroupRs oGetPartyGroupRs = new GetPartyGroupRs();
			if (ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				oGetPartyGroupRs = await loginBL.GetPartyGroup(registeredphonenumber);
				return Ok(oGetPartyGroupRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[Authorize]
		[HttpGet]
		[Route("GetPartyByGroup")]
		public async Task<ActionResult> GetPartyByGroup([FromQuery] Int64 registeredphonenumber, [FromQuery] string groupname)
		{
			GetPartyByGroupRs oGetPartyByGroupRs = new GetPartyByGroupRs();
			if (ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				oGetPartyByGroupRs = await loginBL.GetPartyByGroup(registeredphonenumber, groupname);
				return Ok(oGetPartyByGroupRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[Authorize]
		[HttpPost]
		[Route("AddUpdatePartyGroup")]
		public async Task<ActionResult> AddUpdatePartyGroup(AddUpdatePartyGropRq oAddUpdatePartyGropRq)
		{
			AddUpdatePartyGropRs oAddUpdatePartyGropRs = new AddUpdatePartyGropRs();
			if (ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				oAddUpdatePartyGropRs = await loginBL.AddUpdatePartyGroup(oAddUpdatePartyGropRq);
				return Ok(oAddUpdatePartyGropRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

        [Authorize]
        [HttpGet]
        [Route("GetBusinessInfo")]
        public async Task<ActionResult> GetBusinessInfo([FromQuery] Int64 registeredphonenumber)
        {
            GetBusinessInfoRs oGetBusinessInfoRs = new GetBusinessInfoRs();
            if (ModelState.IsValid)
            {
                LoginBL loginBL = new LoginBL(this.config);
                oGetBusinessInfoRs = await loginBL.GetBusinessInfo(registeredphonenumber);
                return Ok(oGetBusinessInfoRs);
            }

            return BadRequest("Please Provide Valid Details");
        }

        [Authorize]
		[HttpPost]
        [Route("AddUpdateBusinessInformation")]
        public async Task<ActionResult> AddUpdateBusinessInformation(AddBusinessInformationRq oAddBusinessInformationRq)
        {
            AddBusinessInformationRs oAddBusinessInformationRs = new AddBusinessInformationRs();
            if (ModelState.IsValid)
            {
                LoginBL loginBL = new LoginBL(this.config);
                oAddBusinessInformationRs = await loginBL.AddUpdateBusinessInformation(oAddBusinessInformationRq);
                return Ok(oAddBusinessInformationRs);
            }

            return BadRequest("Please Provide Valid Details");
        }

		[Authorize]
		[HttpGet]
		[Route("DashboardDetails")]
		public async Task<IActionResult> DashboardDetails([FromQuery] Int64 registeredphonenumber)
		{
			DashboardDetailsRs oDashboardDetailsRs = new DashboardDetailsRs();
			LoginBL loginBL = new LoginBL(this.config);
			if (ModelState.IsValid)
			{
				oDashboardDetailsRs = await loginBL.DashBoardDetails(registeredphonenumber);
				return Ok(oDashboardDetailsRs);
			}
			return BadRequest();
		}

		[Authorize]
		[HttpPost]
		[Route("DashboardSaleDetails")]
		public async Task<IActionResult> DashboardSaleDetails([FromBody] DashboardSaleDetailsRq oDashboardSaleDetailsRq)
		{
			DashboardSaleDetailsRs oDashboardSaleDetailsRs = new DashboardSaleDetailsRs();
			LoginBL loginBL = new LoginBL(this.config);
			if (ModelState.IsValid)
			{
				oDashboardSaleDetailsRs = await loginBL.DashboardSaleDetails(oDashboardSaleDetailsRq);
				return Ok(oDashboardSaleDetailsRs);
			}
			return BadRequest();
		}
	}
}
