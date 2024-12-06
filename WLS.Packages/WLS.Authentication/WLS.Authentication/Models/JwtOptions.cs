using Microsoft.IdentityModel.Tokens;

namespace WLS.Authentication.Models
{
    public class JwtOptions
    {
        public readonly string JwtValidIssuer;
        public readonly string JwtValidAudience;
        public readonly string JwtSecretKey;
        public readonly SigningCredentials JwtSigningCredentials;

        public JwtOptions(string jwtValidIssuer, string jwtValidAudience, string jwtSecretKey)
        {
            JwtValidIssuer = jwtValidIssuer;
            JwtValidAudience = jwtValidAudience;
            JwtSecretKey = jwtSecretKey;
            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(JwtSecretKey));
            JwtSigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        }
    }
}
