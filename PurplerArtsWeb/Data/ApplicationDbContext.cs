using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PurplerArtsWeb.Models.SubmissionApplication;

namespace PurplerArtsWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SubmissionApplication> SubmissionApplicationModel { get; set; }
    }
}