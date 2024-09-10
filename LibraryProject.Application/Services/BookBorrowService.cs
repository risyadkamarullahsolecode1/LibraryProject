using LibraryProject.Application.Interfaces;
using LibraryProject.Domain.Entities;
using LibraryProject.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Services
{
    public class BookBorrowService:IBookBorrowService
    {
        private readonly IConfiguration _configuration;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookBorrowRepository _bookBorrowRepository;

        public BookBorrowService(IConfiguration configuration, IBookRepository bookRepository, IUserRepository userRepository, IBookBorrowRepository bookBorrowRepository)
        {
            _configuration = configuration;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _bookBorrowRepository = bookBorrowRepository;
        }

        public async Task CheckOutBook(string userId, int bookId)
        {

            var maxBooks = _configuration.GetValue<int>("LibrarySettings:MaxBooksPerUser");
            var PinjamDuration = _configuration.GetValue<int>("LibrarySettings:PinjamDuration");

            var book = await _bookRepository.GetBookById(bookId); // Await the async method
            if (book == null)
            {
                throw new InvalidOperationException("Book not found.");
            }

            int currentBorrowedBookCount = await _bookBorrowRepository.GetBorrowedBooksCountByUser(userId);

            // Check if the user has already borrowed the maximum number of books
            if (currentBorrowedBookCount >= maxBooks)
            {
                throw new InvalidOperationException("User has already borrowed the maximum number of books allowed.");
            }

            //Configure Tanggal Pinjam dan kembali
            DateOnly tanggalPinjam = DateOnly.FromDateTime(DateTime.Now);
            DateOnly tanggalKembali = tanggalPinjam.AddDays(PinjamDuration);

            var borrowBook = new BookBorrow
            {
                AppUserId = userId,
                BookId = bookId,
                TanggalPinjam = tanggalPinjam,
                DueDate = tanggalKembali
            };
            await _bookBorrowRepository.AddBorrowRecord(borrowBook);
        }

        public async Task CheckInBook(string userId, int bookId)
        {
            var PinjamDuration = _configuration.GetValue<int>("LibrarySettings:PinjamDuration");
            var Penalty = _configuration.GetValue<int>("LibrarySettings:PenaltyPerHari");

            //Configure Tanggal Pinjam dan kembali
            DateOnly tanggalPinjam = DateOnly.FromDateTime(DateTime.Now);
            DateOnly DueDate = tanggalPinjam.AddDays(PinjamDuration);
            
            // Fetch the borrow record for the given user and book
            var borrowRecord = _bookBorrowRepository.GetBorrowRecord(userId, bookId);
            if (borrowRecord == null) throw new Exception("Borrow record not found");

            var book = _bookRepository.GetBookById(bookId);
            if (book == null)
            {
                throw new Exception("book not found");
            }

            var tanggalkembali = new DateOnly();
            if (tanggalkembali > DueDate)
            {
            }
            else
            {

            }
            await _bookBorrowRepository.UpdateBorrowRecord(borrowRecord);
        }

        
    }
}
