using Microsoft.EntityFrameworkCore.Migrations;

namespace BookShoppingProject.DataAccess.Migrations
{
    public partial class ChangeUpdateCreateTypeToUpdateCoverType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE SP_UpdateCoverType
                                @Id int,
                                @Name varchar(50)
                                AS
                                update CoverTypes set name=@Name where Id=@Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE SP_UpdateCreateType
                                @Id int,
                                @Name varchar(50)
                                AS
                                update CoverTypes set name=@Name where Id=@Id");

        }
    }
}
