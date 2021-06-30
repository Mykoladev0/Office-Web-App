using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreDAL.Migrations
{
    public partial class IntialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Breeds",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Breed = table.Column<string>(unicode: false, maxLength: 30, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Breeds", x => x.id)
            //            .Annotation("SqlServer:Clustered", false);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Clubs",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        ClubId = table.Column<int>(nullable: false),
            //        ClubName = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Address1 = table.Column<string>(unicode: false, maxLength: 35, nullable: true),
            //        Address2 = table.Column<string>(unicode: false, maxLength: 35, nullable: true),
            //        Address3 = table.Column<string>(unicode: false, maxLength: 35, nullable: true),
            //        City = table.Column<string>(unicode: false, maxLength: 25, nullable: true),
            //        State = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
            //        Zip = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
            //        Country = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        International = table.Column<bool>(nullable: true),
            //        President = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        VicePresident = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        PresContact = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
            //        VPContact = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
            //        WebAddress = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Email = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        DuesPaid = table.Column<decimal>(type: "decimal(6, 2)", nullable: true),
            //        MemberCount = table.Column<int>(nullable: true),
            //        GoodStanding = table.Column<bool>(nullable: true),
            //        CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        Comments = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
            //        ModifiedBy = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Clubs", x => x.ClubId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Colors",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Color = table.Column<string>(unicode: false, maxLength: 25, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Colors", x => x.Id)
            //            .Annotation("SqlServer:Clustered", false);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Defaults",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        UserName = table.Column<string>(unicode: false, maxLength: 25, nullable: true),
            //        RegFee = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        MergeDataPath = table.Column<string>(unicode: false, maxLength: 75, nullable: true),
            //        ReportsPath = table.Column<string>(unicode: false, maxLength: 75, nullable: true),
            //        LastABKCNo = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        Ped_CustomPaperSize = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        PuppyReg_CustomPaperSize = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        ABKCShowFee = table.Column<int>(nullable: true),
            //        NonABKCShowFee = table.Column<int>(nullable: true),
            //        IsLocked = table.Column<bool>(nullable: false),
            //        LastBullyId = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        LastOwnerId = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        LastLitterId = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        LastShowId = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        LastClassId = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        LastClubId = table.Column<int>(nullable: true),
            //        LastJrHandlerId = table.Column<int>(nullable: true),
            //        CurVersion = table.Column<string>(maxLength: 12, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Defaults", x => x.id)
            //            .Annotation("SqlServer:Clustered", false);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Dogs",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Bully_Id = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        ABKC_No = table.Column<string>(unicode: false, maxLength: 12, nullable: true),
            //        UKC_No = table.Column<string>(unicode: false, maxLength: 12, nullable: true),
            //        AKC_No = table.Column<string>(unicode: false, maxLength: 12, nullable: true),
            //        ADBA_No = table.Column<string>(unicode: false, maxLength: 12, nullable: true),
            //        Other_No = table.Column<string>(unicode: false, maxLength: 12, nullable: true),
            //        RegNo = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
            //        RegType = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
            //        DogName = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Title = table.Column<string>(unicode: false, maxLength: 25, nullable: true),
            //        Suffix = table.Column<string>(unicode: false, maxLength: 25, nullable: true),
            //        Breed = table.Column<string>(unicode: false, maxLength: 30, nullable: true, defaultValueSql: "('')"),
            //        Gender = table.Column<string>(unicode: false, maxLength: 6, nullable: true, defaultValueSql: "('')"),
            //        Color = table.Column<string>(unicode: false, maxLength: 25, nullable: true, defaultValueSql: "('')"),
            //        Sire_No = table.Column<int>(nullable: true, defaultValueSql: "((1))"),
            //        Dam_No = table.Column<int>(nullable: true, defaultValueSql: "((1))"),
            //        Birthdate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        Litter_No = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        Owner_Id = table.Column<int>(nullable: false, defaultValueSql: "((1))"),
            //        CoOwner_Id = table.Column<int>(nullable: true, defaultValueSql: "((1))"),
            //        Tatoo_No = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
            //        Chip_No = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
            //        Points = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        ChampPoints = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        ChampWins = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        First_Generation = table.Column<bool>(nullable: false),
            //        Registered = table.Column<bool>(nullable: false),
            //        Verified = table.Column<bool>(nullable: false),
            //        Printed = table.Column<bool>(nullable: false),
            //        Comments = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
            //        Date_Created = table.Column<DateTime>(type: "datetime", nullable: true),
            //        Date_Registered = table.Column<DateTime>(type: "datetime", nullable: true),
            //        Reg_Amt = table.Column<decimal>(type: "money", nullable: true, defaultValueSql: "((0))"),
            //        Reg_User = table.Column<string>(unicode: false, maxLength: 25, nullable: true),
            //        Pups = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        Degrees = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
            //        LastModified = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ModifiedBy = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        BDNA = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
            //        NW = table.Column<bool>(nullable: true),
            //        CHCertPrinted = table.Column<bool>(nullable: true),
            //        BreedingDamOwnerId = table.Column<int>(nullable: true),
            //        BreedingDamCoOwnerId = table.Column<int>(nullable: true),
            //        BreedingSireOwnerId = table.Column<int>(nullable: true),
            //        BreedingSireCoOwnerId = table.Column<int>(nullable: true),
            //        WP_Points = table.Column<int>(nullable: true),
            //        SaveBully = table.Column<bool>(nullable: true),
            //        SAB_Date = table.Column<DateTime>(type: "datetime", nullable: true),
            //        NeedsNewCHCert = table.Column<bool>(nullable: true),
            //        WP_ChampWins = table.Column<int>(nullable: true),
            //        NeedsNewWPCHCert = table.Column<bool>(nullable: true),
            //        WP_Title = table.Column<string>(unicode: false, maxLength: 25, nullable: true),
            //        N = table.Column<bool>(nullable: true),
            //        BB = table.Column<bool>(nullable: true),
            //        Majors = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        Judges = table.Column<int>(nullable: true),
            //        OrigTitle = table.Column<string>(unicode: false, maxLength: 25, nullable: true),
            //        INW = table.Column<bool>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Dogs", x => x.Id)
            //            .Annotation("SqlServer:Clustered", false);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "JrHandlers",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        JrHandlerId = table.Column<int>(nullable: false),
            //        ChildFirstName = table.Column<string>(unicode: false, maxLength: 30, nullable: false),
            //        ChildLastName = table.Column<string>(unicode: false, maxLength: 30, nullable: false),
            //        ChildName = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        DOB = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ParentLastName = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
            //        ParentFirstName = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
            //        Address1 = table.Column<string>(unicode: false, maxLength: 35, nullable: true),
            //        Address2 = table.Column<string>(unicode: false, maxLength: 35, nullable: true),
            //        Address3 = table.Column<string>(unicode: false, maxLength: 35, nullable: true),
            //        City = table.Column<string>(unicode: false, maxLength: 25, nullable: true),
            //        State = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
            //        Zip = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
            //        Country = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
            //        International = table.Column<bool>(nullable: true),
            //        Email = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
            //        Cell = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
            //        Comments = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
            //        CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ModifiedBy = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Points = table.Column<int>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_JrHandlers", x => x.JrHandlerId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Litters",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Litter_Id = table.Column<int>(nullable: false),
            //        Owner_id = table.Column<int>(nullable: false),
            //        Dam_No = table.Column<int>(nullable: false),
            //        Sire_No = table.Column<int>(nullable: false),
            //        Males = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        Females = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        Birthdate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        BreedingDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        Breed = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Comments = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
            //        CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ModifiedBy = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        CoOwner_Id = table.Column<int>(nullable: true),
            //        SireOwnerId = table.Column<int>(nullable: true),
            //        SireCoOwnerId = table.Column<int>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Litters", x => x.id)
            //            .Annotation("SqlServer:Clustered", false);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ManualUpdateLog",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Dog_RecId = table.Column<int>(nullable: false),
            //        Bully_Id = table.Column<int>(nullable: false),
            //        ABKC_No = table.Column<string>(unicode: false, maxLength: 12, nullable: false),
            //        DogName = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        Title = table.Column<string>(unicode: false, maxLength: 25, nullable: true),
            //        OldPoints = table.Column<int>(nullable: true),
            //        OldChampWins = table.Column<int>(nullable: true),
            //        NewPoints = table.Column<int>(nullable: true),
            //        NewChampWins = table.Column<int>(nullable: true),
            //        DateInserted = table.Column<DateTime>(type: "datetime", nullable: true),
            //        InsertedBy = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ManualUpdateLog", x => x.id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Owners",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Owner_Id = table.Column<int>(nullable: false),
            //        Last_Name = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
            //        MiddleInitial = table.Column<string>(unicode: false, maxLength: 12, nullable: true),
            //        First_Name = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
            //        FullName = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Address1 = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Address2 = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Address3 = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        City = table.Column<string>(unicode: false, maxLength: 25, nullable: true),
            //        State = table.Column<string>(unicode: false, maxLength: 3, nullable: true, defaultValueSql: "('')"),
            //        Zip = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
            //        Country = table.Column<string>(unicode: false, maxLength: 20, nullable: true),
            //        International = table.Column<bool>(nullable: true),
            //        Email = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Phone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
            //        Cell = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
            //        Dual_Signature = table.Column<bool>(nullable: true, defaultValueSql: "((0))"),
            //        Comments = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
            //        CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ModifiedBy = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        WebPassword = table.Column<string>(unicode: false, maxLength: 20, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Owners", x => x.Owner_Id)
            //            .Annotation("SqlServer:Clustered", false);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Shows",
            //    columns: table => new
            //    {
            //        ShowId = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        ShowName = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
            //        ShowDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        Address = table.Column<string>(unicode: false, maxLength: 35, nullable: true),
            //        City = table.Column<string>(unicode: false, maxLength: 25, nullable: true),
            //        State = table.Column<string>(unicode: false, maxLength: 3, nullable: true),
            //        Zip = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
            //        International = table.Column<bool>(nullable: true),
            //        InsurancePolicy = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        InsuranceExpires = table.Column<DateTime>(type: "datetime", nullable: true),
            //        EntriesAllowed = table.Column<int>(nullable: true),
            //        ClubId = table.Column<int>(nullable: true),
            //        FirstShow = table.Column<bool>(nullable: true),
            //        AppRecvd = table.Column<bool>(nullable: true),
            //        DateApproved = table.Column<DateTime>(type: "datetime", nullable: true),
            //        FeePaid = table.Column<decimal>(type: "decimal(6, 3)", nullable: true),
            //        ABKCHosted = table.Column<bool>(nullable: true),
            //        Judge1 = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Judge2 = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        RingSteward = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Paperwork1 = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Paperwork2 = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        ABKCRep = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        AuthLetterRecvd = table.Column<bool>(nullable: true),
            //        DateLetterRecvd = table.Column<DateTime>(type: "datetime", nullable: true),
            //        BreedsShown = table.Column<string>(unicode: false, maxLength: 150, nullable: true),
            //        StylesShown = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        ClassesClosed = table.Column<bool>(nullable: true),
            //        CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ModifiedBy = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
            //        Comments = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
            //        JudgeId = table.Column<int>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Shows", x => x.ShowId)
            //            .Annotation("SqlServer:Clustered", false);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Transfers",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Dog_Id = table.Column<int>(nullable: true, defaultValueSql: "((0))"),
            //        New_Owner_Id = table.Column<int>(nullable: false),
            //        New_CoOwner_Id = table.Column<int>(nullable: true),
            //        Sale_Date = table.Column<DateTime>(type: "datetime", nullable: true),
            //        Old_Owner_Id = table.Column<int>(nullable: false),
            //        Old_CoOwner_Id = table.Column<int>(nullable: true),
            //        Comments = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
            //        CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true),
            //        ModifiedBy = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Transfers", x => x.id)
            //            .Annotation("SqlServer:Clustered", false);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserLoginHistory",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        LoginName = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        LoggedIn = table.Column<DateTime>(type: "datetime", nullable: true),
            //        LoggedOut = table.Column<DateTime>(type: "datetime", nullable: true),
            //        Version = table.Column<string>(unicode: false, maxLength: 25, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserLoginHistory", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        UserName = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
            //        LoginName = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
            //        Password = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
            //        DogsGroup = table.Column<string>(unicode: false, maxLength: 1, nullable: true, defaultValueSql: "('X')"),
            //        OwnersGroup = table.Column<string>(unicode: false, maxLength: 1, nullable: true, defaultValueSql: "('X')"),
            //        LittersGroup = table.Column<string>(unicode: false, maxLength: 1, nullable: true, defaultValueSql: "('X')"),
            //        ShowSetupGroup = table.Column<string>(unicode: false, maxLength: 1, nullable: true, defaultValueSql: "('X')"),
            //        ShowResultsGroup = table.Column<string>(unicode: false, maxLength: 1, nullable: true, defaultValueSql: "('X')"),
            //        ClubsGroup = table.Column<string>(unicode: false, maxLength: 1, nullable: true, defaultValueSql: "('X')"),
            //        ReportsGroup = table.Column<string>(unicode: false, maxLength: 1, nullable: true, defaultValueSql: "('X')"),
            //        UsersGroup = table.Column<string>(unicode: false, maxLength: 1, nullable: true, defaultValueSql: "('X')"),
            //        LookupsGroup = table.Column<string>(unicode: false, maxLength: 1, nullable: true, defaultValueSql: "('X')"),
            //        DefaultsGroup = table.Column<string>(unicode: false, maxLength: 1, nullable: true, defaultValueSql: "('X')"),
            //        LoggedIn = table.Column<bool>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.LoginName);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "Breed",
            //    table: "Breeds",
            //    column: "Breed",
            //    unique: true,
            //    filter: "[Breed] IS NOT NULL");

            //migrationBuilder.CreateIndex(
            //    name: "id",
            //    table: "Breeds",
            //    column: "id");

            //migrationBuilder.CreateIndex(
            //    name: "Color",
            //    table: "Colors",
            //    column: "Color",
            //    unique: true,
            //    filter: "[Color] IS NOT NULL");

            //migrationBuilder.CreateIndex(
            //    name: "id",
            //    table: "Defaults",
            //    column: "id");

            //migrationBuilder.CreateIndex(
            //    name: "LastClassId",
            //    table: "Defaults",
            //    column: "LastClassId");

            //migrationBuilder.CreateIndex(
            //    name: "LasstShowId",
            //    table: "Defaults",
            //    column: "LastShowId");

            //migrationBuilder.CreateIndex(
            //    name: "NEWID",
            //    table: "Dogs",
            //    column: "Bully_Id");

            //migrationBuilder.CreateIndex(
            //    name: "CoOwnerId",
            //    table: "Dogs",
            //    column: "CoOwner_Id");

            //migrationBuilder.CreateIndex(
            //    name: "DogsDam_No",
            //    table: "Dogs",
            //    column: "Dam_No");

            //migrationBuilder.CreateIndex(
            //    name: "Name",
            //    table: "Dogs",
            //    column: "DogName");

            //migrationBuilder.CreateIndex(
            //    name: "Owner_Id",
            //    table: "Dogs",
            //    column: "Owner_Id");

            //migrationBuilder.CreateIndex(
            //    name: "DogsSire_No",
            //    table: "Dogs",
            //    column: "Sire_No");

            //migrationBuilder.CreateIndex(
            //    name: "Dam_No",
            //    table: "Litters",
            //    column: "Dam_No");

            //migrationBuilder.CreateIndex(
            //    name: "id",
            //    table: "Litters",
            //    column: "id");

            //migrationBuilder.CreateIndex(
            //    name: "Litter_Id",
            //    table: "Litters",
            //    column: "Litter_Id");

            //migrationBuilder.CreateIndex(
            //    name: "Owner_id",
            //    table: "Litters",
            //    column: "Owner_id");

            //migrationBuilder.CreateIndex(
            //    name: "Sire_No",
            //    table: "Litters",
            //    column: "Sire_No");

            //migrationBuilder.CreateIndex(
            //    name: "id",
            //    table: "Owners",
            //    column: "id",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "Last_Name1",
            //    table: "Owners",
            //    column: "Last_Name");

            //migrationBuilder.CreateIndex(
            //    name: "Owner_Id",
            //    table: "Owners",
            //    column: "Owner_Id");

            //migrationBuilder.CreateIndex(
            //    name: "Dog_Id",
            //    table: "Transfers",
            //    column: "Dog_Id");

            //migrationBuilder.CreateIndex(
            //    name: "id",
            //    table: "Transfers",
            //    column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Breeds");

            //migrationBuilder.DropTable(
            //    name: "Clubs");

            //migrationBuilder.DropTable(
            //    name: "Colors");

            //migrationBuilder.DropTable(
            //    name: "Defaults");

            //migrationBuilder.DropTable(
            //    name: "Dogs");

            //migrationBuilder.DropTable(
            //    name: "JrHandlers");

            //migrationBuilder.DropTable(
            //    name: "Litters");

            //migrationBuilder.DropTable(
            //    name: "ManualUpdateLog");

            //migrationBuilder.DropTable(
            //    name: "Owners");

            //migrationBuilder.DropTable(
            //    name: "Shows");

            //migrationBuilder.DropTable(
            //    name: "Transfers");

            //migrationBuilder.DropTable(
            //    name: "UserLoginHistory");

            //migrationBuilder.DropTable(
            //    name: "Users");
        }
    }
}
