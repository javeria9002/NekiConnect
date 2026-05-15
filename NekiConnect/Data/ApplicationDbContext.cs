using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NekiConnect.Models;

namespace NekiConnect.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<NGO> NGOs { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<VolunteerApplication> VolunteerApplications { get; set; }
    }
}