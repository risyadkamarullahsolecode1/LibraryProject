﻿using LibraryProject.Application.Dtos;
using LibraryProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Interfaces
{
    public interface IBookBorrowService
    {
        Task CheckOutBook(string userId, int bookId);
        Task CheckInBook(string userId, int bookId, DateOnly tanggalKembali);

        //Task<byte[]> GenerateUserReportPdfAsync(string userId);
        //Task<List<OverdueBook>> GetOverdueBooks();
    }
}
