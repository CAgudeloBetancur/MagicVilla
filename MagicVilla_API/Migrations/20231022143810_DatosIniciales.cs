using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class DatosIniciales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenidad", "Area", "Detalle", "FechaActualizacion", "FechaCreacion", "ImageUrl", "Nombre", "Ocupantes", "Tarifa" },
                values: new object[,]
                {
                    { 1, "", 60, "Detalle Villa Real", new DateTime(2023, 10, 22, 9, 38, 10, 121, DateTimeKind.Local).AddTicks(2740), new DateTime(2023, 10, 22, 9, 38, 10, 121, DateTimeKind.Local).AddTicks(2730), "", "Villa Real", 5, 200.0 },
                    { 2, "", 45, "Detalle Villa Maria", new DateTime(2023, 10, 22, 9, 38, 10, 121, DateTimeKind.Local).AddTicks(2744), new DateTime(2023, 10, 22, 9, 38, 10, 121, DateTimeKind.Local).AddTicks(2743), "", "Villa Maria", 3, 150.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
