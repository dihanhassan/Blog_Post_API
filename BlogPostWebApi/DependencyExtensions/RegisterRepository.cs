using BlogPost.Domain.Interfaces;
using BlogPost.Domain.Interfaces.Categories;
using BlogPost.Domain.Interfaces.Posts;
using BlogPost.Repo.Auth;
using BlogPost.Repo.Categories;
using BlogPost.Repo.Posts;

namespace BlogPostWebApi.DependencyExtensions
{
    public static class RegisterRepository
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
        }
    }
}
