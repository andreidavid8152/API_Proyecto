using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API_Proyecto.Migrations
{
    /// <inheritdoc />
    public partial class Creacion_tablas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locales",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropietarioID = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locales", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Locales_Usuarios_PropietarioID",
                        column: x => x.PropietarioID,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocalID = table.Column<int>(type: "int", nullable: false),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Calificacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Comentarios_Locales_LocalID",
                        column: x => x.LocalID,
                        principalTable: "Locales",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comentarios_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Horarios",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocalID = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horarios", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Horarios_Locales_LocalID",
                        column: x => x.LocalID,
                        principalTable: "Locales",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImagenesLocal",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagenesLocal", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ImagenesLocal_Locales_LocalID",
                        column: x => x.LocalID,
                        principalTable: "Locales",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocalID = table.Column<int>(type: "int", nullable: false),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    HorarioID = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Reservas_Horarios_HorarioID",
                        column: x => x.HorarioID,
                        principalTable: "Horarios",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservas_Locales_LocalID",
                        column: x => x.LocalID,
                        principalTable: "Locales",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Reservas_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "Nombre", "Password", "Username" },
                values: new object[,]
                {
                    { 1, "admin@gmail.com", "administrador", "2003", "admin" },
                    { 2, "andrei@gmail.com", "andrei", "2003", "andrei" }
                });

            migrationBuilder.InsertData(
                table: "Locales",
                columns: new[] { "ID", "Capacidad", "Descripcion", "Direccion", "Nombre", "PropietarioID" },
                values: new object[,]
                {
                    { 1, 50, "Cafetería con ambiente acogedor y música en vivo.", "Avenida Siempre Viva 742", "Café Central", 1 },
                    { 2, 20, "Espacio cultural con selección de libros de autores independientes.", "Calle Literaria 101", "Librería Letras", 2 }
                });

            migrationBuilder.InsertData(
                table: "Horarios",
                columns: new[] { "ID", "HoraFin", "HoraInicio", "LocalID" },
                values: new object[,]
                {
                    { 1, new TimeSpan(0, 12, 0, 0, 0), new TimeSpan(0, 8, 0, 0, 0), 1 },
                    { 2, new TimeSpan(0, 16, 0, 0, 0), new TimeSpan(0, 14, 0, 0, 0), 1 },
                    { 3, new TimeSpan(0, 20, 0, 0, 0), new TimeSpan(0, 18, 0, 0, 0), 1 },
                    { 4, new TimeSpan(0, 23, 59, 0, 0), new TimeSpan(0, 22, 0, 0, 0), 1 },
                    { 5, new TimeSpan(0, 12, 0, 0, 0), new TimeSpan(0, 8, 0, 0, 0), 2 },
                    { 6, new TimeSpan(0, 16, 0, 0, 0), new TimeSpan(0, 14, 0, 0, 0), 2 },
                    { 7, new TimeSpan(0, 20, 0, 0, 0), new TimeSpan(0, 18, 0, 0, 0), 2 },
                    { 8, new TimeSpan(0, 23, 59, 0, 0), new TimeSpan(0, 22, 0, 0, 0), 2 }
                });

            migrationBuilder.InsertData(
                table: "ImagenesLocal",
                columns: new[] { "ID", "LocalID", "Url" },
                values: new object[,]
                {
                    { 1, 1, "https://img10.naventcdn.com/avisos/resize/9/01/41/76/97/06/1200x1200/1130234069.jpg" },
                    { 2, 1, "https://img10.naventcdn.com/avisos/resize/9/01/41/76/97/06/1200x1200/1130234070.jpg" },
                    { 3, 1, "https://img10.naventcdn.com/avisos/resize/9/01/41/76/97/06/1200x1200/1130234071.jpg" },
                    { 4, 2, "https://img10.naventcdn.com/avisos/resize/9/00/91/40/78/30/1200x1200/1121170509.jpg" },
                    { 5, 2, "https://img10.naventcdn.com/avisos/resize/9/00/91/40/78/30/1200x1200/1121170504.jpg" },
                    { 6, 2, "https://img10.naventcdn.com/avisos/resize/9/00/91/40/78/30/1200x1200/1121170505.jpg" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_LocalID",
                table: "Comentarios",
                column: "LocalID");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UsuarioID",
                table: "Comentarios",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_LocalID",
                table: "Horarios",
                column: "LocalID");

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesLocal_LocalID",
                table: "ImagenesLocal",
                column: "LocalID");

            migrationBuilder.CreateIndex(
                name: "IX_Locales_PropietarioID",
                table: "Locales",
                column: "PropietarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HorarioID",
                table: "Reservas",
                column: "HorarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_LocalID",
                table: "Reservas",
                column: "LocalID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_UsuarioID",
                table: "Reservas",
                column: "UsuarioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "ImagenesLocal");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Horarios");

            migrationBuilder.DropTable(
                name: "Locales");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
