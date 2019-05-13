using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Gases.Data.Migrations
{
    public partial class GData_20190510_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    GDataTypeId = table.Column<int>(nullable: false),
                    GaseId = table.Column<int>(nullable: false),
                    Latitude = table.Column<decimal>(nullable: true),
                    Longtitude = table.Column<decimal>(nullable: true),
                    Month = table.Column<int>(nullable: true),
                    RegionId = table.Column<int>(nullable: true),
                    Season = table.Column<int>(nullable: true),
                    Value = table.Column<decimal>(nullable: true),
                    VerticalSlice = table.Column<decimal>(nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GData_GDataType_GDataTypeId",
                        column: x => x.GDataTypeId,
                        principalTable: "GDataType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GData_Gase_GaseId",
                        column: x => x.GaseId,
                        principalTable: "Gase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GData_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GData_GDataTypeId",
                table: "GData",
                column: "GDataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GData_GaseId",
                table: "GData",
                column: "GaseId");

            migrationBuilder.CreateIndex(
                name: "IX_GData_RegionId",
                table: "GData",
                column: "RegionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GData");
        }
    }
}
