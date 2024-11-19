using BlogPost.Application.Dto.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogPost.Service.Helper
{
    public static class ServiceHelper
    {
        public static async Task<ResponseDto<T>> MapToResponse<T>(T data, string msg) where T : class
        {
            ResponseDto<T> response = new()
            {
                Data = data,
                Message = msg,
                StatusCode = 200
            };
            return await Task.FromResult(response);
        }
    }
}
