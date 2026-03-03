using System;
using CinemaSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
#nullable disable
namespace CinemaSystem.Infrastructure.Migrations
{
    [DbContext(typeof(CinemaDbContext))]
    partial class CinemaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);
            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);
            modelBuilder.Entity("CinemaSystem.Domain.Entities.CinemaHall", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");
                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");
                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");
                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");
                    b.HasKey("Id");
                    b.ToTable("CinemaHalls");
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.Movie", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");
                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");
                    b.Property<int>("DurationMinutes")
                        .HasColumnType("int");
                    b.Property<string>("Genre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");
                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");
                    b.Property<DateOnly>("ReleaseDate")
                        .HasColumnType("date");
                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");
                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");
                    b.HasKey("Id");
                    b.HasIndex("Title");
                    b.ToTable("Movies");
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");
                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");
                    b.Property<Guid>("ShowTimeId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");
                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(10,2)");
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");
                    b.HasKey("Id");
                    b.HasIndex("ShowTimeId");
                    b.HasIndex("UserId");
                    b.ToTable("Reservations");
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.ReservationSeat", b =>
                {
                    b.Property<Guid>("ReservationId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<Guid>("SeatId")
                        .HasColumnType("uniqueidentifier");
                    b.HasKey("ReservationId", "SeatId");
                    b.HasIndex("SeatId");
                    b.ToTable("ReservationSeats");
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.Seat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<Guid>("HallId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<int>("Number")
                        .HasColumnType("int");
                    b.Property<string>("Row")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");
                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");
                    b.HasKey("Id");
                    b.HasIndex("HallId", "Row", "Number")
                        .IsUnique();
                    b.ToTable("Seats");
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.ShowTime", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2");
                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");
                    b.Property<Guid>("HallId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");
                    b.Property<Guid>("MovieId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");
                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");
                    b.Property<decimal>("TicketPrice")
                        .HasColumnType("decimal(10,2)");
                    b.HasKey("Id");
                    b.HasIndex("MovieId");
                    b.HasIndex("HallId", "StartTime", "EndTime");
                    b.ToTable("ShowTimes");
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.ShowTimeSeat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<bool>("IsReserved")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);
                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");
                    b.Property<Guid>("SeatId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<Guid>("ShowTimeId")
                        .HasColumnType("uniqueidentifier");
                    b.HasKey("Id");
                    b.HasIndex("SeatId");
                    b.HasIndex("ShowTimeId", "SeatId")
                        .IsUnique();
                    b.ToTable("ShowTimeSeats");
                });
            modelBuilder.Entity("CinemaSystem.Infrastructure.Persistence.ApplicationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");
                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");
                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");
                    b.HasKey("Id");
                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");
                    b.ToTable("AspNetRoles", (string)null);
                });
            modelBuilder.Entity("CinemaSystem.Infrastructure.Persistence.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");
                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");
                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");
                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");
                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");
                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");
                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");
                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");
                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");
                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");
                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");
                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");
                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");
                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");
                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");
                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");
                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");
                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");
                    b.HasKey("Id");
                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");
                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");
                    b.ToTable("AspNetUsers", (string)null);
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");
                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");
                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");
                    b.HasKey("Id");
                    b.HasIndex("RoleId");
                    b.ToTable("AspNetRoleClaims", (string)null);
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");
                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");
                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");
                    b.HasKey("Id");
                    b.HasIndex("UserId");
                    b.ToTable("AspNetUserClaims", (string)null);
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");
                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");
                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");
                    b.HasKey("LoginProvider", "ProviderKey");
                    b.HasIndex("UserId");
                    b.ToTable("AspNetUserLogins", (string)null);
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");
                    b.HasKey("UserId", "RoleId");
                    b.HasIndex("RoleId");
                    b.ToTable("AspNetUserRoles", (string)null);
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");
                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");
                    b.HasKey("UserId", "LoginProvider", "Name");
                    b.ToTable("AspNetUserTokens", (string)null);
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.Reservation", b =>
                {
                    b.HasOne("CinemaSystem.Domain.Entities.ShowTime", "ShowTime")
                        .WithMany("Reservations")
                        .HasForeignKey("ShowTimeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("ShowTime");
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.ReservationSeat", b =>
                {
                    b.HasOne("CinemaSystem.Domain.Entities.Reservation", "Reservation")
                        .WithMany("Seats")
                        .HasForeignKey("ReservationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("CinemaSystem.Domain.Entities.Seat", "Seat")
                        .WithMany()
                        .HasForeignKey("SeatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Reservation");
                    b.Navigation("Seat");
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.Seat", b =>
                {
                    b.HasOne("CinemaSystem.Domain.Entities.CinemaHall", "Hall")
                        .WithMany("Seats")
                        .HasForeignKey("HallId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.Navigation("Hall");
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.ShowTime", b =>
                {
                    b.HasOne("CinemaSystem.Domain.Entities.CinemaHall", "Hall")
                        .WithMany("ShowTimes")
                        .HasForeignKey("HallId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.HasOne("CinemaSystem.Domain.Entities.Movie", "Movie")
                        .WithMany("ShowTimes")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.Navigation("Hall");
                    b.Navigation("Movie");
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.ShowTimeSeat", b =>
                {
                    b.HasOne("CinemaSystem.Domain.Entities.Seat", "Seat")
                        .WithMany()
                        .HasForeignKey("SeatId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    b.HasOne("CinemaSystem.Domain.Entities.ShowTime", "ShowTime")
                        .WithMany("ShowTimeSeats")
                        .HasForeignKey("ShowTimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.Navigation("Seat");
                    b.Navigation("ShowTime");
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("CinemaSystem.Infrastructure.Persistence.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("CinemaSystem.Infrastructure.Persistence.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("CinemaSystem.Infrastructure.Persistence.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("CinemaSystem.Infrastructure.Persistence.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                    b.HasOne("CinemaSystem.Infrastructure.Persistence.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("CinemaSystem.Infrastructure.Persistence.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.CinemaHall", b =>
                {
                    b.Navigation("Seats");
                    b.Navigation("ShowTimes");
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.Movie", b =>
                {
                    b.Navigation("ShowTimes");
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.Reservation", b =>
                {
                    b.Navigation("Seats");
                });
            modelBuilder.Entity("CinemaSystem.Domain.Entities.ShowTime", b =>
                {
                    b.Navigation("Reservations");
                    b.Navigation("ShowTimeSeats");
                });
#pragma warning restore 612, 618
        }
    }
}