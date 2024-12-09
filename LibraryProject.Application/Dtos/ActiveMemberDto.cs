using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Dtos
{
    public class ActiveMemberDto
    {
        public string MemberName { get; set; } // Name of the member
        public int BorrowCount { get; set; }  // Number of books borrowed
    }
}
