using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Person.Api.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonTypes",
                columns: table => new
                {
                    PersonTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonTypes", x => x.PersonTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    PersonTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_Persons_PersonTypes_PersonTypeId",
                        column: x => x.PersonTypeId,
                        principalTable: "PersonTypes",
                        principalColumn: "PersonTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PersonTypes",
                columns: new[] { "PersonTypeId", "Description" },
                values: new object[,]
                {
                    { 1, "Teacher" },
                    { 2, "Student" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "PersonId", "Age", "Name", "PersonTypeId" },
                values: new object[,]
                {
                    { 1, 30, "John Doe", 1 },
                    { 2, 25, "Jane Smith", 2 },
                    { 3, 28, "Alice Johnson", 1 },
                    { 4, 22, "Bob Brown", 2 },
                    { 5, 35, "Carol White", 1 },
                    { 6, 20, "David Black", 2 },
                    { 7, 27, "Eve Green", 1 },
                    { 8, 23, "Frank Blue", 2 },
                    { 9, 32, "Grace Red", 1 },
                    { 10, 21, "Henry Yellow", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_PersonTypeId",
                table: "Persons",
                column: "PersonTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "PersonTypes");
        }
    }
}
