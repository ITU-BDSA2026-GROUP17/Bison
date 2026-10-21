using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEmailsAndTaxonsAndPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Observations_ObservationId",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_Users_AuthorId",
                table: "Proposals");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Observations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Proposals",
                table: "Proposals");

            migrationBuilder.RenameTable(
                name: "Proposals",
                newName: "Posts");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_ObservationId",
                table: "Posts",
                newName: "IX_Posts_ObservationId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_AuthorId",
                table: "Posts",
                newName: "IX_Posts_AuthorId");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "TaxonId",
                table: "Posts",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<int>(
                name: "ObservationId",
                table: "Posts",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Posts",
                type: "TEXT",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Proposal_ObservationId",
                table: "Posts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Proposal_TaxonId",
                table: "Posts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Posts",
                type: "TEXT",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Taxons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DWC_TaxonId = table.Column<string>(type: "TEXT", nullable: false),
                    VernacularName = table.Column<string>(type: "TEXT", nullable: true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Taxons_Taxons_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Taxons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Proposal_ObservationId",
                table: "Posts",
                column: "Proposal_ObservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Proposal_TaxonId",
                table: "Posts",
                column: "Proposal_TaxonId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_TaxonId",
                table: "Posts",
                column: "TaxonId");

            migrationBuilder.CreateIndex(
                name: "IX_Taxons_DWC_TaxonId",
                table: "Taxons",
                column: "DWC_TaxonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Taxons_ParentId",
                table: "Taxons",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Posts_ObservationId",
                table: "Posts",
                column: "ObservationId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Posts_Proposal_ObservationId",
                table: "Posts",
                column: "Proposal_ObservationId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Taxons_Proposal_TaxonId",
                table: "Posts",
                column: "Proposal_TaxonId",
                principalTable: "Taxons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Taxons_TaxonId",
                table: "Posts",
                column: "TaxonId",
                principalTable: "Taxons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_AuthorId",
                table: "Posts",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Posts_ObservationId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Posts_Proposal_ObservationId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Taxons_Proposal_TaxonId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Taxons_TaxonId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_AuthorId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "Taxons");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Proposal_ObservationId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Proposal_TaxonId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_TaxonId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Proposal_ObservationId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Proposal_TaxonId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Posts");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Proposals");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ObservationId",
                table: "Proposals",
                newName: "IX_Proposals_ObservationId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_AuthorId",
                table: "Proposals",
                newName: "IX_Proposals_AuthorId");

            migrationBuilder.AlterColumn<string>(
                name: "TaxonId",
                table: "Proposals",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ObservationId",
                table: "Proposals",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proposals",
                table: "Proposals",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Location = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Observations_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    ObservationId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Text = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Observations_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "Observations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ObservationId",
                table: "Comments",
                column: "ObservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_AuthorId",
                table: "Observations",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Observations_ObservationId",
                table: "Proposals",
                column: "ObservationId",
                principalTable: "Observations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_Users_AuthorId",
                table: "Proposals",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
