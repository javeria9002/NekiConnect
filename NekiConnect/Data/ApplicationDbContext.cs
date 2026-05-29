using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NekiConnect.Models;

namespace NekiConnect.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<NGO> NGOs { get; set; }
        public DbSet<Fundraising> Fundraisings { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<VolunteerApplication> VolunteerApplications { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BeneficiaryRecord> BeneficiaryRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EventRsvp> EventRsvps { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Donation → Fundraising (nullable)
            builder.Entity<Donation>()
                .HasOne(d => d.Fundraising).WithMany(f => f.Donations)
                .HasForeignKey(d => d.FundraisingId)
                .OnDelete(DeleteBehavior.Restrict).IsRequired(false);

            // Donation → Campaign (nullable)
            builder.Entity<Donation>()
                .HasOne(d => d.Campaign).WithMany(c => c.Donations)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Restrict).IsRequired(false);

            // Donation → User (restrict so deleting user keeps donation records)
            builder.Entity<Donation>()
                .HasOne(d => d.Donor).WithMany()
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            // VolunteerApplication → User
            builder.Entity<VolunteerApplication>()
                .HasOne(v => v.User).WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // VolunteerApplication → Campaign
            builder.Entity<VolunteerApplication>()
                .HasOne(v => v.Campaign).WithMany(c => c.VolunteerApplications)
                .HasForeignKey(v => v.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            // EventRsvp → User
            builder.Entity<EventRsvp>()
                .HasOne(r => r.User).WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // EventRsvp → Campaign
            builder.Entity<EventRsvp>()
                .HasOne(r => r.Campaign).WithMany()
                .HasForeignKey(r => r.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification → User
            builder.Entity<Notification>()
                .HasOne(n => n.User).WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // NGO cascades
            builder.Entity<Fundraising>()
                .HasOne(f => f.NGO).WithMany(n => n.Fundraisings)
                .HasForeignKey(f => f.NgoId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Campaign>()
                .HasOne(c => c.NGO).WithMany(n => n.Campaigns)
                .HasForeignKey(c => c.NgoId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BlogPost>()
                .HasOne(b => b.NGO).WithMany(n => n.BlogPosts)
                .HasForeignKey(b => b.NgoId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BeneficiaryRecord>()
                .HasOne(br => br.NGO).WithMany(n => n.BeneficiaryRecords)
                .HasForeignKey(br => br.NgoId).OnDelete(DeleteBehavior.Cascade);

            // Decimal precision
            builder.Entity<Fundraising>().Property(f => f.GoalAmount).HasPrecision(18, 2);
            builder.Entity<Fundraising>().Property(f => f.RaisedAmount).HasPrecision(18, 2);
            builder.Entity<Campaign>().Property(c => c.GoalAmount).HasPrecision(18, 2);
            builder.Entity<Campaign>().Property(c => c.RaisedAmount).HasPrecision(18, 2);
            builder.Entity<Donation>().Property(d => d.Amount).HasPrecision(18, 2);
        }
    }
}