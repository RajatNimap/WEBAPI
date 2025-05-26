using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_Management.Migrations
{
    /// <inheritdoc />
    public partial class slotupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "doctorAvailabilities",
                newName: "MorningStartTime");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "doctorAvailabilities",
                newName: "MorningEndTime");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EveningEndTime",
                table: "doctorAvailabilities",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EveningStartTime",
                table: "doctorAvailabilities",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EveningEndTime",
                table: "doctorAvailabilities");

            migrationBuilder.DropColumn(
                name: "EveningStartTime",
                table: "doctorAvailabilities");

            migrationBuilder.RenameColumn(
                name: "MorningStartTime",
                table: "doctorAvailabilities",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "MorningEndTime",
                table: "doctorAvailabilities",
                newName: "EndTime");
        }
    }
}
