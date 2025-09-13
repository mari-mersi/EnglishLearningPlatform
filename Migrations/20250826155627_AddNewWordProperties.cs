using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishLearningPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddNewWordProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Example", "PartOfSpeech", "Transcription" },
                values: new object[] { "Hello, how are you?", "interjection", "/həˈloʊ/" });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Example", "PartOfSpeech", "Transcription" },
                values: new object[] { "The world is beautiful", "noun", "/wɜːrld/" });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Example", "PartOfSpeech", "Transcription" },
                values: new object[] { "The cat is sleeping", "noun", "/kæt/" });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Example", "PartOfSpeech", "Transcription" },
                values: new object[] { "The dog is barking", "noun", "/dɒɡ/" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Example", "PartOfSpeech", "Transcription" },
                values: new object[] { "", "", "" });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Example", "PartOfSpeech", "Transcription" },
                values: new object[] { "", "", "" });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Example", "PartOfSpeech", "Transcription" },
                values: new object[] { "", "", "" });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Example", "PartOfSpeech", "Transcription" },
                values: new object[] { "", "", "" });
        }
    }
}
