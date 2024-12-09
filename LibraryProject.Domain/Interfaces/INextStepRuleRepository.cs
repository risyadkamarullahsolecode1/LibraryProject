using LibraryProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Interfaces
{
    public interface INextStepRuleRepository
    {
        Task<IEnumerable<NextStepRule>> GetAllAsync();

        Task<NextStepRule> GetByIdAsync(int id);

        Task CreateAsync(NextStepRule entity);

        Task UpdateAsync(NextStepRule entity);

        Task DeleteAsync(NextStepRule entity);
        Task<NextStepRule?> GetFirstOrDefaultAsync(Expression<Func<NextStepRule, bool>> expression);
    }
}
