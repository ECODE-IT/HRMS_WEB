﻿// <auto-generated />
using System;
using HRMS_WEB.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HRMS_WEB.Migrations
{
    [DbContext(typeof(HRMSDbContext))]
    [Migration("20201028164830_add sublevel")]
    partial class addsublevel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("HRMS_WEB.Entities.DutyLog", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("IsDutyOn")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("LogDateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("DutyLogs");
                });

            modelBuilder.Entity("HRMS_WEB.Entities.SubLevel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime(6)");

                    b.Property<double>("ManHours")
                        .HasColumnType("double");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Remark")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("SubLevels");
                });

            modelBuilder.Entity("HRMS_WEB.Entities.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UserPassword")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HRMS_WEB.Entities.DutyLog", b =>
                {
                    b.HasOne("HRMS_WEB.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HRMS_WEB.Entities.SubLevel", b =>
                {
                    b.HasOne("HRMS_WEB.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
