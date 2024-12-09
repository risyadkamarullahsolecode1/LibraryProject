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
    public class ProcessRepository:IProcessRepository
    {
        private readonly LibraryProjectContext _context;

        public ProcessRepository(LibraryProjectContext context) 
        { 
            _context = context; 
        }

        public async Task CreateAsync(Process entity)
        {
            await _context.Processs.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Process entity)
        {
            _context.Processs.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Process>> GetAllAsync()
        {
            var processes = await _context.Processs.ToListAsync();

            return processes;
        }

        public async Task<Process?> GetByIdAsync(int id)
        {
            // var process = await _context.Processes.FindAsync(id);
            return await _context.Processs
                .Include(p => p.CurrentStep)
                .ThenInclude(cs => cs.RequiredRole)
                .FirstOrDefaultAsync(p => p.ProcessId == id);
        }

        public async Task UpdateAsync(Process entity)
        {
            _context.Processs.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Process?> GetFirstOrDefaultAsync(Expression<Func<Process, bool>> expression)
        {
            return await _context.Processs.FirstOrDefaultAsync(expression);
        }
    }
}
