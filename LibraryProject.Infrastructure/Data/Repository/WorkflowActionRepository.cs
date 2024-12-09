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
    public class WorkflowActionRepository:IWorkflowActionRepository
    {
        private readonly LibraryProjectContext _context;

        public WorkflowActionRepository(LibraryProjectContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(WorkflowAction entity)
        {
            await _context.WorkflowActions.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(WorkflowAction entity)
        {
            _context.WorkflowActions.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<WorkflowAction>> GetAllAsync()
        {
            var workflowActions = await _context.WorkflowActions.ToListAsync();

            return workflowActions;
        }

        public async Task<WorkflowAction?> GetByIdAsync(int id)
        {
            var workflowAction = await _context.WorkflowActions.FindAsync(id);

            return workflowAction;
        }
        public async Task<List<WorkflowAction>> GetByProcessIdAsync(int processId)
        {
            var workflowActions = await _context.WorkflowActions
                                                .Where(w => w.ProcessId == processId)
                                                .ToListAsync();

            return workflowActions;
        }

        public async Task<WorkflowAction?> GetFirstOrDefaultAsync(Expression<Func<WorkflowAction, bool>> expression)
        {
            return await _context.WorkflowActions.FirstOrDefaultAsync(expression);
        }

        public async Task UpdateAsync(WorkflowAction entity)
        {
            _context.WorkflowActions.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
