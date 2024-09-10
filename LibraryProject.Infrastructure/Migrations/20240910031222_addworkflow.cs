using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LibraryProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addworkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Workflows",
                columns: table => new
                {
                    WorkflowId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkflowName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.WorkflowId);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowSequences",
                columns: table => new
                {
                    StepId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkflowId = table.Column<int>(type: "integer", nullable: false),
                    StepName = table.Column<string>(type: "text", nullable: true),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    RequiredRoleId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowSequences", x => x.StepId);
                    table.ForeignKey(
                        name: "FK_WorkflowSequences_AspNetRoles_RequiredRoleId",
                        column: x => x.RequiredRoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkflowSequences_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "WorkflowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NextStepRules",
                columns: table => new
                {
                    RuleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CurrentStepId = table.Column<int>(type: "integer", nullable: false),
                    NextStepId = table.Column<int>(type: "integer", nullable: false),
                    ConditionType = table.Column<string>(type: "text", nullable: true),
                    ConditionValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NextStepRules", x => x.RuleId);
                    table.ForeignKey(
                        name: "FK_NextStepRules_WorkflowSequences_CurrentStepId",
                        column: x => x.CurrentStepId,
                        principalTable: "WorkflowSequences",
                        principalColumn: "StepId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NextStepRules_WorkflowSequences_NextStepId",
                        column: x => x.NextStepId,
                        principalTable: "WorkflowSequences",
                        principalColumn: "StepId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Processs",
                columns: table => new
                {
                    ProcessId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkflowId = table.Column<int>(type: "integer", nullable: false),
                    RequesterId = table.Column<string>(type: "text", nullable: true),
                    RequestType = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    CurrentStepId = table.Column<int>(type: "integer", nullable: true),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processs", x => x.ProcessId);
                    table.ForeignKey(
                        name: "FK_Processs_AspNetUsers_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Processs_NextStepRules_CurrentStepId",
                        column: x => x.CurrentStepId,
                        principalTable: "NextStepRules",
                        principalColumn: "RuleId");
                    table.ForeignKey(
                        name: "FK_Processs_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "WorkflowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequestName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AppUserId = table.Column<string>(type: "text", nullable: true),
                    ProcessId = table.Column<int>(type: "integer", nullable: false),
                    BookTitle = table.Column<string>(type: "text", nullable: true),
                    Author = table.Column<string>(type: "text", nullable: true),
                    Publisher = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_BookRequests_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BookRequests_Processs_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "Processs",
                        principalColumn: "ProcessId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowActions",
                columns: table => new
                {
                    ActionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcessId = table.Column<int>(type: "integer", nullable: false),
                    StepId = table.Column<int>(type: "integer", nullable: false),
                    ActorId = table.Column<string>(type: "text", nullable: true),
                    Action = table.Column<string>(type: "text", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowActions", x => x.ActionId);
                    table.ForeignKey(
                        name: "FK_WorkflowActions_AspNetUsers_ActorId",
                        column: x => x.ActorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkflowActions_Processs_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "Processs",
                        principalColumn: "ProcessId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowActions_WorkflowSequences_StepId",
                        column: x => x.StepId,
                        principalTable: "WorkflowSequences",
                        principalColumn: "StepId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookRequests_AppUserId",
                table: "BookRequests",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookRequests_ProcessId",
                table: "BookRequests",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_NextStepRules_CurrentStepId",
                table: "NextStepRules",
                column: "CurrentStepId");

            migrationBuilder.CreateIndex(
                name: "IX_NextStepRules_NextStepId",
                table: "NextStepRules",
                column: "NextStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Processs_CurrentStepId",
                table: "Processs",
                column: "CurrentStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Processs_RequesterId",
                table: "Processs",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Processs_WorkflowId",
                table: "Processs",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowActions_ActorId",
                table: "WorkflowActions",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowActions_ProcessId",
                table: "WorkflowActions",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowActions_StepId",
                table: "WorkflowActions",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSequences_RequiredRoleId",
                table: "WorkflowSequences",
                column: "RequiredRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSequences_WorkflowId",
                table: "WorkflowSequences",
                column: "WorkflowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookRequests");

            migrationBuilder.DropTable(
                name: "WorkflowActions");

            migrationBuilder.DropTable(
                name: "Processs");

            migrationBuilder.DropTable(
                name: "NextStepRules");

            migrationBuilder.DropTable(
                name: "WorkflowSequences");

            migrationBuilder.DropTable(
                name: "Workflows");
        }
    }
}
