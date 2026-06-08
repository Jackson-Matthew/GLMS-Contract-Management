using GLMS_ST10445830.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS_ST10445830.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        // application db context for the project.
        //constructor that takes the perameter options 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // the tables for the database are made here , set as DbSet 
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        //determines the rules for the realtionships between the tables that are set above

        //classes inherit from the dbcontext
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //relationships
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(c => c.Contracts)
                .HasForeignKey(c => c.ClientId);
           
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(r => r.Contract)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(r => r.ContractId);

            modelBuilder.Entity<ServiceRequest>()
                .Property(s => s.USDAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ServiceRequest>()
                .Property(s => s.ZARCost)
                .HasColumnType("decimal(18,2)");
        }
    }
}
