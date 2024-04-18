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

			return BadRequest("Please Provide Valid Details"); ;
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("GetPartyDetails")]
		public async Task<ActionResult> GetPartyDetails(GetPartyRq ogetPartyRq)
		{
			GetPartyRs ogetPartyRs = new GetPartyRs();
			if (ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				ogetPartyRs = await loginBL.GetPartyDetails(ogetPartyRq);
				return Ok(ogetPartyRs);
			}

			return BadRequest("Please Provide Valid Details"); ;
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("GetPartyList")]
		public async Task<ActionResult> GetPartyList(GetPartyListRq oGetPartyListRq)
		{
			GetPartyListRs oGetPartyListRs = new GetPartyListRs();
			if (ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				oGetPartyListRs = await loginBL.GetPartyList(oGetPartyListRq);
				return Ok(oGetPartyListRs);
			}

			return BadRequest("Please Provide Valid Details"); ;
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("GetPartyGroup")]
		public async Task<ActionResult> GetPartyGrop(GetPartyGroupRq oGetPartyGroupRq)
		{
			GetPartyGroupRs oGetPartyGroupRs = new GetPartyGroupRs();
			if (ModelState.IsValid)
			{
				LoginBL loginBL = new LoginBL(this.config);
				oGetPartyGroupRs = await loginBL.GetPartyGrop(oGetPartyGroupRq);
				return Ok(oGetPartyGroupRs);
			}

			return BadRequest("Please Provide Valid Details"); ;
		}
	}
}
