using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Gases.Data.Migrations
{
    public partial class Layer_20180906_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Layer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    FileNameWithPath = table.Column<string>(nullable: true),
                    GeoServerName = table.Column<string>(nullable: true),
                    GeoServerStyle = table.Column<string>(nullable: true),
                    NameEN = table.Column<string>(nullable: true),
                    NameKK = table.Column<string>(nullable: true),
                    NameRU = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layer", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Layer");
        }
    }
}
