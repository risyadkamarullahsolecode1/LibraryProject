using LibraryProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Domain.Interfaces
{
    public interface IProcessRepository
    {
        Task<IEnumerable<Process>> GetAllAsync();

        Task<Process> GetByIdAsync(int id);

        Task CreateAsync(Process entity);

        Task UpdateAsync(Process entity);

        Task DeleteAsync(Process entity);
        Task<Process?> GetFirstOrDefaultAsync(Expression<Func<Process, bool>> expression);
    }
}
