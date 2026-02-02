using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FileNova.Migrations
{
    /// <inheritdoc />
    public partial class FixIdentityUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    Id_Permiso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.Id_Permiso);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RefreshTokken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MustChangePassword = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermisoRole",
                columns: table => new
                {
                    PermisosId_Permiso = table.Column<int>(type: "int", nullable: false),
                    RolesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermisoRole", x => new { x.PermisosId_Permiso, x.RolesId });
                    table.ForeignKey(
                        name: "FK_PermisoRole_Permisos_PermisosId_Permiso",
                        column: x => x.PermisosId_Permiso,
                        principalTable: "Permisos",
                        principalColumn: "Id_Permiso",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermisoRole_Role_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rol_Permisos",
                columns: table => new
                {
                    Id_Rol = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id_Permiso = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol_Permisos", x => new { x.Id_Rol, x.Id_Permiso });
                    table.ForeignKey(
                        name: "FK_Rol_Permisos_Permisos_Id_Permiso",
                        column: x => x.Id_Permiso,
                        principalTable: "Permisos",
                        principalColumn: "Id_Permiso",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rol_Permisos_Role_Id_Rol",
                        column: x => x.Id_Rol,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    id_documento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tamaño_KB = table.Column<float>(type: "real", nullable: false),
                    Fecha_Subida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ruta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_modificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    id_usuario = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.id_documento);
                    table.ForeignKey(
                        name: "FK_Documentos_User_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Solicitudes",
                columns: table => new
                {
                    id_solicitud = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre_Solicitud = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fecha_Solicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Detalles = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_usuario = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Tipo_Solicitud = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitudes", x => x.id_solicitud);
                    table.ForeignKey(
                        name: "FK_Solicitudes_User_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_Permisos",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id_Permiso = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_Permisos", x => new { x.UserId, x.Id_Permiso });
                    table.ForeignKey(
                        name: "FK_user_Permisos_Permisos_Id_Permiso",
                        column: x => x.Id_Permiso,
                        principalTable: "Permisos",
                        principalColumn: "Id_Permiso",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_Permisos_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trazabilidad_Documentos",
                columns: table => new
                {
                    id_trazabilidad_documento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ruta_Antigua = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ruta_Nueva = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fecha_Cambio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_documento = table.Column<int>(type: "int", nullable: false),
                    id_usuario = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trazabilidad_Documentos", x => x.id_trazabilidad_documento);
                    table.ForeignKey(
                        name: "FK_Trazabilidad_Documentos_Documentos_id_documento",
                        column: x => x.id_documento,
                        principalTable: "Documentos",
                        principalColumn: "id_documento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trazabilidad_Documentos_User_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id_Permiso", "Nombre" },
                values: new object[,]
                {
                    { 1, "DOCUMENTOS_VER" },
                    { 2, "DOCUMENTOS_CREAR" },
                    { 3, "DOCUMENTOS_EDITAR" },
                    { 4, "DOCUMENTOS_ELIMINAR" },
                    { 5, "ROLES_ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "562419f5-eed1-473b-bcc1-9f2dbab182b4", null, "Administrador", "ADMINISTRADOR" },
                    { "d12540b0-6de7-48dd-befa-066de9d3a6a0", null, "Cliente", "CLIENTE" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "AccessFailedCount", "Apellido", "ConcurrencyStamp", "Email", "EmailConfirmed", "Estado", "FechaCreacion", "LockoutEnabled", "LockoutEnd", "MustChangePassword", "Nombre", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshTokenExpiryTime", "RefreshTokken", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1", 0, "Sistema", "22222222-2222-2222-2222-222222222222", "admin@test.com", true, 1, new DateTime(2025, 12, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, true, "Admin", "ADMIN@TEST.COM", "ADMIN", "AQAAAAIAAYagAAAAEBGZiSMC2XDHccsUuRCJdG0VuDXu6I7CTGDK4JVO3oX11hZ+dOcdc1TsntsHaPwjAQ==", null, false, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "11111111-1111-1111-1111-111111111111", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "562419f5-eed1-473b-bcc1-9f2dbab182b4", "1" });

            migrationBuilder.InsertData(
                table: "Rol_Permisos",
                columns: new[] { "Id_Permiso", "Id_Rol" },
                values: new object[,]
                {
                    { 1, "562419f5-eed1-473b-bcc1-9f2dbab182b4" },
                    { 2, "562419f5-eed1-473b-bcc1-9f2dbab182b4" },
                    { 3, "562419f5-eed1-473b-bcc1-9f2dbab182b4" },
                    { 4, "562419f5-eed1-473b-bcc1-9f2dbab182b4" },
                    { 5, "562419f5-eed1-473b-bcc1-9f2dbab182b4" },
                    { 1, "d12540b0-6de7-48dd-befa-066de9d3a6a0" }
                });

            migrationBuilder.InsertData(
                table: "Solicitudes",
                columns: new[] { "id_solicitud", "Detalles", "Estado", "Fecha_Solicitud", "id_usuario", "Nombre_Solicitud", "Tipo_Solicitud" },
                values: new object[] { 1, "Solicitud para obtener acceso a documentos confidenciales.", "Pendiente", new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "1", null, "Acceso a Documentos" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_id_usuario",
                table: "Documentos",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_PermisoRole_RolesId",
                table: "PermisoRole",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_Nombre",
                table: "Permisos",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rol_Permisos_Id_Permiso",
                table: "Rol_Permisos",
                column: "Id_Permiso");

            migrationBuilder.CreateIndex(
                name: "IX_Rol_Permisos_Id_Rol_Id_Permiso",
                table: "Rol_Permisos",
                columns: new[] { "Id_Rol", "Id_Permiso" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Role",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_id_usuario",
                table: "Solicitudes",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Trazabilidad_Documentos_id_documento",
                table: "Trazabilidad_Documentos",
                column: "id_documento");

            migrationBuilder.CreateIndex(
                name: "IX_Trazabilidad_Documentos_id_usuario",
                table: "Trazabilidad_Documentos",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_user_Permisos_Id_Permiso",
                table: "user_Permisos",
                column: "Id_Permiso");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "PermisoRole");

            migrationBuilder.DropTable(
                name: "Rol_Permisos");

            migrationBuilder.DropTable(
                name: "Solicitudes");

            migrationBuilder.DropTable(
                name: "Trazabilidad_Documentos");

            migrationBuilder.DropTable(
                name: "user_Permisos");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
