using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace WebApplication1.Controllers
{
	public class BaseController : Controller
	{
		private readonly IConfiguration config;
		public BaseController(IConfiguration _config)
		{
			config = _config;
		}
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			bool route = context.RouteData.Values.Values.Contains("RegisterUser") || context.RouteData.Values.Values.Contains("AuthenticateUser");
			if(context.ModelState.IsValid && !route)
			{
				IHeaderDictionary headers = context.HttpContext.Request.Headers;
				if (headers.ContainsKey("Authorization"))
				{
					string token = headers["Authorization"];
					string jwtToken = token.Replace("Bearer ",  string.Empty);
					bool userValid = ValidateToken(jwtToken);
					if(!userValid)
					{
						context.Result = new UnauthorizedResult();
					}
				}
				else
				{
					context.Result = new UnauthorizedResult();
				}
			}
			else if (!route)
			{
				context.Result = new BadRequestObjectResult(context.ModelState);
			}
		}

		private bool ValidateToken(string inputToken)
		{
			try
			{
				string secretKey = config.GetValue<string>("JwtSettings:SecretKey");
				if (inputToken != null)
				{
					var tokenHandler = new JwtSecurityTokenHandler();
					var validationParameters = new TokenValidationParameters
					{
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateLifetime = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
					};

					try
					{
						SecurityToken validatedToken;
						tokenHandler.ValidateToken(inputToken, validationParameters, out validatedToken);
						return true;
					}
					catch (SecurityTokenExpiredException)
					{
						return false;
					}
					catch (Exception)
					{
						return false;
					}
				}
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
