using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PatientAppointmentSchedulingSystem.Pages.Data
{
    public class AvailabilitySlotConfiguration : IEntityTypeConfiguration<AvailabilitySlots>
    {
        public void Configure(EntityTypeBuilder<AvailabilitySlots> builder)
        {
            builder.HasData(new List<AvailabilitySlots> {
                new AvailabilitySlots
                {
                    SlotId = 1,
                    DoctorId = 1,
                    AppointmentDate = new DateTime(2024, 12, 1),
                    AptStartTime = new DateTime(2024, 12, 1, 9, 0, 0),
                    AptEndTime = new DateTime(2024, 12, 1, 9, 30, 0),
                    AppointmentStatus = 0,
                    AppointmentType = "In-person",
                    PatientId = null
                }
            });
        }
    }
}
