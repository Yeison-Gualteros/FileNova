using AutoMapper;
using Contracts;
using Contracts.Interface;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.DataTransferObjects.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager _logger;  // <- tu logger personalizado
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RepositoryContext _context;
        private readonly JwtConfiguration _jwtConfig;
        private readonly IEmailService _emailService;

        private User? _currentUser;

        public AuthenticationService(
            ILoggerManager logger,  // <- cambia aquí
            IMapper mapper,
            UserManager<User> userManager,
            IOptions<JwtConfiguration> config,
            RepositoryContext context,
            IEmailService emailService)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _context = context;
            _jwtConfig = config.Value;
            _emailService = emailService;
        }

        // Registro de usuario
        public async Task<IdentityResult> RegisterUser(UserForRegistrationDto dto)
        {
            var user = _mapper.Map<User>(dto);

            user.UserName = dto.UserName.ToLower();
            user.Estado = 1;
            user.MustChangePassword = true;

            // Contraseña temporal
            var tempPassword = string.IsNullOrWhiteSpace(dto.Password)
                ? $"Temp@{Guid.NewGuid().ToString("N")[..8]}"
                : dto.Password;

            var result = await _userManager.CreateAsync(user, tempPassword);

            if (!result.Succeeded)
                return result;

            if (dto.RoleIds != null && dto.RoleIds.Any())
            {
                var roleId = dto.RoleIds.First();
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);

                if (role == null)
                    return IdentityResult.Failed(
                        new IdentityError { Description = "Rol no válido" });

                await _userManager.AddToRoleAsync(user, role.Name);
            }

            // ⚠️ OPCIONAL: aquí podrías enviar el correo con la contraseña temporal
            await _emailService.SendPasswordAsync(
                user.Email,
                user.UserName,
                tempPassword
            );

            return result;
        }

        // Validar login
        public async Task<bool> ValidateUser(UserForAuthenticationDto dto)
        {
            if (dto == null) return false;

            // Normalizar y buscar usuario
            var normalizedUserName = _userManager.NormalizeName(dto.UserName);
            _currentUser = await _userManager.FindByNameAsync(normalizedUserName);

            if (_currentUser == null)
            {
                _logger.LogWarn($"Usuario no encontrado: {dto.UserName}");
                return false;
            }

            // ❌ Bloquear usuarios inactivos o eliminados
            if (_currentUser.Estado == 3)
            {
                _logger.LogWarn($"Usuario inactivo: {dto.UserName}");
                return false;
            }
            if (_currentUser.Estado == 0)
            {
                _logger.LogWarn($"Usuario eliminado: {dto.UserName}");
                return false;
            }

            // Validar contraseña
            bool validPassword = await _userManager.CheckPasswordAsync(_currentUser, dto.Password);

            if (!validPassword)
                _logger.LogWarn($"Contraseña incorrecta: {dto.UserName}");

            return validPassword;
        }


        // Crear JWT con roles y permisos
        public async Task<TokenDto> CreateToken(bool populateExpiry = true)
        {
            if (_currentUser == null)
                throw new InvalidOperationException("Usuario no validado.");

            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var token = GenerateJwtToken(signingCredentials, claims);

            // Refresh token
            string refreshToken = GenerateRefreshToken();
            _currentUser.RefreshTokken = refreshToken;

            if (populateExpiry)
                _currentUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(_currentUser);

            return new TokenDto(
                AccessToken: new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken: refreshToken
            );
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Key!);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            if (_currentUser == null)
                throw new InvalidOperationException("Usuario no validado.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _currentUser.UserName),
                new Claim(ClaimTypes.NameIdentifier, _currentUser.Id),
                new Claim("mustChangePassword", _currentUser.MustChangePassword.ToString())
            };

            var permissionSet = new HashSet<string>();

            // 🔹 Rol (solo uno)
            var roleName = (await _userManager.GetRolesAsync(_currentUser)).FirstOrDefault();
            if (!string.IsNullOrEmpty(roleName))
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));

                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (role != null)
                {
                    var rolePermissions = await _context.Rol_Permisos
                        .Include(rp => rp.Permiso)
                        .Where(rp => rp.Id_Rol == role.Id)
                        .Select(rp => rp.Permiso.Nombre)
                        .ToListAsync();

                    foreach (var p in rolePermissions)
                        permissionSet.Add(p);
                }
            }

            // 🔹 Permisos extra del usuario
            var userPermissions = await _context.user_Permisos
                .Include(up => up.Permiso)
                .Where(up => up.UserId == _currentUser.Id)
                .Select(up => up.Permiso.Nombre)
                .ToListAsync();

            foreach (var p in userPermissions)
                permissionSet.Add(p);

            // 🔹 Claims finales
            foreach (var permiso in permissionSet)
                claims.Add(new Claim("permission", permiso));

            _logger.LogInfo("=== PERMISOS DEL USUARIO ===");

            foreach (var p in permissionSet)
            {
                _logger.LogInfo(p);
            }


            return claims;
        }


        private JwtSecurityToken GenerateJwtToken(SigningCredentials creds, List<Claim> claims)
        {
            return new JwtSecurityToken(
                issuer: _jwtConfig.ValidIssuer,
                audience: _jwtConfig.ValidAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtConfig.Expires)),
                signingCredentials: creds
            );
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        // Refresh token
        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

            var user = await _userManager.FindByNameAsync(principal.Identity!.Name!);
            if (user == null ||
                user.RefreshTokken != tokenDto.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new Exception("Refresh token inválido o expirado.");
            }

            _currentUser = user;
            return await CreateToken(populateExpiry: false);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidation = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key!)),
                ValidateLifetime = false,
                ValidIssuer = _jwtConfig.ValidIssuer,
                ValidAudience = _jwtConfig.ValidAudience
            };

            var handler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;

            var principal = handler.ValidateToken(token, tokenValidation, out validatedToken);
            if (validatedToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Token inválido");
            }

            return principal;
        }

        public User GetCurrentUser()
        {
            if (_currentUser == null)
                throw new InvalidOperationException("Usuario no autenticado.");

            return _currentUser;
        }

        public async Task<IdentityResult> ChangePassword(ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return IdentityResult.Failed(
                    new IdentityError { Description = "Usuario no encontrado" });

            var result = await _userManager.ChangePasswordAsync(
                user,
                dto.OldPassword,
                dto.NewPassword
            );

            if (!result.Succeeded)
                return result;

            user.MustChangePassword = false;
            await _userManager.UpdateAsync(user);

            return IdentityResult.Success;
        }


    }
}
