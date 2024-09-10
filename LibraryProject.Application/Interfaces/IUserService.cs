using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Interfaces
{
    public interface IUserService
    {
        Task AttachNotes(int id, string notes);
        Task<byte[]> generatereportpdf();
    }
}
