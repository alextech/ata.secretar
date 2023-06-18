﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ata.Investment.Allocation.Data;

namespace Ata.Investment.Allocation.Data.Migrations
{
    [DbContext(typeof(AllocationContext))]
    partial class AllocationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("FundsV1")
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Ata.Investment.Allocation.Domain.Allocation", b =>
                {
                    b.Property<int>("RiskLevel")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

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
                            Guid = new Guid("a2a62e17-23ed-490f-b8be-a2b175251e04"),
                            Name = "Safety"
                        },
                        new
                        {
                            RiskLevel = 2,
                            Guid = new Guid("638a769e-ad51-497b-929e-95dcf457ad52"),
                            Name = "Conservative Income"
                        },
                        new
                        {
                            RiskLevel = 3,
                            Guid = new Guid("f7c6f32b-3d12-4ed9-906a-bc4d9d5521d8"),
                            Name = "Balanced"
                        },
                        new
                        {
                            RiskLevel = 4,
                            Guid = new Guid("c4880dc3-dd6b-4db1-b1e0-db7a6991287e"),
                            Name = "Growth"
                        },
                        new
                        {
                            RiskLevel = 5,
                            Guid = new Guid("33f25fd1-9a35-40b2-961e-19e1134e4338"),
                            Name = "Aggressive Growth"
                        });
                });

            modelBuilder.Entity("Ata.Investment.Allocation.Domain.AllocationVersion", b =>
                {
                    b.Property<int>("RiskLevel")
                        .HasColumnType("int");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.Property<bool>("IsListed")
                        .HasColumnType("bit");

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
                        .UseIdentityColumn();

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
                        .UseIdentityColumn();

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
                        .UseIdentityColumn();

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Draft")
                        .HasColumnType("nvarchar(max)");

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
                                .UseIdentityColumn();

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

                            b1.Navigation("Fund");
                        });

                    b.Navigation("CompositionParts");

                    b.Navigation("Option");
                });

            modelBuilder.Entity("Ata.Investment.Allocation.Domain.Allocation", b =>
                {
                    b.Navigation("_history");
                });

            modelBuilder.Entity("Ata.Investment.Allocation.Domain.AllocationVersion", b =>
                {
                    b.Navigation("Options");
                });
#pragma warning restore 612, 618
        }
    }
}
