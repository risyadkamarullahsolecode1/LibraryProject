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
    public class WorkflowSequenceRepository:IWorkflowSequenceRepository
    {
        private readonly LibraryProjectContext _context;

        public WorkflowSequenceRepository(LibraryProjectContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(WorkflowSequence entity)
        {
            await _context.WorkflowSequences.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(WorkflowSequence entity)
        {
            _context.WorkflowSequences.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<WorkflowSequence>> GetAllAsync()
        {
            var workflowSequences = await _context.WorkflowSequences.ToListAsync();

            return workflowSequences;
        }

        public async Task<IEnumerable<WorkflowSequence>> GetAllAsync(Expression<Func<WorkflowSequence, bool>> expression)
        {
            var workflowSequences = await _context.WorkflowSequences.Where(expression).ToListAsync();

            return workflowSequences;
        }

        public async Task<WorkflowSequence?> GetByIdAsync(int id)
        {
            var workflowSequence = await _context.WorkflowSequences.FindAsync(id);

            return workflowSequence;
        }

        public async Task UpdateAsync(WorkflowSequence entity)
        {
            _context.WorkflowSequences.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<WorkflowSequence?> GetFirstOrDefaultAsync(Expression<Func<WorkflowSequence, bool>> expression)
        {
            return await _context.WorkflowSequences.OrderBy(wfs => wfs.StepId).FirstOrDefaultAsync(expression);
        }
    }
}
