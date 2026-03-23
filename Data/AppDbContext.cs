using Microsoft.EntityFrameworkCore;
using SmartQueueAPI.Entities;

namespace SmartQueueAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<QueueConfiguration> QueueConfigurations => Set<QueueConfiguration>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Priority)
                .HasDefaultValue(1);

            modelBuilder.Entity<Ticket>()
                .Property(t => t.Status)
                .HasDefaultValue(TicketStatus.Waiting);

            modelBuilder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasDefaultValue(AppointmentStatus.Booked);
        }
    }
}
