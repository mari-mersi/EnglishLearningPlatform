using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishLearningPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddWordStatisticsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ConfidenceScore",
                table: "Words",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReviewed",
                table: "Words",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "Words",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConfidenceScore", "LastReviewed", "ReviewCount" },
                values: new object[] { 0.0, null, 0 });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConfidenceScore", "LastReviewed", "ReviewCount" },
                values: new object[] { 0.0, null, 0 });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConfidenceScore", "LastReviewed", "ReviewCount" },
                values: new object[] { 0.0, null, 0 });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConfidenceScore", "LastReviewed", "ReviewCount" },
                values: new object[] { 0.0, null, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfidenceScore",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "LastReviewed",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "Words");
        }
    }
}
