using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnimotors.Reclamos.Domain.AggregatesModel.UsuarioAggregate;
using Tecnimotors.Reclamos.Domain.Models;

namespace Tecnimotors.Reclamos.Domain.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> AuthenticateAsync(string email, string password);
        string GenerateJwtTokenAsync(Usuario usuario);
        Task<int> RegisterUserAsync(UserRegistrationModel model);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt);
    }
}
