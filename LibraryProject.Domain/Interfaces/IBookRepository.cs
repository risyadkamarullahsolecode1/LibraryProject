using LibraryProject.Domain.Entities;
using LibraryProject.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooks();
        Task<Book> GetBookById(int id);
        Task<Book> AddBook(Book book);
        Task<Book> UpdateBook(Book book);
        Task<bool> DeleteBook(int id);
        Task<IEnumerable<Book>> SearchBookAsync(QueryObject query, Pagination pagination);
        Task SaveChangesAsync();
        Task<int> GetTotalBooksAsync();
    }
}
