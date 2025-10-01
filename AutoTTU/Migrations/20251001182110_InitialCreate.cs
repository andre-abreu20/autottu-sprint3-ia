using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoTTU.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Checkin",
                columns: table => new
                {
                    IdCheckin = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    IdMoto = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IdUsuario = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AtivoChar = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false),
                    Observacao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ImagensUrl = table.Column<string>(type: "NCLOB", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkin", x => x.IdCheckin);
                });

            migrationBuilder.CreateTable(
                name: "Motos",
                columns: table => new
                {
                    IdMoto = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Modelo = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Marca = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Ano = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Placa = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    AtivoChar = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false),
                    FotoUrl = table.Column<string>(type: "NCLOB", maxLength: 2048, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(11,8)", nullable: true),
                    Endereco = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    UltimaAtualizacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motos", x => x.IdMoto);
                });

            migrationBuilder.CreateTable(
                name: "Slot",
                columns: table => new
                {
                    IdSlot = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    IdMoto = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AtivoChar = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slot", x => x.IdSlot);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Senha = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Telemetria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DeviceId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(11,8)", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Temperatura = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Velocidade = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Quilometragem = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    NivelCombustivel = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RotacaoMotor = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    Observacoes = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telemetria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Telemetria_Motos_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Motos",
                        principalColumn: "IdMoto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Telemetria_DeviceId",
                table: "Telemetria",
                column: "DeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Checkin");

            migrationBuilder.DropTable(
                name: "Slot");

            migrationBuilder.DropTable(
                name: "Telemetria");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Motos");
        }
    }
}
