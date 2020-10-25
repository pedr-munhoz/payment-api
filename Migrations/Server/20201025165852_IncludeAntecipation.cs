using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace payment_api.Migrations.Server
{
    public partial class IncludeAntecipation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AntecipationAnalysis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FinalStatus = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AntecipationAnalysis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Antecipations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SolicitationDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AnalysisId = table.Column<int>(type: "integer", nullable: true),
                    SolicitedValue = table.Column<double>(type: "double precision", nullable: false),
                    AntecipatedValue = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Antecipations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Antecipations_AntecipationAnalysis_AnalysisId",
                        column: x => x.AnalysisId,
                        principalTable: "AntecipationAnalysis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransactionDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AprovalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CancelDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Anticipated = table.Column<bool>(type: "boolean", nullable: true),
                    Approved = table.Column<bool>(type: "boolean", nullable: false),
                    RawValue = table.Column<double>(type: "double precision", nullable: false),
                    LiquidValue = table.Column<double>(type: "double precision", nullable: false),
                    Tax = table.Column<double>(type: "double precision", nullable: false),
                    CreditCard = table.Column<string>(type: "text", nullable: true),
                    AntecipationEntityId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Antecipations_AntecipationEntityId",
                        column: x => x.AntecipationEntityId,
                        principalTable: "Antecipations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentInstallmentEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentId = table.Column<int>(type: "integer", nullable: false),
                    RawValue = table.Column<double>(type: "double precision", nullable: false),
                    LiquidValue = table.Column<double>(type: "double precision", nullable: false),
                    InstallmentNumber = table.Column<int>(type: "integer", nullable: false),
                    AnticipatedValue = table.Column<double>(type: "double precision", nullable: true),
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
                name: "IX_Antecipations_AnalysisId",
                table: "Antecipations",
                column: "AnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInstallmentEntity_PaymentEntityId",
                table: "PaymentInstallmentEntity",
                column: "PaymentEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AntecipationEntityId",
                table: "Payments",
                column: "AntecipationEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentInstallmentEntity");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Antecipations");

            migrationBuilder.DropTable(
                name: "AntecipationAnalysis");
        }
    }
}
