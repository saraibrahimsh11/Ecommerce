using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class add_data_to_brand_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Brands (name, description, status) values ('Apple', 'Integer tincidunt ante vel ipsum.', 1);insert into Brands (name, description, status) values ('Hp', 'Maecenas tincidunt lacus at velit.', 0);insert into Brands (name, description, status) values ('Dell', 'Maecenas leo odio, condimentum id, luctus nec, molestie sed, justo.', 1);insert into Brands (name, description, status) values ('Oppo', 'Cum sociis natoque penatibus et magnis dis .', 1);insert into Brands (name, description, status) values ('LG', 'Quisque porta volutpat erat.', 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from Brands");
        }
    }
}
