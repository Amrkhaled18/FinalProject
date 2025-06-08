using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProjectTest.Migrations
{
    /// <inheritdoc />
    public partial class AddPreferenceUserLinkWithRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Preferences_PreferencesID",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Preferences_PreferencesID",
                table: "AspNetUsers",
                column: "PreferencesID",
                principalTable: "Preferences",
                principalColumn: "PreferenceID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Preferences_PreferencesID",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Preferences_PreferencesID",
                table: "AspNetUsers",
                column: "PreferencesID",
                principalTable: "Preferences",
                principalColumn: "PreferenceID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
