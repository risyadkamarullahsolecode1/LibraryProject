using LibraryProject.Domain.Entities;
using LibraryProject.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Infrastructure.Data.Repository
{
    public class BookRequestRepository:IBookRequestRepository
    {
        private readonly LibraryProjectContext _context;

        public BookRequestRepository(LibraryProjectContext context)
        {
            _context = context;
        }

        public async Task<BookRequest> AddAsync(BookRequest bookRequest)
        {
            await _context.BookRequests.AddAsync(bookRequest);
            await _context.SaveChangesAsync();
            return bookRequest;
        }

        public async Task DeleteAsync(int id)
        {
            var req = await _context.BookRequests.FindAsync(id);
            _context.BookRequests.Remove(req);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BookRequest>> GetAllAsync()
        {
            return await _context.BookRequests.ToListAsync();
        }

        public async Task<BookRequest> GetAsync(int id)
        {
            var req = await _context.BookRequests.FindAsync(id);
            return req;
        }

        public async Task<BookRequest> UpdateAsync(BookRequest bookRequest)
        {
            _context.BookRequests.Update(bookRequest);
            await _context.SaveChangesAsync();
            return bookRequest;
        }

        public async Task<IEnumerable<BookRequest>> GetAllByUserAsync(Expression<Func<BookRequest, bool>> expression)
        {
            return await _context.BookRequests
                .Include(r => r.Process)
                .ThenInclude(p => p.CurrentStep)
                .ThenInclude(wfs => wfs.RequiredRole)
                .Include(r => r.Process)
                .ThenInclude(p => p.Requester)
                .Include(r => r.Process)
                .ThenInclude(p => p.WorkflowActions)
                .Where(expression) // Filter
                .ToListAsync();
        }

        public async Task<IEnumerable<BookRequest>> GetAllToStatusAsync(string userRole)
        {
            return await _context.BookRequests
                .Include(r => r.Process)
                    .ThenInclude(p => p.CurrentStep)
                    .ThenInclude(wfs => wfs.RequiredRole)
                .Include(r => r.Process)
                    .ThenInclude(p => p.WorkflowActions)
                .Where(r => r.Process.CurrentStep.RequiredRole.Name == userRole)
                .ToListAsync();
        }
    }
}
