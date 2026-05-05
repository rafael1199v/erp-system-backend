#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Erp.Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inv");

            migrationBuilder.CreateTable(
                name: "companies",
                schema: "inv",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    image_url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    global_tax = table.Column<decimal>(type: "numeric(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "core_products",
                schema: "inv",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    image_url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_global_product = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_core_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "movement_statuses",
                schema: "inv",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movement_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "movement_types",
                schema: "inv",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movement_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_statuses",
                schema: "inv",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "suppliers",
                schema: "inv",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_suppliers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "transaction_types",
                schema: "inv",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaction_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                schema: "inv",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                    table.ForeignKey(
                        name: "FK_categories_companies_company_id",
                        column: x => x.company_id,
                        principalSchema: "inv",
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "units",
                schema: "inv",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_units", x => x.id);
                    table.ForeignKey(
                        name: "FK_units_companies_company_id",
                        column: x => x.company_id,
                        principalSchema: "inv",
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "warehouses",
                schema: "inv",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouses", x => x.id);
                    table.ForeignKey(
                        name: "FK_warehouses_companies_company_id",
                        column: x => x.company_id,
                        principalSchema: "inv",
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory_movements",
                schema: "inv",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    movement_date = table.Column<DateOnly>(type: "date", nullable: false),
                    movement_status_id = table.Column<int>(type: "integer", nullable: false),
                    movement_type_id = table.Column<int>(type: "integer", nullable: false),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_movements", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_movements_companies_company_id",
                        column: x => x.company_id,
                        principalSchema: "inv",
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inventory_movements_movement_statuses_movement_status_id",
                        column: x => x.movement_status_id,
                        principalSchema: "inv",
                        principalTable: "movement_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inventory_movements_movement_types_movement_type_id",
                        column: x => x.movement_type_id,
                        principalSchema: "inv",
                        principalTable: "movement_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "products",
                schema: "inv",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    core_product_id = table.Column<int>(type: "integer", nullable: false),
                    unit_id = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    company_id = table.Column<int>(type: "integer", nullable: false),
                    product_status_id = table.Column<int>(type: "integer", nullable: false),
                    supplier_id = table.Column<int>(type: "integer", nullable: false),
                    current_cost = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    reorder_level = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                    table.ForeignKey(
                        name: "FK_products_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "inv",
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_companies_company_id",
                        column: x => x.company_id,
                        principalSchema: "inv",
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_core_products_core_product_id",
                        column: x => x.core_product_id,
                        principalSchema: "inv",
                        principalTable: "core_products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_product_statuses_product_status_id",
                        column: x => x.product_status_id,
                        principalSchema: "inv",
                        principalTable: "product_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalSchema: "inv",
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_units_unit_id",
                        column: x => x.unit_id,
                        principalSchema: "inv",
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "products_warehouses",
                schema: "inv",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    warehouse_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_movement = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products_warehouses", x => new { x.product_id, x.warehouse_id });
                    table.ForeignKey(
                        name: "FK_products_warehouses_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "inv",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_products_warehouses_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalSchema: "inv",
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                schema: "inv",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    warehouse_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    transaction_date = table.Column<DateOnly>(type: "date", nullable: false),
                    inventory_movement_id = table.Column<int>(type: "integer", nullable: false),
                    transaction_type_id = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_transactions_inventory_movements_inventory_movement_id",
                        column: x => x.inventory_movement_id,
                        principalSchema: "inv",
                        principalTable: "inventory_movements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transactions_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "inv",
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transactions_transaction_types_transaction_type_id",
                        column: x => x.transaction_type_id,
                        principalSchema: "inv",
                        principalTable: "transaction_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_transactions_warehouses_warehouse_id",
                        column: x => x.warehouse_id,
                        principalSchema: "inv",
                        principalTable: "warehouses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_categories_company_id",
                schema: "inv",
                table: "categories",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_movements_company_id",
                schema: "inv",
                table: "inventory_movements",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_movements_movement_status_id",
                schema: "inv",
                table: "inventory_movements",
                column: "movement_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_movements_movement_type_id",
                schema: "inv",
                table: "inventory_movements",
                column: "movement_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_category_id",
                schema: "inv",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_company_id",
                schema: "inv",
                table: "products",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_core_product_id",
                schema: "inv",
                table: "products",
                column: "core_product_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_product_status_id",
                schema: "inv",
                table: "products",
                column: "product_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_supplier_id",
                schema: "inv",
                table: "products",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_unit_id",
                schema: "inv",
                table: "products",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_warehouses_warehouse_id",
                schema: "inv",
                table: "products_warehouses",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_inventory_movement_id",
                schema: "inv",
                table: "transactions",
                column: "inventory_movement_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_product_id",
                schema: "inv",
                table: "transactions",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_transaction_type_id",
                schema: "inv",
                table: "transactions",
                column: "transaction_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_transactions_warehouse_id",
                schema: "inv",
                table: "transactions",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "IX_units_company_id",
                schema: "inv",
                table: "units",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_company_id",
                schema: "inv",
                table: "warehouses",
                column: "company_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products_warehouses",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "transactions",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "inventory_movements",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "products",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "transaction_types",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "warehouses",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "movement_statuses",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "movement_types",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "categories",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "core_products",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "product_statuses",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "suppliers",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "units",
                schema: "inv");

            migrationBuilder.DropTable(
                name: "companies",
                schema: "inv");
        }
    }
}
