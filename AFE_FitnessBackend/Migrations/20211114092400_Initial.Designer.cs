﻿// <auto-generated />
using System;
using AFE_FitnessBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AFE_FitnessBackend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20211114092400_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AFE_FitnessBackend.Models.Entities.Exercise", b =>
                {
                    b.Property<long>("ExerciseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasMaxLength(4096)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<long?>("PersonalTrainerId")
                        .HasColumnType("bigint");

                    b.Property<int?>("Repetitions")
                        .HasColumnType("int");

                    b.Property<string>("Time")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<long?>("WorkoutProgramId")
                        .HasColumnType("bigint");

                    b.HasKey("ExerciseId");

                    b.HasIndex("WorkoutProgramId");

                    b.ToTable("Exercises");
                });

            modelBuilder.Entity("AFE_FitnessBackend.Models.Entities.User", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountType")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Email")
                        .HasMaxLength(254)
                        .HasColumnType("nvarchar(254)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("LastName")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<long?>("PersonalTrainerId")
                        .HasColumnType("bigint");

                    b.Property<string>("PwHash")
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.HasKey("UserId");

                    b.HasIndex("PersonalTrainerId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AFE_FitnessBackend.Models.Entities.WorkoutProgram", b =>
                {
                    b.Property<long>("WorkoutProgramId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("ClientId")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasMaxLength(4096)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<long>("PersonalTrainerId")
                        .HasColumnType("bigint");

                    b.HasKey("WorkoutProgramId");

                    b.ToTable("WorkoutPrograms");
                });

            modelBuilder.Entity("AFE_FitnessBackend.Models.Entities.Exercise", b =>
                {
                    b.HasOne("AFE_FitnessBackend.Models.Entities.WorkoutProgram", null)
                        .WithMany("Exercises")
                        .HasForeignKey("WorkoutProgramId");
                });

            modelBuilder.Entity("AFE_FitnessBackend.Models.Entities.User", b =>
                {
                    b.HasOne("AFE_FitnessBackend.Models.Entities.User", null)
                        .WithMany("Clients")
                        .HasForeignKey("PersonalTrainerId");
                });

            modelBuilder.Entity("AFE_FitnessBackend.Models.Entities.User", b =>
                {
                    b.Navigation("Clients");
                });

            modelBuilder.Entity("AFE_FitnessBackend.Models.Entities.WorkoutProgram", b =>
                {
                    b.Navigation("Exercises");
                });
#pragma warning restore 612, 618
        }
    }
}
