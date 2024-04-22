using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

		[AllowAnonymous]
		[HttpGet]
		[Route("GetPartyDetails")]
		public async Task<ActionResult> GetPartyDetails([FromQuery] Int64 registeredPhoneNumber, [FromQuery] string partyName)
		{
			GetPartyRs ogetPartyRs = new GetPartyRs();
			if (ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				ogetPartyRs = await loginBL.GetPartyDetails(registeredPhoneNumber, partyName);
				return Ok(ogetPartyRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("GetPartyList")]
		public async Task<ActionResult> GetPartyList([FromQuery] Int64 registeredPhoneNumber)
		{
			GetPartyListRs oGetPartyListRs = new GetPartyListRs();
			if (ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				oGetPartyListRs = await loginBL.GetPartyList(registeredPhoneNumber);
				return Ok(oGetPartyListRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("GetPartyGroup")]
		public async Task<ActionResult> GetPartyGroup([FromQuery] Int64 registeredPhoneNumber)
		{
			GetPartyGroupRs oGetPartyGroupRs = new GetPartyGroupRs();
			if (ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				oGetPartyGroupRs = await loginBL.GetPartyGroup(registeredPhoneNumber);
				return Ok(oGetPartyGroupRs);
			}

			return BadRequest("Please Provide Valid Details");
		}

		[AllowAnonymous]
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

		[AllowAnonymous]
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
