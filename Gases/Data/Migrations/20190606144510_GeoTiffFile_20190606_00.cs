using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Gases.Data.Migrations
{
    public partial class GeoTiffFile_20190606_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeoTiffFile_Gase_GaseId",
                table: "GeoTiffFile");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "GeoTiffFile");

            migrationBuilder.AlterColumn<int>(
                name: "GaseId",
                table: "GeoTiffFile",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<decimal>(
                name: "VerticalSlice",
                table: "GeoTiffFile",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GeoTiffFile_Gase_GaseId",
                table: "GeoTiffFile",
                column: "GaseId",
                principalTable: "Gase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeoTiffFile_Gase_GaseId",
                table: "GeoTiffFile");

            migrationBuilder.DropColumn(
                name: "VerticalSlice",
                table: "GeoTiffFile");

            migrationBuilder.AlterColumn<int>(
                name: "GaseId",
                table: "GeoTiffFile",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Month",
                table: "GeoTiffFile",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GeoTiffFile_Gase_GaseId",
                table: "GeoTiffFile",
                column: "GaseId",
                principalTable: "Gase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
