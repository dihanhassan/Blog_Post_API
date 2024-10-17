using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogPost.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<ResponseDto<UserLoginResponse>> Login(LoginRequest loginRequest);
        Task<ResponseDto<UserLoginResponse>> RequestGenerateRefreshTokenAsync(string refreshToken);

        #region Save
        Task<ResponseDto<UserLoginResponse>> Register(UserRequest request);
        #endregion Save
    }
}
