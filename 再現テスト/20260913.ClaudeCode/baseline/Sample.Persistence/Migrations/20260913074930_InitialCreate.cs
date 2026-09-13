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
                name: "Item",
                columns: table => new
                {
                    item_code = table.Column<string>(type: "varchar2(20)", nullable: false),
                    item_name = table.Column<string>(type: "nvarchar2(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.item_code);
                });

            migrationBuilder.CreateTable(
                name: "Material",
                columns: table => new
                {
                    item_code = table.Column<string>(type: "varchar2(20)", nullable: false),
                    item_name = table.Column<string>(type: "nvarchar2(40)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Material", x => x.item_code);
                });

            migrationBuilder.CreateTable(
                name: "Bom",
                columns: table => new
                {
                    item_code = table.Column<string>(type: "varchar2(20)", nullable: false),
                    m_item_code = table.Column<string>(type: "varchar2(20)", nullable: false),
                    requirement = table.Column<decimal>(type: "number(9,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bom", x => new { x.item_code, x.m_item_code });
                    table.ForeignKey(
                        name: "FK_Bom_Item_item_code",
                        column: x => x.item_code,
                        principalTable: "Item",
                        principalColumn: "item_code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bom_Material_m_item_code",
                        column: x => x.m_item_code,
                        principalTable: "Material",
                        principalColumn: "item_code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bom_m_item_code",
                table: "Bom",
                column: "m_item_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bom");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "Material");
        }
    }
}
