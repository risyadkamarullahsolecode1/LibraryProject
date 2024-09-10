using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryProject.Domain.Entities;
using LibraryProject.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryProject.Infrastructure.Data.Repository
{
    public class BookBorrowRepository:IBookBorrowRepository
    {
        private readonly LibraryProjectContext _context;

        public BookBorrowRepository(LibraryProjectContext context)
        {
            _context = context;
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
    }
}
