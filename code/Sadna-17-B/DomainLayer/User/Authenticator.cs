using Microsoft.IdentityModel.Tokens;
using Sadna_17_B.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Sadna_17_B.DomainLayer.User
{
    public class Authenticator
    {
        private const string SecretKey = "this_is_a_very_secret_key_for_jwt_token"; // This should be stored securely
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        private List<string> invalidatedTokens = new List<string>();

        public string GenerateToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new[] {
                new Claim(ClaimTypes.Name, username)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GetNameFromToken(string token)
        {
            ClaimsPrincipal principal = ValidateToken(token);
            if (principal == null)
            {
                throw new Sadna17BException("Invalid access token was given.");
            }
            else if (invalidatedTokens.Contains(token))
            {
                throw new Sadna17BException("Given access token is no longer valid.");
            }
            return principal.FindFirst(ClaimTypes.Name).Value;
        }

        public int GetGuestIDFromToken(string token)
        {
            string guestIDstring = GetNameFromToken(token);
            int guestID;
            bool validGuestID = int.TryParse(guestIDstring, out guestID);
            if (!validGuestID)
            {
                throw new Sadna17BException("Invalid GuestID was saved in the system.");
            }
            return guestID;
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            if (invalidatedTokens.Contains(token))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _signingKey
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public void InvalidateToken(string token)
        {
            if (!invalidatedTokens.Contains(token))
            {
                invalidatedTokens.Add(token);
            } else
            {
                throw new Sadna17BException("Given token is already logged out of the system.");
            }
        }
    }
}