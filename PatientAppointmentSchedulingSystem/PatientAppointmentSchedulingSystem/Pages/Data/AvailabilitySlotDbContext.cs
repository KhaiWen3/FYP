using Microsoft.EntityFrameworkCore;

namespace PatientAppointmentSchedulingSystem.Pages.Data
{
    public class AvailabilitySlotDbContext : DbContext
    {
        public AvailabilitySlotDbContext(DbContextOptions<AvailabilitySlotDbContext> options) : base(options) { }

        public DbSet<AvailabilitySlots> AvailabilitySlots { get; set; }

        //public DbSet<PatientDetails> PatientDetails { get; set; }
        public DbSet<DoctorDetails> Doctor { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AvailabilitySlots>()
                .HasKey(a => a.SlotId); //Ensure define a primary key

            modelBuilder.Entity<AvailabilitySlots>()
                .HasOne(a =>a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId); //FK to DoctorDetails

            //modelBuilder.Entity<AvailabilitySlots>()
            //    .HasOne<PatientDetails>()
            //    .WithMany()
            //    .HasForeignKey(a => a.PatientId); //FK to PatientDetails
        }
    }
}
