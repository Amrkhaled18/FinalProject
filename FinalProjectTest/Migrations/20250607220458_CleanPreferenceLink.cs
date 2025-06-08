using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProjectTest.Migrations
{
    /// <inheritdoc />
    public partial class CleanPreferenceLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Preferences_PreferencesID",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Preferences_AspNetUsers_UserId",
                table: "Preferences");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PreferencesID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PreferencesID",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_Preferences_AspNetUsers_UserId",
                table: "Preferences",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Preferences_AspNetUsers_UserId",
                table: "Preferences");

            migrationBuilder.AddColumn<int>(
                name: "PreferencesID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PreferencesID",
                table: "AspNetUsers",
                column: "PreferencesID",
                unique: true,
                filter: "[PreferencesID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Preferences_PreferencesID",
                table: "AspNetUsers",
                column: "PreferencesID",
                principalTable: "Preferences",
                principalColumn: "PreferenceID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Preferences_AspNetUsers_UserId",
                table: "Preferences",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
