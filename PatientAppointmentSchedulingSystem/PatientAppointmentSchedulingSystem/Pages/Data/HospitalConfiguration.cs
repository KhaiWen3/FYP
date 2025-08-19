using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;

namespace PatientAppointmentSchedulingSystem.Pages.Data
{
    public class HospitalConfiguration
    {
        public void Configure(EntityTypeBuilder<HospitalDetails> builder)
        {
            builder.HasData(new List<HospitalDetails> {
                new HospitalDetails
                {
                    HospitalId = 1,
                    HospitalName = "Pantai Hospital Ipoh",
                    HospitalDescription = "",
                    HospitalEstablishedYear = 2022,
                    HospitalContactNum = "012-3456789",
                    HospitalType = "Private",
                    HospitalAddress= "126, Jalan Tambun, 31400 Ipoh, Perak, Malaysia",
                    HospitalState = "Perak",
                    HospitalLogo = "",
                    HospitalServices = "Emergency Medicine",
                    HospitalBedCount = 225,
                    HospitalEmail = "pantaihospitalipoh@gmail.com",
                    HospitalPassword = "pantaihospitalipoh"
                }
            });
        }
    }
}
