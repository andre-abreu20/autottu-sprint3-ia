using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoTTU.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTelemetriaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Bateria",
                table: "Telemetria",
                newName: "RotacaoMotor");

            migrationBuilder.AddColumn<decimal>(
                name: "NivelCombustivel",
                table: "Telemetria",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Quilometragem",
                table: "Telemetria",
                type: "decimal(5,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NivelCombustivel",
                table: "Telemetria");

            migrationBuilder.DropColumn(
                name: "Quilometragem",
                table: "Telemetria");

            migrationBuilder.RenameColumn(
                name: "RotacaoMotor",
                table: "Telemetria",
                newName: "Bateria");
        }
    }
}
