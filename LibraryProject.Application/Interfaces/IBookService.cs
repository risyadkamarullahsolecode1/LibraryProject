using LibraryProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> SearchBookLanguage(string language);
        Task DeleteStampBook(int id, string deleteStatus);
        Task<int> GetTotalBook();
        Task<byte[]> generatereportpdf();
        Task<Dictionary<string, int>> GetBookByCategory();
    }
}
