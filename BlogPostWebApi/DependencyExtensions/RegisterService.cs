using BlogPost.Application.Interfaces.Auth;
using BlogPost.Application.Interfaces.Categories;
using BlogPost.Application.Interfaces.Posts;
using BlogPost.Data.Setups;
using BlogPost.Domain.Interfaces;
using BlogPost.Service;
using BlogPost.Service.Categories;
using BlogPost.Service.Posts;

namespace BlogPostWebApi.DependencyExtensions
{
    public static class RegisterService
    {
        public static void AddServices(this IServiceCollection services)
        {
            #region A
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITransactionUtil, TransactionUtil>();
            #endregion A
            #region C
            services.AddScoped<ICategoryService, CategoryService>();
            #endregion C
            #region P
            services.AddScoped<IPostService, PostService>();
            #endregion P
        }
    }
}
