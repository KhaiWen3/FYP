using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PatientAppointmentSchedulingSystem.Pages.Data
{
    public class PatientConfiguration :IEntityTypeConfiguration<PatientDetails>
    {
        public void Configure(EntityTypeBuilder<PatientDetails> builder)
        {
            builder.HasData(new List<PatientDetails> {
                //new PatientDetails
                //{
                //    PatientId = 1,
                //    PatientFirstName = "Khai Wen",
                //    PatientLastName = "Ong",
                //    PatientPhoneNum = "012-4816725",
                //    PatientAge = 22,
                //    PatientEmail= "okw@gmail.com",
                //    PatientPassword = "Khai Wen"
                //},
                //new PatientDetails
                //{
                //    PatientId = 2,
                //    PatientFirstName = "Jin Xing",
                //    PatientLastName = "Goh",
                //    PatientPhoneNum = "016-9801560",
                //    PatientAge = 22,
                //    PatientEmail= "gjx@gmail.com",
                //    PatientPassword = "Jin Xing"
                //}
            });
        }
    }
}
