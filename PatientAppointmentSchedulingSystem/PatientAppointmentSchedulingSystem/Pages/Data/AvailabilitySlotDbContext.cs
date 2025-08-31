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
            //map doctor to the existing table
            modelBuilder.Entity<DoctorDetails>()
                .ToTable("Doctor")
                .HasKey(d => d.DoctorId);

            modelBuilder.Entity<AvailabilitySlots>(b =>
            {
                b.ToTable("AvailabilitySlots");
                b.HasKey(s => s.SlotId);


                b.HasOne(s => s.Doctor)
                 .WithMany(d => d.AvailabilitySlots)
                 .HasForeignKey(s => s.DoctorId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            //map availabilitySlots and he FK to DoctorId
            //modelBuilder.Entity<AvailabilitySlots>()
            //    .ToTable("AvailabilitySlots")
            //    .HasKey(a => a.SlotId) //Ensure define a primary key
            //    .Property(s => s.AptStartTime).HasColumnType("time")
            //    .Property(s => s.AptEndTime).HasColumnType("time")
            //    .Property(s => s.AppointmentDate).HasColumnType("date");


            //modelBuilder.Entity<AvailabilitySlots>()
            //    .HasOne(a =>a.Doctor)
            //    .WithMany(d => d.AvailabilitySlots)
            //    .HasForeignKey(a => a.DoctorId) //FK to DoctorDetails
            //    .HasPrincipalKey(d => d.DoctorId)
            //    .OnDelete(DeleteBehavior.Cascade);

            



            //modelBuilder.Entity<AvailabilitySlots>()
            //    .HasOne<PatientDetails>()
            //    .WithMany()
            //    .HasForeignKey(a => a.PatientId); //FK to PatientDetails
        }
    }
}
