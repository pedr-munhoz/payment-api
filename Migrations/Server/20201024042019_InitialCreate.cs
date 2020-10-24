using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace payment_api.Migrations.Server
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AprovalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CancelDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Anticipated = table.Column<bool>(type: "boolean", nullable: false),
                    Approved = table.Column<bool>(type: "boolean", nullable: false),
                    RawValue = table.Column<float>(type: "real", nullable: false),
                    LiquidValue = table.Column<float>(type: "real", nullable: false),
                    Tax = table.Column<float>(type: "real", nullable: false),
                    CreditCard = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentInstallmentEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentId = table.Column<int>(type: "integer", nullable: false),
                    RawValue = table.Column<float>(type: "real", nullable: false),
                    LiquidValue = table.Column<float>(type: "real", nullable: false),
                    InstallmentNumber = table.Column<int>(type: "integer", nullable: false),
                    AnticipatedValue = table.Column<float>(type: "real", nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AntecipatedTranfer = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PaymentEntityId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentInstallmentEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentInstallmentEntity_Payments_PaymentEntityId",
                        column: x => x.PaymentEntityId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInstallmentEntity_PaymentEntityId",
                table: "PaymentInstallmentEntity",
                column: "PaymentEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentInstallmentEntity");

            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
