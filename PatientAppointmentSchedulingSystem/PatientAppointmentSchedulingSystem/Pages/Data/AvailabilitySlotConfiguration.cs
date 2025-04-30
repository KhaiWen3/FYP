using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PatientAppointmentSchedulingSystem.Pages.Data
{
    public class AvailabilitySlotConfiguration
    {
        public void Configure(EntityTypeBuilder<AvailabilitySlots> builder)
        {
            builder.HasData(new List<AvailabilitySlots> {
                //new AvailabilitySlots
                //{
                //    SlotId = 1,
                //    StartTime = '01-12-2024 09:00:00',
                //    EndTime = '2023-12-01 09:30:00',
                //    IsBooked = 1,
                //    AppointmentType = "In-person Visit"
                //}
            });
        }
    }
}
