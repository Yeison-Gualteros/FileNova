using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Repository.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = "4ac8240a-8498-4869-bc86-60e5dc982d27",
                    Name = "lider cumplimiento",
                    NormalizedName = "LIDER CUMPLIMIENTO",
                    ConcurrencyStamp = "11111111-1111-1111-1111-111111111111"
                },
                new IdentityRole
                {
                    Id = "562419f5-eed1-473b-bcc1-9f2dbab182b4",
                    Name = "Administrador",
                    NormalizedName = "ADMINISTRADOR",
                    ConcurrencyStamp = "22222222-2222-2222-2222-222222222222"
                }
            );
        }
    }
}
