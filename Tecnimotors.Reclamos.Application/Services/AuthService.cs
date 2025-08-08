using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;
using Tecnimotors.Reclamos.Domain.Models;
using Tecnimotors.Reclamos.Domain.Services;

namespace Tecnimotors.Reclamos.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<AuthResponse> AuthenticateAsync(string email, string password)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(email);
            if (usuario == null || !VerifyPassword(password, usuario.Password, usuario.PasswordSalt))
            {
                return null;
            }

            string token = GenerateJwtTokenAsync(usuario);

            return new AuthResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpirationHours"] ?? "24")),
                UsuarioId = usuario.UsuarioId,
                Nombre = $"{usuario.Nombre} {usuario.Apellidos}",
                Rol = usuario.NombreRol,
                Email = usuario.Email
            };
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public string GenerateJwtTokenAsync(Usuario usuario)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, usuario.Nombre),
                new Claim(JwtRegisteredClaimNames.FamilyName, usuario.Apellidos),
                new Claim(ClaimTypes.Role, usuario.NombreRol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expirationHours = Convert.ToDouble(_configuration["Jwt:ExpirationHours"] ?? "24");
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expirationHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<int> RegisterUserAsync(UserRegistrationModel model)
        {
            var existingUser = await _usuarioRepository.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return 0;
            }

            CreatePasswordHash(model.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var newUsuario = new Usuario(
                model.Email,
                passwordHash,
                passwordSalt,
                model.Nombre,
                model.Apellidos,
                model.RolId
            );

            return await _usuarioRepository.CreateAsync(newUsuario);
        }

        public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Comparar los hashes byte a byte
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                        return false;
                }
            }

            return true;
        }
    }
}
