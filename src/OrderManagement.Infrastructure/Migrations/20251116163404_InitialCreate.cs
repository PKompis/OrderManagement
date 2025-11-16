using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeliveryTimeNeeded = table.Column<TimeSpan>(type: "time", nullable: true),
                    DeliveryStreet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DeliveryCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeliveryZip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DeliveryLine2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DeliveryCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssignmentCourierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssignmentAssignedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AssignmentOutForDeliveryAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AssignmentDeliveredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AssignmentUnableToDeliverAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Staff_AssignmentCourierId",
                        column: x => x.AssignmentCourierId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    MenuItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => new { x.OrderId, x.MenuItemId });
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNumber",
                table: "Customers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AssignmentCourierId",
                table: "Orders",
                column: "AssignmentCourierId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedAt",
                table: "Orders",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Type",
                table: "Orders",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_IsActive_Role",
                table: "Staff",
                columns: new[] { "IsActive", "Role" });

            migrationBuilder.Sql(@"
                INSERT INTO MenuItems (Id, Name, Category, Price, IsAvailable)
                VALUES
                    ('10000000-0000-0000-0000-000000000001', 'Margherita Pizza', 'Pizza', 8.50, 1),
                    ('10000000-0000-0000-0000-000000000002', 'Pepperoni Pizza', 'Pizza', 9.50, 1),
                    ('10000000-0000-0000-0000-000000000003', 'Caesar Salad', 'Salad', 7.00, 1),
                    ('10000000-0000-0000-0000-000000000004', 'Chicken Burger', 'Burger', 8.90, 1),
                    ('10000000-0000-0000-0000-000000000005', 'French Fries', 'Sides', 3.50, 1);
            ");

            migrationBuilder.Sql(@"
                INSERT INTO Staff (Id, Name, Role, IsActive)
                VALUES
                    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Admin User', 2, 1),
                    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Kitchen Staff 1', 0, 1),
                    ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Kitchen Staff 2', 0, 1),
                    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'Courier 1', 1, 1),
                    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Courier 2', 1, 1);
            ");

            migrationBuilder.Sql(@"
                INSERT INTO Customers (Id, Name, PhoneNumber, Email)
                VALUES
                    ('11111111-1111-1111-1111-111111111111', 'Alice Johnson', '+30 690 000 0001', 'alice@example.com'),
                    ('22222222-2222-2222-2222-222222222222', 'Bob Smith', '+30 690 000 0002', 'bob@example.com'),
                    ('33333333-3333-3333-3333-333333333333', 'Charlie Brown', '+30 690 000 0003', 'charlie@example.com');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Staff");
        }
    }
}
