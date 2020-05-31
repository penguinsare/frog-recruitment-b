using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using crm.Models;
using crm.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using crm.Constants;

namespace crm.Data
{
    public class CrmContext: IdentityDbContext<ApplicationUser>
    {
        public CrmContext()
            : base()
        {
        }
        public CrmContext(DbContextOptions<CrmContext> options)
            : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Client> CrmClients { get; set; }
        public DbSet<Recruiter> Recruiters { get; set; }
        public DbSet<CandidateStatus> CandidateStatuses { get; set; }
        public DbSet<JobStatus> JobStatuses { get; set; }
        //public DbSet<Document> JobDocuments { get; set; }
        //public DbSet<Document> CandidateDocuments { get; set; }
        //public DbSet<Document> ClientDocuments { get; set; }
        public DbSet<FileRepresentationInDatabase> Documents { get; set; }
        //public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }
        public DbSet<CandidateSentToIntrerview> CandidatesSentToInterview { get; set; }
        public DbSet<UserPrivilege> UserPrivileges { get; set; }
        //public DbSet<Placement> Placements { get; set; }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{

        //    //optionsBuilder.UseSqlServer($"server={_server};port={_port};database={_database};user={_user};password={_password};SslMode={_sslMode};");
        //    //optionsBuilder.UseSqlServer("Data Source=crm-db-dev.database.windows.net;Initial Catalog=crm;persist security info=True;User id=petkov;Password=K0liogonqm4");

        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // modelBuilder.Entity<Job>()
            //.HasData(
            //    new Job() { JobId = 41, Title = "Experienced Programmer", Client = "Sam Harris",  Recruiter = "", Description = "silencium" },
            //    new Job() { JobId = 5, Title = "Truck driver", Client = "Sam Harris",  Recruiter = "", },
            //    new Job() { JobId = 22, Title = "Maid", Client = "Sam Harris",  Recruiter = "", },
            //    new Job() { JobId = 4, Title = "Business Intelligence Analyst", Client = "Sam Harris",  Recruiter = "", Description = "" },
            //    new Job() { JobId = 431, Title = "Recruiter", Client = "Sam Harris",  Recruiter = "", Description = "" },
            //    new Job() { JobId = 45, Title = "Book Keeper", Client = "Sam Harris",  Recruiter = "", Description = "" },
            //    new Job() { JobId = 48, Title = "Chef", Client = "Sam Harris",  Recruiter = "", Description = "" },
            //    new Job() { JobId = 46, Title = "Manager Outsourcing", Client = "Sam Harris",  Recruiter = "", Description = "" },
            //    new Job() { JobId = 43, Title = "Bartender",  Recruiter = "", Description = "" },
            //    new Job() { JobId = 66, Title = "Barista", Client = "Sam Harris",  Recruiter = "", Description = "" },
            //    new Job() { JobId = 472, Title = "Blogger", Client = "Sam Harris",  Recruiter = "", Description = "" },
            //    new Job() { JobId = 90, Title = "Influencer", Client = "Sam Harris",  Recruiter = "", Description = "" },
            //    new Job() { JobId = 100, Title = "Monkey Trainer", Client = "Sam Harris",  Recruiter = "", Description = "" },
            //    new Job() { JobId = 711, Title = "Construction Worker", Client = "Sam Harris",  Recruiter = "", Description = "" },
            //    new Job() { JobId = 9, Title = "Teacher", Client = "Sam Harris",  Recruiter = "", Description = "" });

            //modelBuilder.Entity<Job>()
            //    .HasOne(j => j.Candidate).WithOne(c => c.Job)
            //    .HasForeignKey<Candidate>(e => e.JobId);
            //modelBuilder.Entity<Candidate>()
            //    .HasOne(c => c.Job).WithOne(j => j.Candidate)
            //    .HasForeignKey<Job>(e => e.CandidateId);
            //modelBuilder.Entity<Candidate>()
            //.HasOne(c => c.CurrentJob);
            //modelBuilder.Entity<JobStatus>()
            //    .HasKey(j => j.JobStatusId);

