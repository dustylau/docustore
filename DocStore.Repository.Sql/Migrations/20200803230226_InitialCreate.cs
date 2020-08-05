using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DocStore.Repository.Sql.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Content",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    Size = table.Column<int>(nullable: false),
                    Hash = table.Column<string>(maxLength: 128, nullable: false),
                    Data = table.Column<byte[]>(maxLength: 2147483647, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Content", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileSystemItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    ItemType = table.Column<int>(nullable: false),
                    Path = table.Column<string>(maxLength: 256, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    ContentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileSystemItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileSystemItems_FileSystemItems_ParentId",
                        column: x => x.ParentId,
                        principalTable: "FileSystemItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FileSystemItems_Content_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Value = table.Column<string>(maxLength: 2147483647, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_FileSystemItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "FileSystemItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileSystemItems_ParentId",
                table: "FileSystemItems",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileSystemItems_ContentId",
                table: "FileSystemItems",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ItemId",
                table: "Tags",
                column: "ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "FileSystemItems");

            migrationBuilder.DropTable(
                name: "Content");
        }
    }
}
