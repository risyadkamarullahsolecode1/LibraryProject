using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryProject.Application.Dtos;
using LibraryProject.Domain.Entities;
using LibraryProject.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LibraryProject.Infrastructure.Data.Repository
{
    public class BookBorrowRepository:IBookBorrowRepository
    {
        private readonly LibraryProjectContext _context;
        private readonly IConfiguration _configuration;

        public BookBorrowRepository(LibraryProjectContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<int> GetBorrowedBooksCountByUser(string userId)
        {
            return await _context.BookBorrows
                         .Where(lb => lb.AppUserId == userId && lb.TanggalKembali >= DateOnly.FromDateTime(DateTime.Now))
                         .CountAsync();
        }

        public BookBorrow GetBorrowRecord(string userId, int bookId)
        {
            return _context.BookBorrows.FirstOrDefault(b => b.AppUserId == userId && b.BookId == bookId);
        }

        public async Task AddBorrowRecord(BookBorrow borrowRecord)
        {
            _context.BookBorrows.AddAsync(borrowRecord);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task DeleteBorrowRecord(BookBorrow borrowRecord)
        {
            _context.BookBorrows.Remove(borrowRecord);
            _context.SaveChanges();
        }

        public async Task UpdateBorrowRecord(BookBorrow borrow)
        {
            _context.BookBorrows.Update(borrow);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<BookBorrow>> GetBorrowsByUserIdAsync(string userId)
        {
           return await _context.BookBorrows
                .Where(bb => bb.AppUserId == userId)
                .Include(bb => bb.BorrowId)
                .OrderBy(bb => bb.AppUserId)
                .ToListAsync();
        }
        public async Task<List<BookBorrow>> GetOverdueBooks()
        {
            var PinjamDuration = _configuration.GetValue<int>("LibrarySettings:PinjamDuration");

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var dueDateLimit = currentDate.AddDays(PinjamDuration);

            return await _context.BookBorrows
                .Where(b => b.TanggalKembali == null )
                .Select(b => new BookBorrow
                {
                    BookId = b.BookId,
                    TanggalPinjam = b.TanggalPinjam,
                    AppUserId = b.AppUserId,
                    DueDate = b.DueDate,
                    TanggalKembali = b.TanggalKembali,
                    Penalty = b.Penalty,
                })
                .ToListAsync();

        }

        public async Task<IEnumerable<BookBorrow>> GetOverdueBorrowsByUser()
        {
            return await _context.BookBorrows
                .Where(b => b.TanggalKembali > b.DueDate )
                .Include(b => b.AppUser)
                .Include(b => b.Book)
                .ToListAsync() ;
        }

        public async Task<IEnumerable<BookBorrow>> GetAll()
        {
            return await _context.BookBorrows.ToListAsync();
        }

       
    }
}
