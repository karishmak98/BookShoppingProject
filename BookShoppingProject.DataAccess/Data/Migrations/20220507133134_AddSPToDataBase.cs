using Microsoft.EntityFrameworkCore.Migrations;

namespace BookShoppingProject.DataAccess.Migrations
{
    public partial class AddSPToDataBase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE SP_GetCoverTypes
                                AS
                                Select * from CoverTypes");
            migrationBuilder.Sql(@"CREATE PROCEDURE SP_GetCoverType
                                @Id int
                                AS
                                Select * from CoverTypes where Id=@Id");
            migrationBuilder.Sql(@"CREATE PROCEDURE SP_CreateCoverType
                                @Name varchar(50)
                                AS
                                insert CoverTypes values(@Name)");
            migrationBuilder.Sql(@"CREATE PROCEDURE SP_UpdateCreateType
                                @Id int,
                                @Name varchar(50)
                                AS
                                update CoverTypes set name=@Name where Id=@Id");


            migrationBuilder.Sql(@"CREATE PROCEDURE SP_DeleteCoverType
                                @Id int
                                AS
                                Delete from CoverTypes where Id=@Id");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
