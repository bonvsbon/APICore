using System.Globalization;
using System.Net;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityManager.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Owin;

namespace APICore.Common {
	public class TokenGenerator {
		public static string GenerateToken(string userId) {
			var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
			var mySecurityKey = new SymmetricSecurityKey (Encoding.ASCII.GetBytes(mySecret));

			var myIssuer = "_Token";
			var myAudience = "_Token";

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor {
				Subject = new ClaimsIdentity (new Claim[] {
						new Claim (ClaimTypes.NameIdentifier, userId.ToString ()),
					}),
					// Expires = DateTime.UtcNow.AddMinutes(5),
					Expires = DateTime.UtcNow.AddDays(1),
					Issuer = myIssuer, 
					Audience = myAudience,
					SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
		public static string ValidateJwtToken(string token) {
			var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(mySecret);
			try {
				tokenHandler.ValidateToken(token, new TokenValidationParameters {
					ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(key),
						ValidateIssuer = false,
						ValidateAudience = false,
						// set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
						ClockSkew = TimeSpan.Zero
				}, out SecurityToken validatedToken);

				var jwtToken = (JwtSecurityToken)validatedToken;
				var accountId = jwtToken.Claims.First (x => x.Type == "nameid").Value.ToString();

				// return account id from JWT token if validation successful
				return accountId;
			} catch (Exception e){
				// return null if validation fails
				return HttpStatusCode.Unauthorized.ToString();
			}
		}
	}
}