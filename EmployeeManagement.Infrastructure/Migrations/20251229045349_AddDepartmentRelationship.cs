using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EmployeeManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Tạo bảng Departments
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Code",
                table: "Departments",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            // 2. Thêm cột DepartmentId vào bảng Employees (nullable tạm thời)
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Employees",
                type: "integer",
                nullable: true);

            // 3. Seed một số Department mẫu (bao gồm Department mặc định)
            migrationBuilder.Sql(@"
                INSERT INTO ""Departments"" (""Code"", ""Name"", ""Description"", ""IsActive"", ""CreatedAt"", ""UpdatedAt"")
                VALUES 
                    ('IT', 'IT', 'Information Technology', 'true', NOW(), NOW()),
                    ('HR', 'HR', 'Human Resources', 'true', NOW(), NOW()),
                    ('FIN', 'Finance', 'Finance Department', 'true', NOW(), NOW()),
                    ('OTHER', 'Other', 'Other Department', 'true', NOW(), NOW())
                ON CONFLICT (""Code"") DO NOTHING;
            ");

            // 4. Map dữ liệu từ Department (string) sang DepartmentId (int)
            // Lấy ID của Department "Other" làm mặc định
            migrationBuilder.Sql(@"
                WITH default_dept AS (
                    SELECT ""Id"" FROM ""Departments"" WHERE ""Code"" = 'OTHER' LIMIT 1
                )
                UPDATE ""Employees""
                SET ""DepartmentId"" = COALESCE(
                    (SELECT ""Id"" 
                     FROM ""Departments"" 
                     WHERE ""Departments"".""Name"" = ""Employees"".""Department""
                     LIMIT 1),
                    (SELECT ""Id"" FROM default_dept)
                )
                WHERE ""DepartmentId"" IS NULL;
            ");

            // 4b. Xử lý trường hợp Employee không có Department hoặc Department = NULL
            migrationBuilder.Sql(@"
                UPDATE ""Employees""
                SET ""DepartmentId"" = (
                    SELECT ""Id"" FROM ""Departments"" WHERE ""Code"" = 'OTHER' LIMIT 1
                )
                WHERE ""DepartmentId"" IS NULL;
            ");

            // 5. Xóa cột Department cũ (nếu có)
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Employees");

            // 6. Đặt DepartmentId là NOT NULL (bây giờ đã không còn NULL nào)
            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Employees",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            // 7. Thêm foreign key constraint
            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa foreign key và index
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees");

            // Thêm lại cột Department (string) nếu muốn rollback
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Employees",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            // Map ngược lại từ DepartmentId sang Department (string)
            migrationBuilder.Sql(@"
                UPDATE ""Employees""
                SET ""Department"" = (
                    SELECT ""Name"" 
                    FROM ""Departments"" 
                    WHERE ""Departments"".""Id"" = ""Employees"".""DepartmentId""
                    LIMIT 1
                );
            ");

            // Xóa cột DepartmentId
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Employees");

            // Xóa bảng Departments
            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}