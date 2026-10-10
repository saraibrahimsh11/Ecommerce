using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class add_data_to_category_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Categories (name, description, status) values ('Mobiles', 'Ut at dolor quis odio consequat varius.', 0);insert into Categories (name, description, status) values ('Laptops', 'Phasellus id sapien in sapien iaculis congue.', 1);insert into Categories (name, description, status) values ('tablets', 'Mauris sit amet eros.', 1);insert into Categories (name, description, status) values ('Accessories', 'Maecenas tristique, est et tempus semper.', 1);insert into Categories (name, description, status) values ('Cameras', 'Sed ante.', 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from Categories");
        }
    }
}
