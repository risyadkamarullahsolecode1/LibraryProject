using LibraryProject.Application.Dtos;
using LibraryProject.Application.Interfaces;
using LibraryProject.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Services
{
    public class DashboardService:IDashboardService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookBorrowRepository _bookBorrowRepository;

        public DashboardService(IBookRepository bookRepository, IUserRepository userRepository, IBookBorrowRepository bookBorrowRepository)
        {
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _bookBorrowRepository = bookBorrowRepository;
        }

        public async Task<KpiReportDto> GetReport()
        {
            var totalBooks = await _bookRepository.GetTotalBooksAsync();

            var overduebooks = await _bookBorrowRepository.GetOverdueBorrowsByUser();

            var category = await _bookRepository.GetBooksCountByCategoryAsync();
            var member = await _bookBorrowRepository.GetBorrowCountsGroupedByMemberAsync();

            // Convert IEnumerable<(string, int)> to Dictionary<string, int>
            var members = (await _bookBorrowRepository.GetBorrowCountsGroupedByMemberAsync())
                .ToDictionary(x => x.AppUserId, x => x.BorrowCount);

            return new KpiReportDto
            {
                TotalBook = totalBooks,
                OverdueBooks = overduebooks,
                Category = category,
                ActiveMember = members
            };

        }
    }
}