            //modelBuilder.Entity<JobStatus>()
            //    .HasData(
            //        new JobStatus() { JobStatusId = 1, Status = JobStatusList.FirstInterview },
            //        new JobStatus() { JobStatusId = 2, Status = JobStatusList.SecondInterview },
            //        new JobStatus() { JobStatusId = 3, Status = JobStatusList.FinalInterview },
            //        new JobStatus() { JobStatusId = 4, Status = JobStatusList.Offer },
            //        new JobStatus() { JobStatusId = 5, Status = JobStatusList.Reject }
            //    );
                

            

            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.Job)
                .WithOne(j => j.AssignedCandidate)
                //.HasForeignKey<Candidate>(c => c.JobId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.CandidateStatus)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.Recruiter)
                .WithMany(r => r.Candidates)
                .HasForeignKey(c => c.RecruiterId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Candidate>()
                .HasMany(c => c.Documents)
                .WithOne(f => f.Candidate)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Candidate>()
                .HasMany(c => c.AppliedForJobs)
                .WithOne(j => j.Candidate)
                .HasForeignKey(j => j.CandidateId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Job>()
                .HasOne(j => j.Client)
                .WithMany()
                .HasForeignKey(j => j.ClientId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Recruiter)
                .WithMany()
                .HasForeignKey(j => j.RecruiterId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Job>()
                .HasOne(j => j.JobStatus)
                .WithMany()
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Job>()
                .HasMany(c => c.Documents)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Job>()
                .HasMany(j => j.CandidatesSent)
                .WithOne(cs => cs.Job)
                .HasForeignKey(cs => cs.JobId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Client>()
                .HasOne(cl => cl.Recruiter)
                .WithMany(r => r.Clients)
                .HasForeignKey(cl => cl.RecruiterId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Documents)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<CandidateSentToIntrerview>()
            //    .HasOne(cs => cs.Candidate)
            //    .WithMany()
            //    .HasForeignKey(cs => cs.CandidateId)
            //    .IsRequired(true)
            //    .OnDelete(DeleteBehavior.Cascade);
            //modelBuilder.Entity<CandidateSentToIntrerview>()
            //    .HasOne(cs => cs.Recruiter)
            //    .WithMany()
            //    .HasForeignKey(cs => cs.RecruiterId)
            //    .IsRequired(true)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Placement>()
            //    .HasOne(pl => pl.Job)
            //    .WithOne(j => j.Placement)
            //    .HasForeignKey(pl => pl.JobId)
            //    .IsRequired(false)
            //    .OnDelete(DeleteBehavior.SetNull);
            //modelBuilder.Entity<Placement>()
            //    .HasOne(pl => pl.Candidate)
            //    .WithMany(c => c.Placements)
            //    .HasForeignKey(pl => pl.CandidateId)
            //    .IsRequired(true)
            //    .OnDelete(DeleteBehavior.Cascade);
            //modelBuilder.Entity<FileRepresentationInDatabase>()
            //    .HasOne(fr => fr.Recruiter)
            //    .WithMany()
            //    .HasForeignKey(fr => fr.RecruiterId)
            //    .IsRequired(true)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserPrivilege>()
                .HasData(
                    new UserPrivilege() { UserPrivilegeId = 1, Value = DefinedClaimAccessValues.Normal},
                    new UserPrivilege() { UserPrivilegeId = 2, Value = DefinedClaimAccessValues.Elevated}
                );
            //modelBuilder.Entity<Job>()
            //    .HasOne(j => j.Candidate)
            //    .WithOne(c => c.Job)
            //    //.HasForeignKey<Job>(c => c.CandidateId)
            //    .IsRequired(false)
            //    .OnDelete(DeleteBehavior.SetNull);

            //modelBuilder.Entity<User>()
            //    .HasKey(u => u.Username);

            //modelBuilder.Entity<IdentityRole>()
            //    .HasData(
            //        new IdentityRole(DefinedRoles.Boss) { NormalizedName = DefinedRoles.Boss.ToUpper()},
            //        new IdentityRole(DefinedRoles.Employee) { NormalizedName = DefinedRoles.Employee.ToUpper() }
            //    );
            //    modelBuilder.Entity<User>().Property<Username>.
        }

        internal bool Where(Func<object, bool> p)
        {
            throw new NotImplementedException();
        }
    }
}
