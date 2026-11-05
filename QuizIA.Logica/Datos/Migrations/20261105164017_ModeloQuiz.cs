using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizIA.Logica.Datos.Migrations
{
    /// <inheritdoc />
    public partial class ModeloQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TextoOrigen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Proveedor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizzes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Preguntas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Enunciado = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OpcionA = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    OpcionB = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    OpcionC = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    OpcionD = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IndiceCorrecta = table.Column<int>(type: "int", nullable: false),
                    Explicacion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CitaTextual = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preguntas", x => x.Id);
                    table.CheckConstraint("CK_Preguntas_IndiceCorrecta", "[IndiceCorrecta] BETWEEN 0 AND 3");
                    table.ForeignKey(
                        name: "FK_Preguntas_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Preguntas_QuizId",
                table: "Preguntas",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_UserId",
                table: "Quizzes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Preguntas");

            migrationBuilder.DropTable(
                name: "Quizzes");
        }
    }
}
