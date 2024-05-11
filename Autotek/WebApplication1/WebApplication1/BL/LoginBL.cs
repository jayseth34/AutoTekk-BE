using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.DL;
using WebApplication1.Models;

namespace WebApplication1.BL
{
	public class LoginBL
	{
		private readonly IConfiguration config;
		private readonly string dbConn;
		public LoginBL(IConfiguration _config)
		{
			config = _config;
			dbConn = config.GetValue<string>("ConnectionStrings");
		}
		public async Task<LoginRs> ValidateLogin(LoginRq ologinRq)
		{
			LoginRs ologinRs = new LoginRs();
			LoginDL logindl = new LoginDL(this.config);
			ologinRs = logindl.GetLoginDetails(ologinRq);
			if(ologinRs.status == "SUCCESS")
			{
				string _token = await CreateToken();
				ologinRs.accessToken = _token;
			}
			return ologinRs;
		}

		public async Task<string> CreateToken()
		{
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, config["JwtSettings:Subject"]),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};
			var secretKey = config["JwtSettings:SecretKey"];
			if (secretKey.Length < 32)
			{
				// Pad the key with zeroes to ensure it meets the required size
				secretKey = secretKey.PadRight(32, '\0');
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
			var signature = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			// Generate token
			var token = new JwtSecurityToken(
				config["JwtSettings:Issuer"],
				config["JwtSettings:Audience"],
				claims,
				expires: DateTime.UtcNow.AddHours(12), // Expiry set to 12 hours from now
				signingCredentials: signature);

			string _token = new JwtSecurityTokenHandler().WriteToken(token);
			return _token;
		}

		public async Task<RegisterRs> RegisterUser(RegisterRq oregisterRq)
		{
			RegisterRs oregisterRs = new RegisterRs();
			LoginDL logindl = new LoginDL(this.config);
			oregisterRs = logindl.RegisterUser(oregisterRq);
			return oregisterRs;
		}

		public async Task<PartyRs> Party(PartyRq opartyRq)
		{
			PartyRs opartyRs = new PartyRs();
			LoginDL logindl = new LoginDL(this.config);
			if (opartyRq.ispartyupdate)
			{
				opartyRs = logindl.UpdateParty(opartyRq);
			} 
			else
			{
				opartyRs = logindl.Party(opartyRq);
			}
			return opartyRs;
		}

		public async Task<GetPartyRs> GetPartyDetails(Int64 registeredphonenumber, string partyname)
		{
			GetPartyRs ogetPartyRs = new GetPartyRs();
			LoginDL logindl = new LoginDL(this.config);
			ogetPartyRs = logindl.GetPartyDetails(registeredphonenumber, partyname);
			return ogetPartyRs;
		}

		public async Task<GetPartyListRs> GetPartyList(Int64 registeredphonenumber)
		{
			GetPartyListRs oGetPartyListRs = new GetPartyListRs();
			LoginDL logindl = new LoginDL(this.config);
			oGetPartyListRs = logindl.GetPartyList(registeredphonenumber);
			return oGetPartyListRs;
		}
		public async Task<GetPartyGroupRs> GetPartyGroup(Int64 registeredphonenumber)
		{
			GetPartyGroupRs oGetPartyGroupRs = new GetPartyGroupRs();
			LoginDL logindl = new LoginDL(this.config);
			oGetPartyGroupRs = logindl.GetPartyGroup(registeredphonenumber);
			return oGetPartyGroupRs;
		}

		public async Task<GetPartyByGroupRs> GetPartyByGroup(Int64 registeredphonenumber, string groupname)
		{
			GetPartyByGroupRs oGetPartyByGroupRs = new GetPartyByGroupRs();
			LoginDL logindl = new LoginDL(this.config);
			oGetPartyByGroupRs = logindl.GetPartyByGroup(registeredphonenumber, groupname);
			return oGetPartyByGroupRs;
		}

		public async Task<AddUpdatePartyGropRs> AddUpdatePartyGroup(AddUpdatePartyGropRq oAddUpdatePartyGropRq)
		{
			AddUpdatePartyGropRs oAddUpdatePartyGropRs = new AddUpdatePartyGropRs();
			LoginDL logindl = new LoginDL(this.config);
			oAddUpdatePartyGropRs = logindl.AddUpdatePartyGroup(oAddUpdatePartyGropRq);
			return oAddUpdatePartyGropRs;
		}
	}
}
