using Microsoft.EntityFrameworkCore;

namespace PatientAppointmentSchedulingSystem.Pages.Data
{
    //DoctorDbContext used to interated with the tables
    public class DoctorDbContext : DbContext
    {
        public DoctorDbContext(DbContextOptions<DoctorDbContext> options) : base(options) { }

        //A DbSet represent a collection of all entities of a specific type in context & used to query and save data
        //Represent the table for Doctor Details.
        public DbSet<DoctorDetails> Doctor { get; set; }

        public DbSet<AvailabilitySlots> AvailabilitySlots { get; set; }

        public DbSet<ProviderDetails> ProviderDetails { get; set; }
        
        public DbSet<Specialty> SpecialtyDetails {  get; set; }

        public DbSet<PatientDetails> PatientDetails { get; set; }
        public DbSet<ProviderSpecialty> ProviderSpecialties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProviderDetails>().ToTable("Provider");
            modelBuilder.Entity<Specialty>().ToTable("Specialty");
            modelBuilder.Entity<PatientDetails>().ToTable("Patients");
            modelBuilder.Entity<ProviderSpecialty>().ToTable("ProviderSpecialty");

            modelBuilder.Entity<DoctorDetails>()
                .HasKey(d => d.DoctorId); //HasKey specify DoctorId as primary key in DoctorDetails table.

            modelBuilder.Entity<DoctorDetails>()
                .HasOne(d => d.Provider)
                .WithMany(p => p.Doctors)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<AvailabilitySlots>()
            //.HasOne(a => a.Doctor)
            //.WithMany()
            //.HasForeignKey(a => a.DoctorId);
        }
    }
}
