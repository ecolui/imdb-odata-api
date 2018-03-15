using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DataAccess.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaRating",
                columns: table => new
                {
                    MediaRatingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AverageRating = table.Column<decimal>(nullable: false),
                    MediaIMDBId = table.Column<string>(nullable: true),
                    NumVotes = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaRating", x => x.MediaRatingId);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    MediaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MediaIMDBId = table.Column<string>(nullable: true),
                    RatingMediaRatingId = table.Column<int>(nullable: true),
                    endYear = table.Column<int>(nullable: true),
                    isAdult = table.Column<bool>(nullable: true),
                    originalTitle = table.Column<string>(nullable: true),
                    primaryTitle = table.Column<string>(nullable: true),
                    runtimeMinutes = table.Column<int>(nullable: true),
                    startYear = table.Column<int>(nullable: true),
                    titleType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.MediaId);
                    table.ForeignKey(
                        name: "FK_Media_MediaRating_RatingMediaRatingId",
                        column: x => x.RatingMediaRatingId,
                        principalTable: "MediaRating",
                        principalColumn: "MediaRatingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    GenreId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssociatedMediaMediaId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    MediaIMDBId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.GenreId);
                    table.ForeignKey(
                        name: "FK_Genre_Media_AssociatedMediaMediaId",
                        column: x => x.AssociatedMediaMediaId,
                        principalTable: "Media",
                        principalColumn: "MediaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    StaffId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BirthYear = table.Column<int>(nullable: true),
                    DeathYear = table.Column<int>(nullable: true),
                    MediaId = table.Column<int>(nullable: true),
                    PrimaryName = table.Column<string>(nullable: true),
                    imdbStaffId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.StaffId);
                    table.ForeignKey(
                        name: "FK_Staff_Media_MediaId",
                        column: x => x.MediaId,
                        principalTable: "Media",
                        principalColumn: "MediaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Principal",
                columns: table => new
                {
                    PrincipalId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssociatedMediaMediaId = table.Column<int>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    Characters = table.Column<string>(nullable: true),
                    Job = table.Column<string>(nullable: true),
                    MediaIMDBId = table.Column<string>(nullable: true),
                    Ordering = table.Column<int>(nullable: true),
                    StaffId = table.Column<int>(nullable: true),
                    imdbStaffId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Principal", x => x.PrincipalId);
                    table.ForeignKey(
                        name: "FK_Principal_Media_AssociatedMediaMediaId",
                        column: x => x.AssociatedMediaMediaId,
                        principalTable: "Media",
                        principalColumn: "MediaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Principal_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "StaffId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Profession",
                columns: table => new
                {
                    ProfessionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssociatedStaffStaffId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    imdbStaffId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profession", x => x.ProfessionId);
                    table.ForeignKey(
                        name: "FK_Profession_Staff_AssociatedStaffStaffId",
                        column: x => x.AssociatedStaffStaffId,
                        principalTable: "Staff",
                        principalColumn: "StaffId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffMediaLink",
                columns: table => new
                {
                    StaffMediaLinkId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssociatedMediaMediaId = table.Column<int>(nullable: true),
                    AssociatedStaffStaffId = table.Column<int>(nullable: true),
                    MediaIMDBId = table.Column<string>(nullable: true),
                    imdbStaffId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffMediaLink", x => x.StaffMediaLinkId);
                    table.ForeignKey(
                        name: "FK_StaffMediaLink_Media_AssociatedMediaMediaId",
                        column: x => x.AssociatedMediaMediaId,
                        principalTable: "Media",
                        principalColumn: "MediaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffMediaLink_Staff_AssociatedStaffStaffId",
                        column: x => x.AssociatedStaffStaffId,
                        principalTable: "Staff",
                        principalColumn: "StaffId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Genre_AssociatedMediaMediaId",
                table: "Genre",
                column: "AssociatedMediaMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_RatingMediaRatingId",
                table: "Media",
                column: "RatingMediaRatingId",
                unique: true,
                filter: "[RatingMediaRatingId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Principal_AssociatedMediaMediaId",
                table: "Principal",
                column: "AssociatedMediaMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Principal_StaffId",
                table: "Principal",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Profession_AssociatedStaffStaffId",
                table: "Profession",
                column: "AssociatedStaffStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_MediaId",
                table: "Staff",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffMediaLink_AssociatedMediaMediaId",
                table: "StaffMediaLink",
                column: "AssociatedMediaMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffMediaLink_AssociatedStaffStaffId",
                table: "StaffMediaLink",
                column: "AssociatedStaffStaffId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "Principal");

            migrationBuilder.DropTable(
                name: "Profession");

            migrationBuilder.DropTable(
                name: "StaffMediaLink");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "MediaRating");
        }
    }
}
