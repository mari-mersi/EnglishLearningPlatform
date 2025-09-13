using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishLearningPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToWord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DifficultyLevel",
                table: "Words",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Example",
                table: "Words",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PartOfSpeech",
                table: "Words",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Transcription",
                table: "Words",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DifficultyLevel", "Example", "PartOfSpeech", "Transcription" },
                values: new object[] { 1, "", "", "" });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DifficultyLevel", "Example", "PartOfSpeech", "Transcription" },
                values: new object[] { 1, "", "", "" });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DifficultyLevel", "Example", "PartOfSpeech", "Transcription" },
                values: new object[] { 1, "", "", "" });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DifficultyLevel", "Example", "PartOfSpeech", "Transcription" },
                values: new object[] { 1, "", "", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "Example",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "PartOfSpeech",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "Transcription",
                table: "Words");
        }
    }
}
