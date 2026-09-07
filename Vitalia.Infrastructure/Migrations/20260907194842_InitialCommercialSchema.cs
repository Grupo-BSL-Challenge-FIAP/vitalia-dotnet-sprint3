using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vitalia.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommercialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_VITALIA_CART",
                columns: table => new
                {
                    CART_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USER_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_VITALIA_CART", x => x.CART_ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_VITALIA_ORDERS",
                columns: table => new
                {
                    ORDER_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USER_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ORDER_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    TOTAL_AMOUNT = table.Column<decimal>(type: "DECIMAL(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_VITALIA_ORDERS", x => x.ORDER_ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_VITALIA_PRODUCT_CATEGORY",
                columns: table => new
                {
                    CATEGORY_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_VITALIA_PRODUCT_CATEGORY", x => x.CATEGORY_ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_VITALIA_PRODUCT",
                columns: table => new
                {
                    PRODUCT_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    PRICE = table.Column<decimal>(type: "DECIMAL(12,2)", precision: 12, scale: 2, nullable: false),
                    STOCK = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CATEGORY_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_VITALIA_PRODUCT", x => x.PRODUCT_ID);
                    table.ForeignKey(
                        name: "FK_TB_VITALIA_PRODUCT_TB_VITALIA_PRODUCT_CATEGORY_CATEGORY_ID",
                        column: x => x.CATEGORY_ID,
                        principalTable: "TB_VITALIA_PRODUCT_CATEGORY",
                        principalColumn: "CATEGORY_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_VITALIA_CART_ITEM",
                columns: table => new
                {
                    CART_ITEM_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CART_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    PRODUCT_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    QUANTITY = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UNIT_PRICE = table.Column<decimal>(type: "DECIMAL(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_VITALIA_CART_ITEM", x => x.CART_ITEM_ID);
                    table.ForeignKey(
                        name: "FK_TB_VITALIA_CART_ITEM_TB_VITALIA_CART_CART_ID",
                        column: x => x.CART_ID,
                        principalTable: "TB_VITALIA_CART",
                        principalColumn: "CART_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_VITALIA_CART_ITEM_TB_VITALIA_PRODUCT_PRODUCT_ID",
                        column: x => x.PRODUCT_ID,
                        principalTable: "TB_VITALIA_PRODUCT",
                        principalColumn: "PRODUCT_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_VITALIA_ORDER_ITEM",
                columns: table => new
                {
                    ORDER_ITEM_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ORDER_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    PRODUCT_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    QUANTITY = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UNIT_PRICE = table.Column<decimal>(type: "DECIMAL(12,2)", precision: 12, scale: 2, nullable: false),
                    SUBTOTAL = table.Column<decimal>(type: "DECIMAL(12,2)", precision: 12, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_VITALIA_ORDER_ITEM", x => x.ORDER_ITEM_ID);
                    table.ForeignKey(
                        name: "FK_TB_VITALIA_ORDER_ITEM_TB_VITALIA_ORDERS_ORDER_ID",
                        column: x => x.ORDER_ID,
                        principalTable: "TB_VITALIA_ORDERS",
                        principalColumn: "ORDER_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_VITALIA_ORDER_ITEM_TB_VITALIA_PRODUCT_PRODUCT_ID",
                        column: x => x.PRODUCT_ID,
                        principalTable: "TB_VITALIA_PRODUCT",
                        principalColumn: "PRODUCT_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_VITALIA_CART_ITEM_CART_ID_PRODUCT_ID",
                table: "TB_VITALIA_CART_ITEM",
                columns: new[] { "CART_ID", "PRODUCT_ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_VITALIA_CART_ITEM_PRODUCT_ID",
                table: "TB_VITALIA_CART_ITEM",
                column: "PRODUCT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_VITALIA_ORDER_ITEM_ORDER_ID",
                table: "TB_VITALIA_ORDER_ITEM",
                column: "ORDER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_VITALIA_ORDER_ITEM_PRODUCT_ID",
                table: "TB_VITALIA_ORDER_ITEM",
                column: "PRODUCT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_VITALIA_PRODUCT_CATEGORY_ID",
                table: "TB_VITALIA_PRODUCT",
                column: "CATEGORY_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_VITALIA_CART_ITEM");

            migrationBuilder.DropTable(
                name: "TB_VITALIA_ORDER_ITEM");

            migrationBuilder.DropTable(
                name: "TB_VITALIA_CART");

            migrationBuilder.DropTable(
                name: "TB_VITALIA_ORDERS");

            migrationBuilder.DropTable(
                name: "TB_VITALIA_PRODUCT");

            migrationBuilder.DropTable(
                name: "TB_VITALIA_PRODUCT_CATEGORY");
        }
    }
}
