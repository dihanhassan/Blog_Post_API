using BlogPost.Domain.Entities;

namespace BlogPost.Domain.Interfaces
{
    public interface IAuthRepository : IBaseRepository<User>
    {
        Task<User> userRegisterAsync(User userRegister);
        Task<User> userLoginAsync(String email);
        Task<User> UpdateUserByEmailAsync(User user);
    }
}
