using AutoMapper;
using BlogPost.Application.AppSettings;
using BlogPost.Application.CustomExceptions;
using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;
using BlogPost.Application.Interfaces.Auth;
using BlogPost.Domain.Entities;
using BlogPost.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogPost.Service.Auth
{
    public class AuthService : IAuthService
    {

        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        private readonly JWTSettings _jWTSettings;
        private const int TokenExpiryTimeInMinute = 1440;
        // private const int TokenExpiryTimeInMinute = 1;
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
        private string GenerateToken(User user, string userId, int? val, string? type = "")
        {
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jWTSettings.SecretKey));

            if (val == 2)
            {
                key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKeyForResetPass));
            }
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", userId.ToString()),
                new Claim("Email", user.Email),

            };
            DateTime TokenExpirationTime = DateTime.UtcNow.AddMinutes(TokenExpiryTimeInMinute);
            if (type == "refreshToken")
            {
                TokenExpirationTime = DateTime.UtcNow.AddDays(RefreshTokenExpiryTimeDays);
            }
            JwtSecurityToken token = new(
                _jWTSettings.Issuer,
                _jWTSettings.Audience,
                claims,
                expires: TokenExpirationTime,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ResponseDto<UserLoginResponse>> RequestGenerateRefreshTokenAsync(string refreshToken)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            ClaimsPrincipal principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jWTSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jWTSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTSettings.SecretKey)),
                ValidateLifetime = false
            }, out SecurityToken validatedToken);

            JwtSecurityToken? jwtToken = validatedToken as JwtSecurityToken;

            string? userId = jwtToken?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            string? email = jwtToken?.Claims.FirstOrDefault(x => x.Type == "Email")?.Value;

            if (userId == null || email == null)
            {
                throw new SecurityTokenException("Invalid token claims.");
            }


            DateTime? expirationClaim = jwtToken?.ValidTo;
            if (expirationClaim <= DateTime.UtcNow)
            {
                throw new SecurityTokenException("Expired refresh token.");
            }


            User? user = await _authRepository.userLoginAsync(email);

            if (user is null || !userId.Equals(user.Id.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                throw new ClientCustomException("Email is not correct", new()
                {
                    { "Email", "User not found with this email" }
                });
            }
            return await MapUserForRsp(user, "Token Generate in successfully");
        }
        // exract the user info from token
        public async Task<int> ExtractUserInfoFromToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jWTSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jWTSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTSettings.SecretKey)),
                ValidateLifetime = true
            }, out SecurityToken validatedToken);

            JwtSecurityToken? jwtToken = validatedToken as JwtSecurityToken;

            string? userId = jwtToken?.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;

            return userId == null ? throw new SecurityTokenException("Invalid token claims User is invalid") : await Task.FromResult(Convert.ToInt32(userId));
        }


        public async Task<ResponseDto<UserLoginResponse>> Login(LoginRequest loginRequest)
        {
            User? user = await _authRepository.userLoginAsync(loginRequest.Email);

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

            return await MapUserForRsp(user, "User logged in successfully");
        }


        private async Task<ResponseDto<UserLoginResponse>> MapUserForRsp(User user, string msg)
        {
            UserLoginResponse loginRsp = _mapper.Map<UserLoginResponse>(user);
            loginRsp.Token = GenerateToken(user, user.Id.ToString(), 1);
            loginRsp.RefreshToken = GenerateToken(user, user.Id.ToString(), null, "refreshToken");

            ResponseDto<UserLoginResponse> response = new()
            {
                Data = loginRsp,
                Message = msg,
                StatusCode = 200
            };
            return response;
        }

        private async Task ValidateRequest(UserRequest userRequest)
        {
            UserLoginResponse response = await GetUserByEmail(userRequest.Email);

            if (response != null)
            {
                throw new UserRegistrationException("Email already exist", new()
                {
                    { "Email", "Email already exist" }
                });
            }
        }

        public async Task<ResponseDto<UserLoginResponse>> Register(UserRequest request)
        {
            try
            {
                await ValidateRequest(request);

                User user = _mapper.Map<User>(request);
                User savedUser = await _authRepository.userRegisterAsync(user);
                UserLoginResponse userRes = _mapper.Map<UserLoginResponse>(savedUser);
                ResponseDto<UserLoginResponse> response = new()
                {
                    Data = userRes,
                    Message = "User registered successfully",
                    StatusCode = 200
                };
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<UserLoginResponse> GetUserByEmail(string email)
        {
            User user = await _authRepository.userLoginAsync(email);
            UserLoginResponse response = _mapper.Map<UserLoginResponse>(user);
            return response;
        }
    }
}
