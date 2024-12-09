using LibraryProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Interfaces
{
    public interface IBookRequestRepository
    {
        Task<IEnumerable<BookRequest>> GetAllAsync();
        Task<BookRequest> GetAsync(int id);
        Task<BookRequest> AddAsync(BookRequest bookRequest);
        Task<BookRequest> UpdateAsync(BookRequest bookRequest);
        Task DeleteAsync(int id);
        Task<IEnumerable<BookRequest>> GetAllByUserAsync(Expression<Func<BookRequest, bool>> expression);
        Task<IEnumerable<BookRequest>> GetAllToStatusAsync(string userRole);
    }
}
