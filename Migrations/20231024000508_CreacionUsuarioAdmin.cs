using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Proyecto.Migrations
{
    /// <inheritdoc />
    public partial class CreacionUsuarioAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "Nombre", "Password", "Username" },
                values: new object[] { 1, "andrei@gmail.com", "Andrei", "2003", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
