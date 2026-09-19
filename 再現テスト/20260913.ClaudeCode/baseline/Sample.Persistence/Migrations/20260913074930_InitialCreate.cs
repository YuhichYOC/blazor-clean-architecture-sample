using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sample.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ITEM",
                columns: table => new
                {
                    ITEM_CODE = table.Column<string>(type: "VARCHAR2(20)", nullable: false),
                    ITEM_NAME = table.Column<string>(type: "NVARCHAR2(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITEM", x => x.ITEM_CODE);
                });

            migrationBuilder.CreateTable(
                name: "MATERIAL",
                columns: table => new
                {
                    ITEM_CODE = table.Column<string>(type: "VARCHAR2(20)", nullable: false),
                    ITEM_NAME = table.Column<string>(type: "NVARCHAR2(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MATERIAL", x => x.ITEM_CODE);
                });

            migrationBuilder.CreateTable(
                name: "BOM",
                columns: table => new
                {
                    ITEM_CODE = table.Column<string>(type: "VARCHAR2(20)", nullable: false),
                    M_ITEM_CODE = table.Column<string>(type: "VARCHAR2(20)", nullable: false),
                    REQUIREMENT = table.Column<decimal>(type: "NUMBER(9,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BOM", x => new { x.ITEM_CODE, x.M_ITEM_CODE });
                    table.ForeignKey(
                        name: "FK_BOM_ITEM",
                        column: x => x.ITEM_CODE,
                        principalTable: "ITEM",
                        principalColumn: "ITEM_CODE",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BOM_MATERIAL",
                        column: x => x.M_ITEM_CODE,
                        principalTable: "MATERIAL",
                        principalColumn: "ITEM_CODE",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BOM_M_ITEM_CODE",
                table: "BOM",
                column: "M_ITEM_CODE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BOM");

            migrationBuilder.DropTable(
                name: "ITEM");

            migrationBuilder.DropTable(
                name: "MATERIAL");
        }
    }
}
