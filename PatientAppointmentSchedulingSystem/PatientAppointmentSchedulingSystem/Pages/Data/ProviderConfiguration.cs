using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;

namespace PatientAppointmentSchedulingSystem.Pages.Data
{
    public class ProviderConfiguration : IEntityTypeConfiguration<ProviderDetails>
    {
        public void Configure(EntityTypeBuilder<ProviderDetails> builder)
        {
            builder.HasData(new List<ProviderDetails> {
                new ProviderDetails
                {
                    ProviderId = 1,
                    ProviderType="Hospital",
                    Name = "Pantai Hospital Ipoh",
                    Description = "",
                    ContactNum = "012-3456789",
                    OwnershipType = "Private",
                    Address= "126, Jalan Tambun, 31400 Ipoh, Perak, Malaysia",
                    State = "Perak",
                    Logo = "",
                    BedCount = 225,
                    Email = "pantaihospitalipoh@gmail.com",
                    Password = "pantaihospitalipoh"
                }
            });
        }
    }
}
