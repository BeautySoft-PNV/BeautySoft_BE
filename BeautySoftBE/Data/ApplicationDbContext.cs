using Microsoft.EntityFrameworkCore;
using BeautySoftBE.Models;

namespace BeautySoftBE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RoleModel> Roles { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<MakeupItemModel> MakeupItems { get; set; }
        public DbSet<MakeupStyleModel> MakeupStyles { get; set; }
        public DbSet<MakeupItemStyleModel> MakeupItemStyles { get; set; }
        public DbSet<TypeStorageModel> TypeStorages { get; set; }
        public DbSet<ManagerStorageModel> ManagerStorages { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }
        public DbSet<NotificationHistoryModel> NotificationHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MakeupItemModel>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MakeupStyleModel>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MakeupStyleModel>()
                .HasMany(e => e.MakeupItemStyles)
                .WithOne(e => e.MakeupStyle)
                .HasForeignKey(e => e.MakeupStyleId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MakeupItemModel>()
                .HasMany(e => e.MakeupItemStyles)
                .WithOne(e => e.MakeupItem)
                .HasForeignKey(e => e.MakeupItemId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<ManagerStorageModel>()
                .HasOne(ms => ms.User)
                .WithMany()
                .HasForeignKey(ms => ms.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ManagerStorageModel>()
                .HasOne(ms => ms.TypeStorage)
                .WithMany()
                .HasForeignKey(ms => ms.TypeStorageId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PaymentModel>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PaymentModel>()
                .HasOne(p => p.TypeStorage)
                .WithMany()
                .HasForeignKey(p => p.TypeStorageId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<NotificationHistoryModel>()
                .HasOne(nh => nh.User)
                .WithMany()
                .HasForeignKey(nh => nh.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<NotificationHistoryModel>()
                .HasOne(nh => nh.Notification)
                .WithMany(n => n.NotificationHistories)
                .HasForeignKey(nh => nh.NotificationId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}