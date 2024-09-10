using LibraryProject.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Infrastructure.Data
{
    public partial class LibraryProjectContext:IdentityDbContext<AppUser>
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowSequence> WorkflowSequences { get; set; }
        public DbSet<NextStepRule> NextStepRules { get; set; }
        public DbSet<BookRequest> BookRequests { get; set; }
        public DbSet<Process> Processs { get; set; }
        public DbSet<WorkflowAction> WorkflowActions { get; set; }
        public DbSet<BookBorrow> BookBorrows { get; set; }

        public LibraryProjectContext(DbContextOptions<LibraryProjectContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Workflow>()
           .HasMany(w => w.WorkflowSequence)
           .WithOne(ws => ws.Workflow)
           .HasForeignKey(ws => ws.WorkflowId);

            modelBuilder.Entity<WorkflowSequence>()
                .HasOne(ws => ws.RequiredRole)
                .WithMany()
                .HasForeignKey(ws => ws.RequiredRoleId);

            modelBuilder.Entity<Process>()
                .HasOne(p => p.Workflow)
                .WithMany(w => w.Processes)
                .HasForeignKey(p => p.WorkflowId);

            modelBuilder.Entity<Process>()
                .HasOne(p => p.Requester)
                .WithMany(u => u.Processes)
                .HasForeignKey(p => p.RequesterId);

            modelBuilder.Entity<NextStepRule>()
                .HasOne(nsr => nsr.CurrentStep)
                .WithMany(ws => ws.NextStepRules)
                .HasForeignKey(nsr => nsr.CurrentStepId);

            modelBuilder.Entity<NextStepRule>()
                .HasOne(nsr => nsr.NextStep)
                .WithMany()
                .HasForeignKey(nsr => nsr.NextStepId);

            modelBuilder.Entity<BookRequest>()
                .HasKey(lr => lr.RequestId);

            modelBuilder.Entity<BookRequest>()
                .HasOne(rb => rb.Process)
                .WithMany(p => p.BookRequests)
                .HasForeignKey(rb => rb.ProcessId);

            modelBuilder.Entity<BookRequest>()
                .HasOne(rb => rb.AppUser)
                .WithMany(u => u.BookRequests)
                .HasForeignKey(rb => rb.AppUserId);

            modelBuilder.Entity<WorkflowAction>()
                .HasOne(wa => wa.Process)
                .WithMany(rb => rb.WorkflowActions)
                .HasForeignKey(wa => wa.ProcessId);

            modelBuilder.Entity<WorkflowAction>()
                .HasOne(wa => wa.Step)
                .WithMany(ws => ws.WorkflowActions)
                .HasForeignKey(wa => wa.StepId);

            modelBuilder.Entity<WorkflowAction>()
                .HasOne(wa => wa.Actor)
                .WithMany(u => u.WorkflowActions)
                .HasForeignKey(wa => wa.ActorId);

            modelBuilder.Entity<NextStepRule>()
                .HasKey(lr => lr.RuleId);
        }
    }
}
