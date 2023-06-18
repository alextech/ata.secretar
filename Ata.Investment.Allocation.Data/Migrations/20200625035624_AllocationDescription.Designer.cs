﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ata.Investment.Allocation.Data;

namespace Ata.Investment.Allocation.Data.Migrations
{
    [DbContext(typeof(AllocationContext))]
    [Migration("20200625035624_AllocationDescription")]
    partial class AllocationDescription
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("FundsV1")
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Ata.Investment.Allocation.Domain.Allocation", b =>
                {
                    b.Property<int>("RiskLevel")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RiskLevel");

                    b.ToTable("Allocations");

                    b.HasData(
                        new
                        {
                            RiskLevel = 1,
                            Guid = new Guid("6e7a5d25-1798-4331-84a1-bda4183bbdcd"),
                            Name = "Safety"
                        },
                        new
                        {
                            RiskLevel = 2,
                            Guid = new Guid("2571d584-8cc7-468c-9e08-a519a650a9ff"),
                            Name = "Conservative Income"
                        },
                        new
                        {
                            RiskLevel = 3,
                            Guid = new Guid("7ddb85b7-164e-452e-b894-9cb80d42ca6b"),
                            Name = "Balanced"
                        },
                        new
                        {
                            RiskLevel = 4,
                            Guid = new Guid("4db0f32b-a1ed-4093-a866-e576ac55cc2c"),
                            Name = "Growth"
                        },
                        new
                        {
                            RiskLevel = 5,
                            Guid = new Guid("6124468e-7540-4b10-838a-3d6e696d0683"),
                            Name = "Aggressive Growth"
                        });
                });

            modelBuilder.Entity("Ata.Investment.Allocation.Domain.AllocationVersion", b =>
                {
                    b.Property<int>("RiskLevel")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RiskLevel", "Version");

                    b.ToTable("AllocationVersions");
                });

            modelBuilder.Entity("Ata.Investment.Allocation.Domain.Composition.AllocationOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AllocationVersionRiskLevel")
                        .HasColumnType("int");

                    b.Property<int?>("AllocationVersionVersion")
                        .HasColumnType("int");

                    b.Property<int?>("OptionId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OptionId");

                    b.HasIndex("AllocationVersionRiskLevel", "AllocationVersionVersion");

                    b.ToTable("AllocationOptions");
                });

            modelBuilder.Entity("Ata.Investment.Allocation.Domain.Composition.Option", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OptionNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Option");
                });

            modelBuilder.Entity("Ata.Investment.Allocation.Domain.Fund", b =>
                {
                    b.Property<string>("FundCode")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("FundCode");

                    b.ToTable("Funds");
                });

            modelBuilder.Entity("Ata.Investment.Allocation.Domain.VersionDraft", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Draft")
                        .HasColumnType("xml");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Version");

                    b.ToTable("VersionDrafts");
                });

            modelBuilder.Entity("Ata.Investment.Allocation.Domain.AllocationVersion", b =>
                {
                    b.HasOne("Ata.Investment.Allocation.Domain.Allocation", null)
                        .WithMany("_history")
                        .HasForeignKey("RiskLevel")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Ata.Investment.Allocation.Domain.Composition.AllocationOption", b =>
                {
                    b.HasOne("Ata.Investment.Allocation.Domain.Composition.Option", "Option")
                        .WithMany()
                        .HasForeignKey("OptionId");

                    b.HasOne("Ata.Investment.Allocation.Domain.AllocationVersion", null)
                        .WithMany("Options")
                        .HasForeignKey("AllocationVersionRiskLevel", "AllocationVersionVersion");

                    b.OwnsMany("Ata.Investment.Allocation.Domain.Composition.CompositionPart", "CompositionParts", b1 =>
                        {
                            b1.Property<int>("OptionId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("FundCode")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<int>("Percent")
                                .HasColumnType("int");

                            b1.HasKey("OptionId", "Id");

                            b1.HasIndex("FundCode");

                            b1.ToTable("CompositionPart");

                            b1.HasOne("Ata.Investment.Allocation.Domain.Fund", "Fund")
                                .WithMany()
                                .HasForeignKey("FundCode");

                            b1.WithOwner()
                                .HasForeignKey("OptionId");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
