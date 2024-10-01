using AutoMapper;
using BlogPost.Application.AppSettings;
using BlogPost.Application.CustomExceptions;
using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;
using BlogPost.Application.Interfaces.Auth;
using BlogPost.Domain.Entities;
using BlogPost.Domain.Interfaces;

using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogPost.Service
{
    public class AuthService : IAuthService
    {

        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        private readonly JWTSettings _jWTSettings;
        //private const int TokenExpiryTimeInMinute = 1440;
        private const int TokenExpiryTimeInMinute = 1;
        private const int RefreshTokenExpiryTimeDays = 7;
        private const string SecretKeyForResetPass = "Yjok123KjdflkLhjkl90483iujokkl904fdedmHjHJDKJF";
        public AuthService(
          IAuthRepository authRepository,
          IMapper mapper,
          JWTSettings jWTSettings
          )
        {
            _authRepository = authRepository;
            _mapper = mapper;
            _jWTSettings = jWTSettings;
        }


        //private methods
        private string GenerateToken(User user, string userId, int? val,string? type="")
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTSettings.SecretKey));

            if (val == 2)
            {
                key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKeyForResetPass));
            }
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", userId.ToString()),
                new Claim("Email", user.Email),

            };
            var TokenExpirationTime = DateTime.UtcNow.AddMinutes(TokenExpiryTimeInMinute);
            if(type=="refreshToken")
            {
                TokenExpirationTime = DateTime.UtcNow.AddDays(RefreshTokenExpiryTimeDays);
            }
            var token = new JwtSecurityToken(
                _jWTSettings.Issuer,
                _jWTSettings.Audience,
                claims,
                expires: TokenExpirationTime,
                signingCredentials: credentials
            );

            return  new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserLoginResponse> RequestGenerateRefreshTokenAsync(string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jWTSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jWTSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTSettings.SecretKey)),
                ValidateLifetime = false 
            }, out SecurityToken validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;

            var userId = jwtToken?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            var email = jwtToken?.Claims.FirstOrDefault(x => x.Type == "Email")?.Value;

            if (userId == null || email == null)
            {
                throw new SecurityTokenException("Invalid token claims.");
            }

       
            var expirationClaim = jwtToken?.ValidTo;
            if (expirationClaim <= DateTime.UtcNow)
            {
                throw new SecurityTokenException("Expired refresh token.");
            }

           
            var user = await _authRepository.userLoginAsync(email);

            if (user is null || !userId.Equals(user.Id.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                throw new ClientCustomException("Email is not correct", new()
                {
                    { "Email", "User not found with this email" }
                });
            }
   
            var response = _mapper.Map<UserLoginResponse>(user);
            response.Token = GenerateToken(user, user.Id.ToString(), 1);
            response.RefreshToken = GenerateToken(user, user.Id.ToString(), null, "refreshToken");
            return response;
        }




        public async Task<UserLoginResponse> Login(LoginRequest loginRequest)
        {
            var user = await _authRepository.userLoginAsync(loginRequest.Email);

            if (user is null)
            {
                throw new ClientCustomException("Email is not correct", new()
                {
                    { "Email", "User not found with this email" }
                });
            }

            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                throw new ClientCustomException("Password is not correct", new()
                {
                    { "Password", "Password is not correct" }
                });
            }

            var response = _mapper.Map<UserLoginResponse>(user);
            response.Token = GenerateToken(user, user.Id.ToString(), 1);
            response.RefreshToken = GenerateToken(user, user.Id.ToString(),null,"refreshToken");
            return response;
        }

        private async Task ValidateRequest(UserRequest userRequest)
        {
            var response = await GetUserByEmail(userRequest.Email);

            if (response != null)
            {
                throw new UserRegistrationException("Email already exist", new()
                {
                    { "Email", "Email already exist" }
                });
            }
        }

        public async Task<UserResponse> Register(UserRequest request)
        {
            try
            {
                await ValidateRequest(request);

                var user = _mapper.Map<User>(request);
                var savedUser = await _authRepository.userRegisterAsync(user);
                var userRes = _mapper.Map<UserLoginResponse>(savedUser);
                return userRes;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<UserLoginResponse> GetUserByEmail(string email)
        {
            var user = await _authRepository.userLoginAsync(email);
            var response = _mapper.Map<UserLoginResponse>(user);
            return response;
        }
    }
}
