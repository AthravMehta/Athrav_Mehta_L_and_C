using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAggregation.Migrations
{
    /// <inheritdoc />
    public partial class IsHiddenandHideReasoncolumnaddedincategoryandkeywordtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HideReason",
                table: "Keywords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Keywords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HideReason",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 1,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 2,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 3,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 4,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 5,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 6,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 7,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 8,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 9,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 10,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 11,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 12,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 13,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 14,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 15,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 16,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 17,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 18,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 19,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 20,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 21,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 22,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 23,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 24,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 25,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 26,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 27,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 28,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 29,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 30,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 31,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 32,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 33,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 34,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 35,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 36,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 37,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 38,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 39,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 40,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 41,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 42,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 43,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 44,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 45,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 46,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 47,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 48,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 49,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 50,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 51,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 52,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 53,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 54,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 55,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 56,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 57,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 58,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 59,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 60,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 61,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 62,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 63,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 64,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 65,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 66,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 67,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 68,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 69,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 70,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 71,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 72,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 73,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 74,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 75,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 76,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 77,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 78,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 79,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 80,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 81,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 82,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 83,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 84,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 85,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 86,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 87,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 88,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 89,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 90,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 91,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 92,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 93,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 94,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 95,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 96,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 97,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 98,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 99,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Keywords",
                keyColumn: "KeywordId",
                keyValue: 100,
                columns: new[] { "HideReason", "IsHidden" },
                values: new object[] { null, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HideReason",
                table: "Keywords");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Keywords");

            migrationBuilder.DropColumn(
                name: "HideReason",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Categories");
        }
    }
}
