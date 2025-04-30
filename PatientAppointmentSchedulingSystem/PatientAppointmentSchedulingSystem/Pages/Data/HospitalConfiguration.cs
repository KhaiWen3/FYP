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
                    HospitalType = "Private",
                    HospitalAdress= "126, Jalan Tambun, 31400 Ipoh, Perak, Malaysia",
                    HospitalLogo = "",
                    HospitalPrimaryContactFirstName = "Khai Wen",
                    HospitalPrimaryContactLastName = "Ong",
                    HospitalPrimaryContactPosition = "Hospital Administrator",
                    HospitalPrimaryContactPhoneNum = "012-4816725",
                    HospitalServices = "Emergency Medicine",
                    HospitalBed = 225,
                    HospitalEmail = "pantaihospitalipoh@gmail.com",
                    HospitalPassword = "pantaihospitalipoh"
                }
            });
        }
    }
}
