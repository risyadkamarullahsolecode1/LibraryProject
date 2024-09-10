using LibraryProject.Application.Interfaces;
using LibraryProject.Domain.Entities;
using LibraryProject.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace LibraryProject.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(IUserRepository userRepository, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task AttachNotes(int id, string notes)
        {
            var note = await _userRepository.GetUserById(id);
            if (note == null)
            {
                throw new Exception($"user with Id {id} not found");
            }
            note.Note = notes;

            _userRepository.UpdateUser(note);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<byte[]> generatereportpdf()
        {
            var userList = await _userRepository.GetAllUser();

            string htmlcontent = String.Empty;
            htmlcontent += "<h1> User Report </h1>";
            htmlcontent += "<table>";
            htmlcontent += "<thead><tr><td>Id</td><td>Username</td><td>Position</td><td>Previllage</td><td>AppUserId</td></tr></thead>";

            userList.ToList().ForEach(item => {;
                htmlcontent += "<tr>";
                htmlcontent += "<td>" + item.Id + "</td>";
                htmlcontent += "<td>" + item.FirstName + " " + item.LastName + "</td>";
                htmlcontent += "<td>" + item.Position + "</td>";
                htmlcontent += "<td>" + item.Previlege + "</td>";
                htmlcontent += "<td>" + item.AppUserId + "</td>";
                htmlcontent += "</tr>";
            });
            htmlcontent += "</table>";

            var document = new PdfDocument();

            var config = new PdfGenerateConfig();

            config.PageOrientation = PageOrientation.Landscape;

            config.PageSize = PageSize.A4;

            string cssStr = File.ReadAllText(@"./Template/ReportTemplates/style.css");

            CssData css = PdfGenerator.ParseStyleSheet(cssStr);

            PdfGenerator.AddPdfPages(document, htmlcontent, config, css);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, false);

            byte[] bytes = stream.ToArray();

            return bytes;
        }
    }
}
