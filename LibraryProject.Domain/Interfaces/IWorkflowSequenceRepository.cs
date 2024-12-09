using LibraryProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Interfaces
{
    public interface IWorkflowSequenceRepository
    {
        Task<IEnumerable<WorkflowSequence>> GetAllAsync();

        Task<WorkflowSequence> GetByIdAsync(int id);

        Task CreateAsync(WorkflowSequence entity);

        Task UpdateAsync(WorkflowSequence entity);

        Task DeleteAsync(WorkflowSequence entity);
        Task<WorkflowSequence?> GetFirstOrDefaultAsync(Expression<Func<WorkflowSequence, bool>> expression);
        Task<IEnumerable<WorkflowSequence>> GetAllAsync(Expression<Func<WorkflowSequence, bool>> expression);
    }
}
