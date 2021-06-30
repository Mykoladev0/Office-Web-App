using System;
using System.Linq;
using System.Threading.Tasks;
using CoreDAL.Models;
using CoreDAL.Models.v2;
using CoreDAL.Models.v2.Registrations;
using CoreDAL.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CoreDAL
{
    public partial class ABKCOnlineContext : DbContext
    {
        public ABKCOnlineContext()
        {
        }

        public ABKCOnlineContext(DbContextOptions<ABKCOnlineContext> options)
            : base(options) { }

        public virtual DbSet<Breeds> Breeds { get; set; }
        public virtual DbSet<Clubs> Clubs { get; set; }
        public virtual DbSet<Colors> Colors { get; set; }
        public virtual DbSet<Defaults> Defaults { get; set; }
        public virtual DbSet<Dogs> Dogs { get; set; }
        public virtual DbSet<JrHandlers> JrHandlers { get; set; }
        public virtual DbSet<Litters> Litters { get; set; }
        public virtual DbSet<ManualUpdateLog> ManualUpdateLog { get; set; }
        public virtual DbSet<Owners> Owners { get; set; }
        public virtual DbSet<Shows> Shows { get; set; }
        public virtual DbSet<Transfers> Transfers { get; set; }
        public virtual DbSet<UserLoginHistory> UserLoginHistory { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<RepresentativeModel> Representatives { get; set; }

        //not auto-generated
        public virtual DbSet<ShowResults> ShowResults { get; set; }
        public virtual DbSet<Judges> Judges { get; set; }
        public virtual DbSet<ClassTemplates> ClassTemplates { get; set; }
        public virtual DbSet<Styles> Styles { get; set; }
        public virtual DbSet<ShowParticipant> ShowParticipants { get; set; }

        // Unable to generate entity type for table 'dbo.ClassTemplates'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.WeightPullTemplate'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.WeightPullResults'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.SABResults'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.SABTemplates'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.ShowResults'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.Judges'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.FixDOB'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.Styles'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.JrHandlerTemplate'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.JrHandlerResults'. Please see the warning messages.

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            if (!optionsBuilder.IsConfigured)
        //            {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //                optionsBuilder.UseSqlServer("Server=localhost;Database=ABKC_Orig_A;Trusted_Connection=true;");
        //            }
        //        }

        #region "ABKC Online Specific DBSets"
        public virtual DbSet<RegistrationModel> Registrations { get; set; }
        public virtual DbSet<UserModel> ABKCUsers { get; set; }
        public virtual DbSet<BaseDogModel> ABKCDogs { get; set; }

        public virtual DbSet<JuniorHandlerRegistrationModel> JuniorHandlerRegistrations { get; set; }
        public virtual DbSet<PuppyRegistrationModel> PuppyRegistrations { get; set; }
        public virtual DbSet<LitterRegistrationModel> LitterRegistrations { get; set; }
        public virtual DbSet<BullyIdRequestModel> BullyIdRequests { get; set; }

        public virtual DbSet<AttachmentModel> Attachments { get; set; }

        public virtual DbSet<TransactionModel> Transactions { get; set; }
        public virtual DbSet<RefundModel> Refunds { get; set; }

        #endregion

        private void ConfigureBaseDBModel(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<BaseDBModel>(entity =>
            // {
            //     entity.Property(b => b.DateCreated).HasDefaultValueSql("getutcdate()");
            //     entity.Property(b => b.DateModified).HasDefaultValueSql("getutcdate()");
            //     entity.Property(e => e.Id).UseSqlServerIdentityColumn();
            //     entity.HasKey(e => e.Id)
            //         .ForSqlServerIsClustered(false);
            //     entity.HasIndex(e => e.Id)
            //         .IsUnique();
            // });
        }
        private void BuildABKCUsersTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.Property(b => b.DateCreated).HasDefaultValueSql("getutcdate()");
                entity.Property(b => b.DateModified).HasDefaultValueSql("getutcdate()");
                entity.Property(e => e.Id).UseSqlServerIdentityColumn();
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);
                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.HasMany(u => u.Roles).WithOne().OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.OktaId).HasMaxLength(80).IsRequired();
                entity.Property(e => e.LoginName).HasMaxLength(80).IsRequired();
                entity.ToTable("ABKCUsers");
            });
            modelBuilder.Entity<RoleType>(entity =>
            {
                entity.HasIndex(r => new
                {
                    r.Id,
                    r.Type
                }).IsUnique();

                entity.ToTable("UserRoles");
            });

            modelBuilder.Entity<RepresentativeModel>(entity =>
            {
                entity.Property(b => b.DateCreated).HasDefaultValueSql("getutcdate()");
                entity.Property(b => b.DateModified).HasDefaultValueSql("getutcdate()");
                entity.HasKey(e => e.Id).ForSqlServerIsClustered(false);
                entity.HasIndex(r => r.Id).IsUnique();

                entity.ToTable("ABKCRepresentatives");
            });
        }

        private void BuildABKCDogsTable(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<DogColorJoinModel>().HasKey(dc => new { dc.DogId, dc.ColorId });
            // modelBuilder.Entity<DogColorJoinModel>().HasOne(dc => dc.Dog).WithMany(d => d.Colors).HasForeignKey(d => d.DogId);
            // modelBuilder.Entity<DogColorJoinModel>().HasOne(dc => dc.Colors).WithMany().HasForeignKey(d => d.ColorId);
            // modelBuilder.Entity<DogColorJoinModel>().ToTable("DogColorTable");
            modelBuilder.Entity<BaseDogModel>(entity =>
       {
           entity.Property(b => b.DateCreated).HasDefaultValueSql("getutcdate()");
           entity.Property(b => b.DateModified).HasDefaultValueSql("getutcdate()");
           entity.Property(e => e.Id).UseSqlServerIdentityColumn();
           entity.HasKey(e => e.Id)
               .ForSqlServerIsClustered(false);
           entity.HasIndex(e => e.Id)
               .IsUnique();

           entity.Property(e => e.DogName).HasMaxLength(50).IsRequired();
           entity.HasOne(e => e.Owner).WithMany().OnDelete(DeleteBehavior.Restrict);//.IsRequired();
           entity.HasOne(e => e.CoOwner).WithMany().HasForeignKey("CoOwnerId").OnDelete(DeleteBehavior.Restrict);
           entity.HasOne(e => e.Breed).WithMany().OnDelete(DeleteBehavior.Restrict);//.IsRequired();
           entity.HasOne(e => e.Color).WithMany().OnDelete(DeleteBehavior.Restrict);
           entity.HasOne(e => e.Sire).WithMany().OnDelete(DeleteBehavior.Restrict);
           entity.HasOne(e => e.Dam).WithMany().OnDelete(DeleteBehavior.Restrict);
           entity.ToTable("ABKC_Dogs");
       });
        }
        private void BuildRegistrationTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseRegistrationStatusModel>(entity =>
         {
             entity.Property(b => b.DateCreated).HasDefaultValueSql("getutcdate()");
             entity.Property(b => b.DateModified).HasDefaultValueSql("getutcdate()");
             entity.Property(e => e.Id).UseSqlServerIdentityColumn();
             entity.HasKey(e => e.Id)
                 .ForSqlServerIsClustered(false);
             entity.HasIndex(e => e.Id)
                 .IsUnique();
             //  entity.HasOne(r => r.Registration).WithMany(r => r.StatusHistory).HasForeignKey("RegistrationId").IsRequired();
             entity.HasOne(b => b.StatusChangedBy).WithMany();//.IsRequired(); TODO:add back once authentication is enabled
             entity.ToTable("RegistrationStatus");
         });

            modelBuilder.Entity<AttachmentModel>(entity =>
            {
                entity.Property(b => b.DateCreated).HasDefaultValueSql("getutcdate()");
                entity.Property(b => b.DateModified).HasDefaultValueSql("getutcdate()");
                entity.Property(e => e.Id).UseSqlServerIdentityColumn();
                entity.HasIndex(e => e.Id)
                    .IsUnique();
                entity.ToTable("Attachments");
            });

            modelBuilder.Entity<RegistrationModel>(entity =>
            {
                entity.Property(b => b.DateCreated).HasDefaultValueSql("getutcdate()");
                entity.Property(b => b.DateModified).HasDefaultValueSql("getutcdate()");
                entity.Property(e => e.Id).UseSqlServerIdentityColumn();
                entity.HasIndex(e => e.Id)
                    .IsUnique();
                // entity.Property(e => e.DogInfo).IsRequired();
                entity.HasOne(e => e.DogInfo).WithMany().HasForeignKey("DogId").IsRequired().OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.StatusHistory).WithOne().HasForeignKey("DogRegistrationId").IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.AssociatedTransaction).WithMany().HasForeignKey("TransactionId");
                // entity.HasOne(e => e.DogInfo);
                // entity.HasMany(e => e.StatusHistory);
                entity.ToTable("Registrations");
            });
        }

        private void BuildJrHandlerRegistrationTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JuniorHandlerRegistrationModel>(entity =>
            {
                entity.Property(b => b.DateCreated).HasDefaultValueSql("getutcdate()");
                entity.Property(b => b.DateModified).HasDefaultValueSql("getutcdate()");
                entity.Property(e => e.Id).UseSqlServerIdentityColumn();
                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.HasMany(e => e.StatusHistory).WithOne().HasForeignKey("JrHandlerRegistrationId").IsRequired().OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<JuniorHandlerRegistrationModel>().ToTable("JrHandlerRegistrations");
        }

        private void BuildPuppyRegistrationTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PuppyRegistrationModel>(entity =>
            {
                entity.Property(b => b.DateCreated).HasDefaultValueSql("getutcdate()");
                entity.Property(b => b.DateModified).HasDefaultValueSql("getutcdate()");
                entity.Property(e => e.Id).UseSqlServerIdentityColumn();
                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.HasOne(e => e.DogInfo).WithMany().HasForeignKey("DogId").IsRequired().OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.NewOwner).WithMany().HasForeignKey("NewOwnerId").OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.NewCoOwner).WithMany().HasForeignKey("NewCoOwnerId").OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AssociatedTransaction);

                entity.HasMany(e => e.StatusHistory).WithOne().HasForeignKey("PuppyRegistrationId").IsRequired().OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.IsTransferRequest).HasDefaultValue(false);//usually will be a puppy registration, true = transfer reg
            });
            modelBuilder.Entity<PuppyRegistrationModel>().ToTable("PuppyRegistrations");
        }
        private void BuildBullyIdRegistrationsTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BullyIdRequestModel>(entity =>
            {
                entity.Property(b => b.DateCreated).HasDefaultValueSql("getutcdate()");
                entity.Property(b => b.DateModified).HasDefaultValueSql("getutcdate()");
                entity.Property(e => e.Id).UseSqlServerIdentityColumn();
                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.HasOne(e => e.DogInfo).WithMany().HasForeignKey("DogId").IsRequired().OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.StatusHistory).WithOne().HasForeignKey("PuppyRegistrationId").IsRequired().OnDelete(DeleteBehavior.Cascade);

            });
            modelBuilder.Entity<BullyIdRequestModel>().ToTable("BullyIdRequests");
        }

        private void BuildLitterRegistrationTable(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<LitterRegistrationModel>(entity =>
            {
                entity.Property(b => b.DateCreated).HasDefaultValueSql("getutcdate()");
                entity.Property(b => b.DateModified).HasDefaultValueSql("getutcdate()");
                entity.Property(e => e.Id).UseSqlServerIdentityColumn();
                entity.HasIndex(e => e.Id)
                    .IsUnique();

                entity.HasOne(e => e.Sire).WithMany().HasForeignKey("SireId").IsRequired().OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Dam).WithMany().HasForeignKey("DamId").OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.LitterFromRegistration);

                entity.HasMany(e => e.StatusHistory).WithOne().HasForeignKey("LitterRegistrationId").IsRequired().OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<PuppyRegistrationModel>().ToTable("PuppyRegistrations");
        }


        private void BuildTransactionsTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionModel>(entity =>
            {
                entity.ToTable("Transactions");
                entity.Property(b => b.DateCreated).HasDefaultValueSql("getutcdate()");
                entity.Property(b => b.DateModified).HasDefaultValueSql("getutcdate()");
                entity.Property(e => e.Id).UseSqlServerIdentityColumn();
                entity.HasIndex(e => e.Id)
                    .IsUnique();
                entity.HasOne(e => e.ChargedTo);

            });
            modelBuilder.Entity<RefundModel>(entity =>
            {
                entity.ToTable("Refunds");
                entity.Property(b => b.DateCreated).HasDefaultValueSql("getutcdate()");
                entity.Property(b => b.DateModified).HasDefaultValueSql("getutcdate()");
                entity.Property(e => e.Id).UseSqlServerIdentityColumn();
                entity.HasIndex(e => e.Id)
                    .IsUnique();
                entity.HasOne(e => e.IssuedBy);
                entity.HasOne(e => e.RefundedTo);
                entity.HasOne(e => e.OriginalTransaction);

            });
        }

        private void BuildShowResultsTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShowResults>(entity =>
            {
                entity.ToTable("ShowResults");
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.Id)
                    .HasName("id");
                entity.HasOne<ClassTemplates>(c => c.ClassTemplate).WithMany().HasConstraintName("ClassTemplateId").IsRequired(false);
                entity.HasOne<Shows>(c => c.Show).WithMany().HasConstraintName("ShowId").IsRequired(true);
                entity.HasOne<Styles>(c => c.StyleRef).WithMany().HasConstraintName("StyleId").IsRequired(false);

                entity.Property(p => p.ModifyDate).HasColumnType("datetime2");
                entity.Property(p => p.CreateDate).HasColumnType("datetime2");
            });
        }

        private void BuildShowParticipantsTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShowParticipant>(entity =>
            {
                entity.ToTable("ShowParticipants");
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasOne<Dogs>(c => c.Dog).WithMany().IsRequired(true);
                entity.HasOne<Shows>(c => c.Show).WithMany().IsRequired(true);
                entity.Property(p => p.ArmbandNumber).HasDefaultValue(null);
                entity.Property(p => p.DateRegistered).HasColumnType("datetime2").IsRequired(true);

            });
        }


        private void BuildJudgesTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Judges>(entity =>
            {
                entity.ToTable("Judges");
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.Id)
                    .HasName("id");

            });
        }

        private void BuildClassAndStylesTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassTemplates>(entity =>
            {
                entity.ToTable("ClassTemplates");
                entity.HasKey(e => e.ClassId)
                    .ForSqlServerIsClustered(false);
                entity.Property(c => c.Gender).HasColumnName("Gender").HasDefaultValue("").IsRequired(false);
                entity.HasIndex(e => e.ClassId)
                    .HasName("ClassId");

            });
            //TODO:pull styles from DB
            modelBuilder.Entity<Styles>(entity =>
            {
                entity.ToTable("Styles");
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.Id)
                    .HasName("id");

            });
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //or configure all DateTime Preperties globally(EF 6 and Above)
            //foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties())
            //    .Where(p => p.ClrType == typeof(DateTime)))
            //{
            //    property.Relational().ColumnType = "datetime2";
            //}

            BuildOriginalTables(modelBuilder);

            BuildShowResultsTable(modelBuilder);
            BuildJudgesTable(modelBuilder);
            BuildClassAndStylesTable(modelBuilder);
            BuildShowParticipantsTable(modelBuilder);

            //ABKC Online Wiring
            ConfigureBaseDBModel(modelBuilder);
            BuildABKCUsersTable(modelBuilder);
            BuildABKCDogsTable(modelBuilder);
            BuildRegistrationTable(modelBuilder);
            BuildJrHandlerRegistrationTable(modelBuilder);
            BuildPuppyRegistrationTable(modelBuilder);
            BuildLitterRegistrationTable(modelBuilder);

            //SEED TEST DATA
            // DogRegistrationSeed.AddPendingRegistrations(modelBuilder).Wait();
        }

        private void BuildOriginalTables(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Breeds>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.Breed)
                    .HasName("Breed")
                    .IsUnique();

                entity.HasIndex(e => e.Id)
                    .HasName("id");

                entity.Property(e => e.Breed).IsUnicode(false);
            });

            modelBuilder.Entity<Clubs>(entity =>
            {
                entity.Property(e => e.ClubId).ValueGeneratedNever();

                entity.Property(e => e.Address1).IsUnicode(false);

                entity.Property(e => e.Address2).IsUnicode(false);

                entity.Property(e => e.Address3).IsUnicode(false);

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.ClubName).IsUnicode(false);

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.Country).IsUnicode(false);

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ModifiedBy).IsUnicode(false);

                entity.Property(e => e.PresContact).IsUnicode(false);

                entity.Property(e => e.President).IsUnicode(false);

                entity.Property(e => e.State).IsUnicode(false);

                entity.Property(e => e.VicePresident).IsUnicode(false);

                entity.Property(e => e.Vpcontact).IsUnicode(false);

                entity.Property(e => e.WebAddress).IsUnicode(false);

                entity.Property(e => e.Zip).IsUnicode(false);
            });

            modelBuilder.Entity<Colors>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.Color)
                    .HasName("Color")
                    .IsUnique();

                entity.Property(e => e.Color).IsUnicode(false);
            });

            modelBuilder.Entity<Defaults>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.Id)
                    .HasName("id");

                entity.HasIndex(e => e.LastClassId)
                    .HasName("LastClassId");

                entity.HasIndex(e => e.LastShowId)
                    .HasName("LasstShowId");

                entity.Property(e => e.LastAbkcno).HasDefaultValueSql("((0))");

                entity.Property(e => e.LastBullyId).HasDefaultValueSql("((0))");

                entity.Property(e => e.LastClassId).HasDefaultValueSql("((0))");

                entity.Property(e => e.LastLitterId).HasDefaultValueSql("((0))");

                entity.Property(e => e.LastOwnerId).HasDefaultValueSql("((0))");

                entity.Property(e => e.LastShowId).HasDefaultValueSql("((0))");

                entity.Property(e => e.MergeDataPath).IsUnicode(false);

                entity.Property(e => e.PedCustomPaperSize).IsUnicode(false);

                entity.Property(e => e.PuppyRegCustomPaperSize).IsUnicode(false);

                entity.Property(e => e.RegFee).HasDefaultValueSql("((0))");

                entity.Property(e => e.ReportsPath).IsUnicode(false);

                entity.Property(e => e.UserName).IsUnicode(false);
            });

            modelBuilder.Entity<Dogs>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.BullyId)
                    .HasName("NEWID");

                entity.HasIndex(e => e.CoOwnerId)
                    .HasName("CoOwnerId");

                entity.HasIndex(e => e.DamNo)
                    .HasName("DogsDam_No");

                entity.HasIndex(e => e.DogName)
                    .HasName("Name");

                entity.HasIndex(e => e.OwnerId)
                    .HasName("Owner_Id");

                entity.HasIndex(e => e.SireNo)
                    .HasName("DogsSire_No");

                entity.Property(e => e.AbkcNo).IsUnicode(false);

                entity.Property(e => e.AdbaNo).IsUnicode(false);

                entity.Property(e => e.AkcNo).IsUnicode(false);

                entity.Property(e => e.Bdna).IsUnicode(false);

                entity.Property(e => e.Breed)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.BullyId).HasDefaultValueSql("((0))");

                entity.Property(e => e.ChampPoints).HasDefaultValueSql("((0))");

                entity.Property(e => e.ChampWins).HasDefaultValueSql("((0))");

                entity.Property(e => e.ChipNo).IsUnicode(false);

                entity.Property(e => e.CoOwnerId).HasDefaultValueSql("((1))");

                entity.Property(e => e.Color)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.DamNo).HasDefaultValueSql("((1))");

                entity.Property(e => e.Degrees).IsUnicode(false);

                entity.Property(e => e.DogName).IsUnicode(false);

                entity.Property(e => e.Gender)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.LitterNo).HasDefaultValueSql("((0))");

                entity.Property(e => e.Majors).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).IsUnicode(false);

                entity.Property(e => e.OrigTitle).IsUnicode(false);

                entity.Property(e => e.OtherNo).IsUnicode(false);

                entity.Property(e => e.OwnerId).HasDefaultValueSql("((1))");

                entity.Property(e => e.Points).HasDefaultValueSql("((0))");

                entity.Property(e => e.Pups).HasDefaultValueSql("((0))");

                entity.Property(e => e.RegAmt).HasDefaultValueSql("((0))");

                entity.Property(e => e.RegNo).IsUnicode(false);

                entity.Property(e => e.RegType).IsUnicode(false);

                entity.Property(e => e.RegUser).IsUnicode(false);

                entity.Property(e => e.SireNo).HasDefaultValueSql("((1))");

                entity.Property(e => e.Suffix).IsUnicode(false);

                entity.Property(e => e.TatooNo).IsUnicode(false);

                entity.Property(e => e.Title).IsUnicode(false);

                entity.Property(e => e.UkcNo).IsUnicode(false);

                entity.Property(e => e.WpTitle).IsUnicode(false);
                entity.HasIndex(e => e.Id).IsUnique();
                entity.HasIndex(e => new { e.Id, e.BullyId, e.AbkcNo, e.DogName }).ForSqlServerIsClustered(false);
            });

            modelBuilder.Entity<JrHandlers>(entity =>
            {
                entity.Property(e => e.JrHandlerId).ValueGeneratedNever();

                entity.Property(e => e.Address1).IsUnicode(false);

                entity.Property(e => e.Address2).IsUnicode(false);

                entity.Property(e => e.Address3).IsUnicode(false);

                entity.Property(e => e.Cell).IsUnicode(false);

                entity.Property(e => e.ChildFirstName).IsUnicode(false);

                entity.Property(e => e.ChildLastName).IsUnicode(false);

                entity.Property(e => e.ChildName).IsUnicode(false);

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.Country).IsUnicode(false);

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ModifiedBy).IsUnicode(false);

                entity.Property(e => e.ParentFirstName).IsUnicode(false);

                entity.Property(e => e.ParentLastName).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.Property(e => e.State).IsUnicode(false);

                entity.Property(e => e.Zip).IsUnicode(false);
            });

            modelBuilder.Entity<Litters>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.DamNo)
                    .HasName("Dam_No");

                entity.HasIndex(e => e.Id)
                    .HasName("id");

                entity.HasIndex(e => e.LitterId)
                    .HasName("Litter_Id");

                entity.HasIndex(e => e.OwnerId)
                    .HasName("Owner_id");

                entity.HasIndex(e => e.SireNo)
                    .HasName("Sire_No");

                entity.Property(e => e.Breed).IsUnicode(false);

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.Females).HasDefaultValueSql("((0))");

                entity.Property(e => e.Males).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).IsUnicode(false);
            });

            modelBuilder.Entity<ManualUpdateLog>(entity =>
            {
                entity.Property(e => e.AbkcNo).IsUnicode(false);

                entity.Property(e => e.DogName).IsUnicode(false);

                entity.Property(e => e.InsertedBy).IsUnicode(false);

                entity.Property(e => e.Title).IsUnicode(false);
            });

            modelBuilder.Entity<Owners>(entity =>
            {
                entity.HasKey(e => e.OwnerId)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.Id)
                    .HasName("id")
                    .IsUnique();

                entity.HasIndex(e => e.LastName)
                    .HasName("Last_Name1");

                entity.HasIndex(e => e.OwnerId)
                    .HasName("Owner_Id");

                entity.Property(e => e.OwnerId).ValueGeneratedNever();

                entity.Property(e => e.Address1).IsUnicode(false);

                entity.Property(e => e.Address2).IsUnicode(false);

                entity.Property(e => e.Address3).IsUnicode(false);

                entity.Property(e => e.Cell).IsUnicode(false);

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.Country).IsUnicode(false);

                entity.Property(e => e.DualSignature).HasDefaultValueSql("((0))");

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                //entity.Property(e => e.FullName).IsUnicode(false);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.MiddleInitial).IsUnicode(false);

                entity.Property(e => e.ModifiedBy).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.Property(e => e.State)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.WebPassword).IsUnicode(false);

                entity.Property(e => e.Zip).IsUnicode(false);
            });

            modelBuilder.Entity<Shows>(entity =>
            {
                entity.HasKey(e => e.ShowId)
                    .ForSqlServerIsClustered(false);

                entity.Property(e => e.Abkcrep).IsUnicode(false);

                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.BreedsShown).IsUnicode(false);

                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.InsurancePolicy).IsUnicode(false);

                entity.Property(e => e.Judge1).IsUnicode(false);

                entity.Property(e => e.Judge2).IsUnicode(false);

                entity.Property(e => e.ModifiedBy).IsUnicode(false);

                entity.Property(e => e.Paperwork1).IsUnicode(false);

                entity.Property(e => e.Paperwork2).IsUnicode(false);

                entity.Property(e => e.RingSteward).IsUnicode(false);

                entity.Property(e => e.ShowName).IsUnicode(false);

                entity.Property(e => e.State).IsUnicode(false);

                entity.Property(e => e.StylesShown).IsUnicode(false);

                entity.Property(e => e.Zip).IsUnicode(false);
            });

            modelBuilder.Entity<Transfers>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.HasIndex(e => e.DogId)
                    .HasName("Dog_Id");

                entity.HasIndex(e => e.Id)
                    .HasName("id");

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.DogId).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).IsUnicode(false);
            });

            modelBuilder.Entity<UserLoginHistory>(entity =>
            {
                entity.Property(e => e.LoginName).IsUnicode(false);

                entity.Property(e => e.Version).IsUnicode(false);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.LoginName)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.ClubsGroup)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('X')");

                entity.Property(e => e.DefaultsGroup)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('X')");

                entity.Property(e => e.DogsGroup)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('X')");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LittersGroup)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('X')");

                entity.Property(e => e.LookupsGroup)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('X')");

                entity.Property(e => e.OwnersGroup)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('X')");

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.ReportsGroup)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('X')");

                entity.Property(e => e.ShowResultsGroup)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('X')");

                entity.Property(e => e.ShowSetupGroup)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('X')");

                entity.Property(e => e.UserName).IsUnicode(false);

                entity.Property(e => e.UsersGroup)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('X')");
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            AddAuditInfo();
            return await base.SaveChangesAsync();
        }
        //watch out for deletes
        private void AddAuditInfo()
        {
            var entries = ChangeTracker.Entries().Where(x => x.Entity is BaseDBModel && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                // if (entry.State == EntityState.Added)
                // {
                //     ((BaseDBObject)entry.Entity).DateCreated = DateTime.UtcNow;
                // }
                ((BaseDBModel)entry.Entity).DateModified = DateTime.UtcNow;
            }
        }
    }
}
