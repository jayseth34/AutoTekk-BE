using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
		private readonly string _apiKey = "7fccc705-21c6-11ef-8b60-0200cd936042";

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

		[HttpPost("send-otp")]
		public async Task<IActionResult> SendOtp([FromQuery] string phoneNumber)
		{
			var client = new RestClient();
			var url = $"https://2factor.in/API/V1/{_apiKey}/SMS/{phoneNumber}/AUTOGEN";
			var restRequest = new RestRequest(url, Method.Get);

			var response = await client.ExecuteAsync(restRequest);

			if (response.IsSuccessful)
			{
				return Ok(response.Content); // Return the response from 2Factor
			}

			return BadRequest("Failed to send OTP");
		}

		[HttpPost("verify-otp")]
		public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
		{
			var client = new RestClient();
			var url = $"https://2factor.in/API/V1/{_apiKey}/SMS/VERIFY/{request.SessionId}/{request.Otp}";
			var restRequest = new RestRequest(url, Method.Get);

			var response = await client.ExecuteAsync(restRequest);

			if (response.IsSuccessful)
			{
				return Ok(response.Content); // Return the response from 2Factor
			}

			return BadRequest("Failed to verify OTP");
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
	}
}
