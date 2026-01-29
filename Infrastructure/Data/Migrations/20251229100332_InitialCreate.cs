using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable //В этом файле не проверяй, могут ли переменные быть null

namespace Infrastructure.Data.Migrations
{
    public partial class InitialCreate : Migration //класс наследующий от Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Managers",
                columns: table => new //Определение колонок новой таблицы через лямбда-выражение
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table => //Определение ограничений
                {
                    table.PrimaryKey("PK_Managers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true), //описание
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ListingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                    table.ForeignKey( //Создание ВНЕШНЕГО ключа
                        name: "FK_Properties_Managers_ManagerId", //Имя ограничения внешнего ключа в БД
                        column: x => x.ManagerId,//Поле в таблице Properties, которое будет внешним ключом
                        principalTable: "Managers",
                        principalColumn: "Id", //Поле в таблице Managers, на которое ссылаемся
                        onDelete: ReferentialAction.Cascade); //При удалении менеджера автоматически удалить все его объекты
                });

            migrationBuilder.CreateIndex(
                name: "IX_Managers_Email",
                table: "Managers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ListingDate",
                table: "Properties",
                column: "ListingDate");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ManagerId",
                table: "Properties",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Status",
                table: "Properties",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Type",
                table: "Properties",
                column: "Type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "Managers");
        }
    }
}
