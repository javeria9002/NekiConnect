using NekiConnect.Models;

namespace NekiConnect.Interfaces
{
    public interface IBlogService
    {
        Task<List<BlogPost>> GetByNgoIdAsync(int ngoId);
        Task<List<BlogPost>> GetAllBlogsAsync();
        Task<List<BlogPost>> GetAllPublishedAsync();
        Task<BlogPost?> GetByIdAsync(int id);
        Task CreateAsync(BlogPost post);
        Task UpdateAsync(BlogPost post);
        Task DeleteAsync(int id);
        Task<int> GetCountAsync(int ngoId);
    }
}