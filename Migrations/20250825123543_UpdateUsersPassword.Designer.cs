using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyRazorApp.Migrations
{
    public partial class UpdateUsersPassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Удаляем старое поле Password (если оно есть)
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            // Добавляем новое поле PasswordHash
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(255)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Удаляем PasswordHash при откате
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            // Восстанавливаем поле Password
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(255)",
                nullable: false,
                defaultValue: "");
        }
    }
}
