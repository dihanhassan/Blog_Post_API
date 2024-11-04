using System.Linq.Expressions;

namespace BlogPost.Domain.Interfaces
{
    public interface IBaseRepository<T>
    {
        #region Get
        IQueryable<T> Active();
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IQueryable<T>> GetAllAsync();
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> expression);
        Task<IQueryable<T>> GetPagedDataAsync(IQueryable<T> query, int pageIndex, int pageSize);

        #endregion Get

        #region Save
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<List<T>> AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        #endregion Save

        #region Update
        Task UpdateAsync(T entity);
        #endregion Update

        #region Delete
        Task DeleteAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        #endregion Delete

        #region SOFT-DELETE
        Task SoftDeleteAsync(T entity);
        Task RestoreDeleteAsync(T entity);
        #endregion SOFT-DELETE
    }
}
