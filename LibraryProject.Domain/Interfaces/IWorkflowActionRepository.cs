using LibraryProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Interfaces
{
    public interface IWorkflowActionRepository
    {
        Task<IEnumerable<WorkflowAction>> GetAllAsync();

        Task<WorkflowAction> GetByIdAsync(int id);

        Task CreateAsync(WorkflowAction entity);

        Task UpdateAsync(WorkflowAction entity);

        Task DeleteAsync(WorkflowAction entity);
        Task<WorkflowAction?> GetFirstOrDefaultAsync(Expression<Func<WorkflowAction, bool>> expression);
        Task<List<WorkflowAction>> GetByProcessIdAsync(int processId);
    }
}
