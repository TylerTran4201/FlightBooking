using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightBooking.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToAirline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Airlines",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Airlines");
        }
    }
}
