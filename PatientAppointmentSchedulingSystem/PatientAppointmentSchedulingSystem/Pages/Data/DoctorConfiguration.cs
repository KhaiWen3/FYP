using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PatientAppointmentSchedulingSystem.Pages.Data
{
    public class DoctorConfiguration
    {
        public void Configure(EntityTypeBuilder<DoctorDetails> builder)
        {
            builder.HasData(new List<DoctorDetails> {
                new DoctorDetails
                {
                    DoctorId = 1,
                    DoctorName = "Goh Jin Xing",
                    DoctorPhoneNum = "016-9801560",
                    DoctorSpeciality = "Family Medicine",
                    DoctorEmail = "gjx@gmail.com",
                    DoctorPassword= "gjx"
                }
            });
        }
    }
}
