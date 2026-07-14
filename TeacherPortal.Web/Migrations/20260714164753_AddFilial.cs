using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherPortal.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddFilial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
        name: "Filials",
        columns: table => new
        {
            Id = table.Column<int>(type: "INTEGER", nullable: false)
                .Annotation("Sqlite:Autoincrement", true),
            Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
            Address = table.Column<string>(type: "TEXT", nullable: true),
            Phone = table.Column<string>(type: "TEXT", nullable: true),
            CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Filials", x => x.Id);
        });

            // 2. Вставляем 6 филиалов
            migrationBuilder.InsertData(
                table: "Filials",
                columns: new[] { "Name", "Address", "Phone", "CreatedAt" },
                values: new object[,]
                {
            { "Условный Ф 1", "Адрес филиала 1", null, DateTime.UtcNow },
            { "Условный Ф 2", "Адрес филиала 2", null, DateTime.UtcNow },
            { "Условный Ф 3", "Адрес филиала 3", null, DateTime.UtcNow },
            { "Условный Ф 4", "Адрес филиала 4", null, DateTime.UtcNow },
            { "Условный Ф 5", "Адрес филиала 5", null, DateTime.UtcNow },
            { "Условный Ф 6", "Адрес филиала 6", null, DateTime.UtcNow }
                });

            // 3. Добавляем столбец FilialId в Courses с внешним ключом
            migrationBuilder.AddColumn<int>(
                name: "FilialId",
                table: "Courses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1); // <-- Важно! Указываем ID первого филиала

            // 4. Создаем индекс для внешнего ключа
            migrationBuilder.CreateIndex(
                name: "IX_Courses_FilialId",
                table: "Courses",
                column: "FilialId");

            // 5. Добавляем ограничение внешнего ключа
            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Filials_FilialId",
                table: "Courses",
                column: "FilialId",
                principalTable: "Filials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Filials_FilialId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "Filials");

            migrationBuilder.DropIndex(
                name: "IX_Courses_FilialId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "FilialId",
                table: "Courses");
        }
    }
}
