using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_netcore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tbl_Empresa",
                columns: table => new
                {
                    empresa_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    empresa_rutaimagen = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, defaultValue: "default.png"),
                    empresa_ruc = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    empresa_nombredueno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    empresa_nombreempresa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    empresa_direccion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    empresa_telefono = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    empresa_email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    empresa_fechacreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    empresa_estado = table.Column<int>(type: "int", nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Empresa", x => x.empresa_id);
                });

            migrationBuilder.CreateTable(
                name: "Tbl_Empleado",
                columns: table => new
                {
                    empleado_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    empleado_nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    empleado_apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    empleado_direccion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    empleado_telefono = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    empleado_correo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    empleado_fechanacimiento = table.Column<DateTime>(type: "date", nullable: true),
                    empleado_fechaingreso = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    empresa_id = table.Column<int>(type: "int", nullable: true),
                    empleado_estado = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_Empleado", x => x.empleado_id);
                    table.ForeignKey(
                        name: "FK_Tbl_Empleado_Tbl_Empresa_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "Tbl_Empresa",
                        principalColumn: "empresa_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tbl_Empleado_empresa_id",
                table: "Tbl_Empleado",
                column: "empresa_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tbl_Empleado");

            migrationBuilder.DropTable(
                name: "Tbl_Empresa");
        }
    }
}
