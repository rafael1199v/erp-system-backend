#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Erp.Sales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitSalesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sal");

            migrationBuilder.CreateTable(
                name: "customers",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "order_statuses",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payment_types",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "restaurant_order_detail_statuses",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_restaurant_order_detail_statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tax_configurations",
                schema: "sal",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GlobalTaxPercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tax_configurations", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "waiters",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_waiters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    OrderDatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrderStatusId = table.Column<int>(type: "integer", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    TaxPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orders_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "sal",
                        principalTable: "customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_orders_order_statuses_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalSchema: "sal",
                        principalTable: "order_statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sales",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    SubtotalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    SaleDatetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    PaymentTypeId = table.Column<int>(type: "integer", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sales_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "sal",
                        principalTable: "customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_sales_payment_types_PaymentTypeId",
                        column: x => x.PaymentTypeId,
                        principalSchema: "sal",
                        principalTable: "payment_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "team_configurations",
                schema: "sal",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_configurations", x => new { x.CompanyId, x.CategoryId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_team_configurations_teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "sal",
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_details",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ProductPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    OrderModelId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_details_orders_OrderModelId",
                        column: x => x.OrderModelId,
                        principalSchema: "sal",
                        principalTable: "orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "restaurant_orders",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    WaiterId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_restaurant_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_restaurant_orders_orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "sal",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_restaurant_orders_waiters_WaiterId",
                        column: x => x.WaiterId,
                        principalSchema: "sal",
                        principalTable: "waiters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "sale_details",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SaleId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sale_details_sales_SaleId",
                        column: x => x.SaleId,
                        principalSchema: "sal",
                        principalTable: "sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "restaurant_order_details",
                schema: "sal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RestaurantOrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    RestaurantOrderDetailStatusId = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_restaurant_order_details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_restaurant_order_details_restaurant_order_detail_statuses_R~",
                        column: x => x.RestaurantOrderDetailStatusId,
                        principalSchema: "sal",
                        principalTable: "restaurant_order_detail_statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_restaurant_order_details_restaurant_orders_RestaurantOrderId",
                        column: x => x.RestaurantOrderId,
                        principalSchema: "sal",
                        principalTable: "restaurant_orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_details_OrderModelId",
                schema: "sal",
                table: "order_details",
                column: "OrderModelId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_CustomerId",
                schema: "sal",
                table: "orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderStatusId",
                schema: "sal",
                table: "orders",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_restaurant_order_details_RestaurantOrderDetailStatusId",
                schema: "sal",
                table: "restaurant_order_details",
                column: "RestaurantOrderDetailStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_restaurant_order_details_RestaurantOrderId",
                schema: "sal",
                table: "restaurant_order_details",
                column: "RestaurantOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_restaurant_orders_OrderId",
                schema: "sal",
                table: "restaurant_orders",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_restaurant_orders_WaiterId",
                schema: "sal",
                table: "restaurant_orders",
                column: "WaiterId");

            migrationBuilder.CreateIndex(
                name: "IX_sale_details_SaleId",
                schema: "sal",
                table: "sale_details",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_sales_CustomerId",
                schema: "sal",
                table: "sales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_sales_PaymentTypeId",
                schema: "sal",
                table: "sales",
                column: "PaymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_team_configurations_TeamId",
                schema: "sal",
                table: "team_configurations",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_details",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "restaurant_order_details",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "sale_details",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "tax_configurations",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "team_configurations",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "restaurant_order_detail_statuses",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "restaurant_orders",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "sales",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "teams",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "waiters",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "payment_types",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "customers",
                schema: "sal");

            migrationBuilder.DropTable(
                name: "order_statuses",
                schema: "sal");
        }
    }
}
