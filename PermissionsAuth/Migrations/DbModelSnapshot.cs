﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PermissionsAuth.Data;

namespace PermissionsAuth.Migrations
{
    [DbContext(typeof(Db))]
    partial class DbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.7");

            modelBuilder.Entity("PermissionsAuth.Data.AccessToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ExpiresIn")
                        .HasColumnType("int");

                    b.Property<string>("Scope")
                        .HasColumnType("longtext");

                    b.Property<string>("Token")
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("PermissionsAuth.Data.UserAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Auth0UserId")
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("Mobile")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("UserAccounts");
                });

            modelBuilder.Entity("PermissionsAuth.Data.UserAccountUserRole", b =>
                {
                    b.Property<int>("UserAccountId")
                        .HasColumnType("int");

                    b.Property<int>("UserRoleId")
                        .HasColumnType("int");

                    b.HasKey("UserAccountId", "UserRoleId");

                    b.HasIndex("UserRoleId");

                    b.ToTable("UserAccountUserRole");
                });

            modelBuilder.Entity("PermissionsAuth.Data.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp(6)");

                    b.Property<string>("_Permissions")
                        .HasColumnType("longtext")
                        .HasColumnName("Permissions");

                    b.HasKey("Id");

                    b.ToTable("UserRoles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "SuperAdmin",
                            _Permissions = "[\"Permissions.All\"]"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Admin",
                            _Permissions = "[\"Permissions.Users.View\"]"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Basic",
                            _Permissions = "[]"
                        });
                });

            modelBuilder.Entity("PermissionsAuth.Data.UserAccountUserRole", b =>
                {
                    b.HasOne("PermissionsAuth.Data.UserAccount", "UserAccount")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PermissionsAuth.Data.UserRole", "UserRole")
                        .WithMany("UserAccounts")
                        .HasForeignKey("UserRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserAccount");

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("PermissionsAuth.Data.UserAccount", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("PermissionsAuth.Data.UserRole", b =>
                {
                    b.Navigation("UserAccounts");
                });
#pragma warning restore 612, 618
        }
    }
}