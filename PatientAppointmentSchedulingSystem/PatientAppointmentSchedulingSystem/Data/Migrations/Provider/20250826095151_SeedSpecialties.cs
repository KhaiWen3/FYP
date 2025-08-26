using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PatientAppointmentSchedulingSystem.Data.Migrations.Provider
{
    /// <inheritdoc />
    public partial class SeedSpecialties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Provider",
                columns: table => new
                {
                    ProviderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnershipType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BedCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provider", x => x.ProviderId);
                });

            migrationBuilder.CreateTable(
                name: "Specialty",
                columns: table => new
                {
                    SpecialtyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecialtyName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialty", x => x.SpecialtyId);
                });

            migrationBuilder.CreateTable(
                name: "ProviderSpecialties",
                columns: table => new
                {
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    SpecialtyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderSpecialties", x => new { x.ProviderId, x.SpecialtyId });
                    table.ForeignKey(
                        name: "FK_ProviderSpecialties_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "ProviderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderSpecialties_Specialty_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialty",
                        principalColumn: "SpecialtyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Specialty",
                columns: new[] { "SpecialtyId", "SpecialtyName" },
                values: new object[,]
                {
                    { 1, "Cardiology" },
                    { 2, "Internal Medicine" },
                    { 3, "Family Medicine" },
                    { 4, "Pediatrics" },
                    { 5, "Obstetrics & Gynecology" },
                    { 6, "Orthopedic Surgery" },
                    { 7, "General Surgery" },
                    { 8, "Neurology" },
                    { 9, "Psychiatry" },
                    { 10, "Dermatology" },
                    { 11, "Ophthalmology" },
                    { 12, "Otorhinolaryngology (ENT)" },
                    { 13, "Radiology (Diagnostic)" },
                    { 14, "Emergency Medicine" },
                    { 15, "Anesthesiology" },
                    { 16, "Urology" },
                    { 17, "Nephrology" },
                    { 18, "Endocrinology" },
                    { 19, "Gastroenterology" },
                    { 20, "Pulmonology (Respiratory)" },
                    { 21, "Oncology (Medical)" },
                    { 22, "Hematology" },
                    { 23, "Rheumatology" },
                    { 24, "Infectious Diseases" },
                    { 25, "Geriatrics" },
                    { 26, "Physical Medicine & Rehabilitation" },
                    { 27, "Pathology" },
                    { 28, "Nuclear Medicine" },
                    { 29, "Plastic Surgery" },
                    { 30, "Neurosurgery" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderSpecialties_SpecialtyId",
                table: "ProviderSpecialties",
                column: "SpecialtyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderSpecialties");

            migrationBuilder.DropTable(
                name: "Provider");

            migrationBuilder.DropTable(
                name: "Specialty");
        }
    }
}
