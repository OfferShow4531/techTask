using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RESTfulAPI_Technical_Task_.Composable
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly IDbConnection _db;

        public JwtService(IConfiguration config, IDbConnection db)
        {
            _config = config;
            _db = db;
        }

        public async Task<string> GenerateTokenAsync(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["JwtSettings:ExpiryMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: new[] { new Claim(ClaimTypes.Name, username) },
                expires: expires,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Сохраняем токен в базе
            var query = @"INSERT INTO ""TechnicalAPI"".""Tokens"" (""Token"", ""CreatedAt"", ""ExpiresAt"") 
                      VALUES (@Token, @CreatedAt, @ExpiresAt);";
            await _db.ExecuteAsync(query, new { Token = tokenString, CreatedAt = DateTime.UtcNow, ExpiresAt = expires });

            return tokenString;
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var query = @"SELECT COUNT(*) FROM ""TechnicalAPI"".""Tokens"" 
                      WHERE ""Token"" = @Token AND ""ExpiresAt"" > NOW();";
            var count = await _db.ExecuteScalarAsync<int>(query, new { Token = token });

            return count > 0;
        }
    }
}
