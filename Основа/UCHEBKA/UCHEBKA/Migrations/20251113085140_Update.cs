using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCHEBKA.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    Activity_ID = table.Column<long>(type: "bigint", nullable: false),
                    Activity_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activity_Score = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Activity__393F5BA573E56DB6", x => x.Activity_ID);
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    CityID = table.Column<long>(type: "bigint", nullable: false),
                    City_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City_Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__City__F2D21A960164C04E", x => x.CityID);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    CountryCode = table.Column<long>(type: "bigint", nullable: false),
                    Country_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country_Name_EN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country_Code2 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Country__5D9B0D2D9D59B178", x => x.CountryCode);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    EventID = table.Column<long>(type: "bigint", nullable: false),
                    Event_Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Event_Start_Time = table.Column<DateTime>(type: "datetime", nullable: true),
                    Event_Duration = table.Column<int>(type: "int", nullable: true),
                    Event_logoURL = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Event__7944C870E55878B3", x => x.EventID);
                });

            migrationBuilder.CreateTable(
                name: "EventType",
                columns: table => new
                {
                    EventTypeID = table.Column<long>(type: "bigint", nullable: false),
                    EventType_Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EventTyp__A9216B1FABED362B", x => x.EventTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<long>(type: "bigint", nullable: false),
                    Role_Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__8AFACE3A7198897A", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Section",
                columns: table => new
                {
                    SecID = table.Column<long>(type: "bigint", nullable: false),
                    Sec_Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Section__14A3699F8F65F1EC", x => x.SecID);
                });

            migrationBuilder.CreateTable(
                name: "Sex",
                columns: table => new
                {
                    SexID = table.Column<long>(type: "bigint", nullable: false),
                    Sex_Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Sex__75622DB6A0856907", x => x.SexID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<long>(type: "bigint", nullable: false),
                    User_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User_Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User_Lastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User_BirthDay = table.Column<DateTime>(type: "datetime", nullable: true),
                    User_Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User_Photo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User_Password = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CCAC157B16E9", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Activity_Event",
                columns: table => new
                {
                    Activity_Event_ID = table.Column<long>(type: "bigint", nullable: false),
                    FK_Activity_ID = table.Column<long>(type: "bigint", nullable: true),
                    FK_Event_ID = table.Column<long>(type: "bigint", nullable: true),
                    Day = table.Column<int>(type: "int", nullable: true),
                    Start_Time = table.Column<DateTime>(type: "datetime", nullable: true),
                    FK_Mod_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Activity__C338358757DE83FD", x => x.Activity_Event_ID);
                    table.ForeignKey(
                        name: "FK_ActivityEvent_Activity",
                        column: x => x.FK_Activity_ID,
                        principalTable: "Activity",
                        principalColumn: "Activity_ID");
                    table.ForeignKey(
                        name: "FK_ActivityEvent_Event",
                        column: x => x.FK_Event_ID,
                        principalTable: "Event",
                        principalColumn: "EventID");
                });

            migrationBuilder.CreateTable(
                name: "City_Event",
                columns: table => new
                {
                    City_Event_ID = table.Column<long>(type: "bigint", nullable: false),
                    FK_Event_ID = table.Column<long>(type: "bigint", nullable: true),
                    FK_City_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__City_Eve__E73D0A9421FACA5B", x => x.City_Event_ID);
                    table.ForeignKey(
                        name: "FK_CityEvent_City",
                        column: x => x.FK_City_ID,
                        principalTable: "City",
                        principalColumn: "CityID");
                    table.ForeignKey(
                        name: "FK_CityEvent_Event",
                        column: x => x.FK_Event_ID,
                        principalTable: "Event",
                        principalColumn: "EventID");
                });

            migrationBuilder.CreateTable(
                name: "Event_EventType",
                columns: table => new
                {
                    Event_EvenTypeID = table.Column<long>(type: "bigint", nullable: false),
                    FK_Event_ID = table.Column<long>(type: "bigint", nullable: true),
                    FK_EvenType_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Event_Ev__E1B0866A3D0758E9", x => x.Event_EvenTypeID);
                    table.ForeignKey(
                        name: "FK_EventEventType_Event",
                        column: x => x.FK_Event_ID,
                        principalTable: "Event",
                        principalColumn: "EventID");
                    table.ForeignKey(
                        name: "FK_EventEventType_EventType",
                        column: x => x.FK_EvenType_ID,
                        principalTable: "EventType",
                        principalColumn: "EventTypeID");
                });

            migrationBuilder.CreateTable(
                name: "Section_Event",
                columns: table => new
                {
                    Sec_Event_ID = table.Column<long>(type: "bigint", nullable: false),
                    FK_Sec_ID = table.Column<long>(type: "bigint", nullable: true),
                    FK_Event_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Section___CF4E4853B823711E", x => x.Sec_Event_ID);
                    table.ForeignKey(
                        name: "FK_SectionEvent_Event",
                        column: x => x.FK_Event_ID,
                        principalTable: "Event",
                        principalColumn: "EventID");
                    table.ForeignKey(
                        name: "FK_SectionEvent_Section",
                        column: x => x.FK_Sec_ID,
                        principalTable: "Section",
                        principalColumn: "SecID");
                });

            migrationBuilder.CreateTable(
                name: "Event_Jury",
                columns: table => new
                {
                    Event_Jury_ID = table.Column<long>(type: "bigint", nullable: false),
                    FK_Event_ID = table.Column<long>(type: "bigint", nullable: true),
                    FK_Activity_ID = table.Column<long>(type: "bigint", nullable: true),
                    FK_Jury_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Event_Ju__D92ACCB5DC74D827", x => x.Event_Jury_ID);
                    table.ForeignKey(
                        name: "FK_EventJury_Activity",
                        column: x => x.FK_Activity_ID,
                        principalTable: "Activity",
                        principalColumn: "Activity_ID");
                    table.ForeignKey(
                        name: "FK_EventJury_Event",
                        column: x => x.FK_Event_ID,
                        principalTable: "Event",
                        principalColumn: "EventID");
                    table.ForeignKey(
                        name: "FK_EventJury_Jury",
                        column: x => x.FK_Jury_ID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "User_Country",
                columns: table => new
                {
                    User_CountryID = table.Column<long>(type: "bigint", nullable: false),
                    FK_User_ID = table.Column<long>(type: "bigint", nullable: true),
                    FK_Country_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Cou__56010421769BD516", x => x.User_CountryID);
                    table.ForeignKey(
                        name: "FK_UserCountry_Country",
                        column: x => x.FK_Country_ID,
                        principalTable: "Country",
                        principalColumn: "CountryCode");
                    table.ForeignKey(
                        name: "FK_UserCountry_User",
                        column: x => x.FK_User_ID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "User_Event",
                columns: table => new
                {
                    User_Event_ID = table.Column<long>(type: "bigint", nullable: false),
                    FK_User_ID = table.Column<long>(type: "bigint", nullable: true),
                    FK_Event_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Eve__4663C0036CD0D3EA", x => x.User_Event_ID);
                    table.ForeignKey(
                        name: "FK_UserEvent_Event",
                        column: x => x.FK_Event_ID,
                        principalTable: "Event",
                        principalColumn: "EventID");
                    table.ForeignKey(
                        name: "FK_UserEvent_User",
                        column: x => x.FK_User_ID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "User_Role",
                columns: table => new
                {
                    User_Role_ID = table.Column<long>(type: "bigint", nullable: false),
                    FK_User_ID = table.Column<long>(type: "bigint", nullable: true),
                    FK_Role_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Rol__134E48EC93C851D8", x => x.User_Role_ID);
                    table.ForeignKey(
                        name: "FK_UserRole_Role",
                        column: x => x.FK_Role_ID,
                        principalTable: "Roles",
                        principalColumn: "RoleID");
                    table.ForeignKey(
                        name: "FK_UserRole_User",
                        column: x => x.FK_User_ID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "User_Sec",
                columns: table => new
                {
                    User_Sec_ID = table.Column<long>(type: "bigint", nullable: false),
                    FK_User_ID = table.Column<long>(type: "bigint", nullable: true),
                    FK_Sec_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Sec__99DF19A4703F9DD2", x => x.User_Sec_ID);
                    table.ForeignKey(
                        name: "FK_UserSec_Section",
                        column: x => x.FK_Sec_ID,
                        principalTable: "Section",
                        principalColumn: "SecID");
                    table.ForeignKey(
                        name: "FK_UserSec_User",
                        column: x => x.FK_User_ID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "User_Sex",
                columns: table => new
                {
                    User_Sec_ID = table.Column<long>(type: "bigint", nullable: false),
                    FK_User_ID = table.Column<long>(type: "bigint", nullable: true),
                    FK_Sex_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User_Sex__99DF19A4AB466115", x => x.User_Sec_ID);
                    table.ForeignKey(
                        name: "FK_UserSex_Sex",
                        column: x => x.FK_Sex_ID,
                        principalTable: "Sex",
                        principalColumn: "SexID");
                    table.ForeignKey(
                        name: "FK_UserSex_User",
                        column: x => x.FK_User_ID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_Event_FK_Activity_ID",
                table: "Activity_Event",
                column: "FK_Activity_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_Event_FK_Event_ID",
                table: "Activity_Event",
                column: "FK_Event_ID");

            migrationBuilder.CreateIndex(
                name: "IX_City_Event_FK_City_ID",
                table: "City_Event",
                column: "FK_City_ID");

            migrationBuilder.CreateIndex(
                name: "IX_City_Event_FK_Event_ID",
                table: "City_Event",
                column: "FK_Event_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventType_FK_Event_ID",
                table: "Event_EventType",
                column: "FK_Event_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventType_FK_EvenType_ID",
                table: "Event_EventType",
                column: "FK_EvenType_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Jury_FK_Activity_ID",
                table: "Event_Jury",
                column: "FK_Activity_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Jury_FK_Event_ID",
                table: "Event_Jury",
                column: "FK_Event_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Jury_FK_Jury_ID",
                table: "Event_Jury",
                column: "FK_Jury_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Section_Event_FK_Event_ID",
                table: "Section_Event",
                column: "FK_Event_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Section_Event_FK_Sec_ID",
                table: "Section_Event",
                column: "FK_Sec_ID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Country_FK_Country_ID",
                table: "User_Country",
                column: "FK_Country_ID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Country_FK_User_ID",
                table: "User_Country",
                column: "FK_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Event_FK_Event_ID",
                table: "User_Event",
                column: "FK_Event_ID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Event_FK_User_ID",
                table: "User_Event",
                column: "FK_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Role_FK_Role_ID",
                table: "User_Role",
                column: "FK_Role_ID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Role_FK_User_ID",
                table: "User_Role",
                column: "FK_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Sec_FK_Sec_ID",
                table: "User_Sec",
                column: "FK_Sec_ID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Sec_FK_User_ID",
                table: "User_Sec",
                column: "FK_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Sex_FK_Sex_ID",
                table: "User_Sex",
                column: "FK_Sex_ID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Sex_FK_User_ID",
                table: "User_Sex",
                column: "FK_User_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activity_Event");

            migrationBuilder.DropTable(
                name: "City_Event");

            migrationBuilder.DropTable(
                name: "Event_EventType");

            migrationBuilder.DropTable(
                name: "Event_Jury");

            migrationBuilder.DropTable(
                name: "Section_Event");

            migrationBuilder.DropTable(
                name: "User_Country");

            migrationBuilder.DropTable(
                name: "User_Event");

            migrationBuilder.DropTable(
                name: "User_Role");

            migrationBuilder.DropTable(
                name: "User_Sec");

            migrationBuilder.DropTable(
                name: "User_Sex");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "EventType");

            migrationBuilder.DropTable(
                name: "Activity");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Section");

            migrationBuilder.DropTable(
                name: "Sex");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
