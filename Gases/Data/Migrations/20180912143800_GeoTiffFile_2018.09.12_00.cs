using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Gases.Data.Migrations
{
    public partial class GeoTiffFile_20180912_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "GeoTiffFile",
                newName: "Year");

            migrationBuilder.AddColumn<string>(
                name: "Month",
                table: "GeoTiffFile",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "GeoTiffFile");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "GeoTiffFile",
                newName: "Date");
        }
    }
}
