using System;
using LibraryProject.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Interfaces
{
    public interface IBookBorrowRepository
    {
        Task<int> GetBorrowedBooksCountByUser(string userId);
        BookBorrow GetBorrowRecord(string userId, int bookId);
        Task AddBorrowRecord(BookBorrow borrowRecord);
        Task UpdateBorrowRecord(BookBorrow borrowRecord);
        Task DeleteBorrowRecord(BookBorrow borrowRecord);
        Task<List<User>> GetMostActiveUser(string topN);
    }
}
