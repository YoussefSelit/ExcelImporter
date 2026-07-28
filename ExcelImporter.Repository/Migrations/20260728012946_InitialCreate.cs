using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelImporter.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CARD_IMPORTS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CLIENT_BRANCH = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    CARD_BRANCH = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    PAN = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    MBR = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    CUSTOMER_NAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    CLIENT_ID = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    EMBOSSING_NAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    CURRENT_CMS_STATUS = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    CURRENT_ONLINE_STATUS = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    CREATION_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    EXPIRY_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ACTIVATION_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CLOSING_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    INTERNAL_ACC = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    EXTERNAL_ACC = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    ACCOUNT_CURRENCY = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    MOBILE_NUMBER = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    PASSPORT_NUMBER = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    CURRENT_BALANCE = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    CREDIT_LIMIT = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    LIMIT_CURRENCY = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    ON_HOLD = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    ARRESTED_AMOUNT = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: true),
                    CARD_TYPE = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    LIMIT_GROUP = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    FINANCIAL_PROFILE = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    CLERK_CODE = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    ISSUANCE_REASON = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    CARD_PRODUCT_NAME = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    EXTERNAL_CODE = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    ISSUANCE_PRIORITY = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    PERSONAL_CODE = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    CONTRACT_NUMBER = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    GENDER = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    BIRTHDAY = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CONTACT_ADDRESS = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    CONTACTLESS = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CARD_IMPORTS", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CARD_IMPORTS");
        }
    }
}
