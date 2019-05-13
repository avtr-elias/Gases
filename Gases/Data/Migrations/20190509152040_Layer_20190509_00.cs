using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Gases.Data.Migrations
{
    public partial class Layer_20190509_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GDataTypeId",
                table: "Layer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GaseId",
                table: "Layer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "VerticalSlice",
                table: "Layer",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Layer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Layer_GDataTypeId",
                table: "Layer",
                column: "GDataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Layer_GaseId",
                table: "Layer",
                column: "GaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Layer_GDataType_GDataTypeId",
                table: "Layer",
                column: "GDataTypeId",
                principalTable: "GDataType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Layer_Gase_GaseId",
                table: "Layer",
                column: "GaseId",
                principalTable: "Gase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Layer_GDataType_GDataTypeId",
                table: "Layer");

            migrationBuilder.DropForeignKey(
                name: "FK_Layer_Gase_GaseId",
                table: "Layer");

            migrationBuilder.DropIndex(
                name: "IX_Layer_GDataTypeId",
                table: "Layer");

            migrationBuilder.DropIndex(
                name: "IX_Layer_GaseId",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "GDataTypeId",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "GaseId",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "VerticalSlice",
                table: "Layer");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Layer");
        }
    }
}
