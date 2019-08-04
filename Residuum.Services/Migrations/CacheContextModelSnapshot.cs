﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Residuum.Services.Database;

namespace Residuum.Services.Migrations
{
    [DbContext(typeof(CacheContext))]
    partial class CacheContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Residuum.Services.Entities.BestMythicRun", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MythicRunId");

                    b.HasKey("Name");

                    b.HasIndex("MythicRunId");

                    b.ToTable("BestMythicRuns");
                });

            modelBuilder.Entity("Residuum.Services.Entities.GuildMember", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Class");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<int>("Rank");

                    b.Property<string>("Realm");

                    b.HasKey("Name");

                    b.ToTable("GuildMembers");
                });

            modelBuilder.Entity("Residuum.Services.Entities.Mythic", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Difficulty");

                    b.Property<string>("DungeonName");

                    b.Property<string>("DungeonShortName");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("ProfileUri");

                    b.Property<int>("Upgrades");

                    b.HasKey("Id");

                    b.ToTable("Mythic");
                });

            modelBuilder.Entity("Residuum.Services.Entities.Progression", b =>
                {
                    b.Property<string>("RaidName")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DetailsSummary");

                    b.Property<string>("RaidProgressName");

                    b.HasKey("RaidName");

                    b.HasIndex("DetailsSummary");

                    b.HasIndex("RaidProgressName");

                    b.ToTable("Progression");
                });

            modelBuilder.Entity("Residuum.Services.Entities.ProgressionDetails", b =>
                {
                    b.Property<string>("Summary")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("HeroicBossesKilled");

                    b.Property<int>("MythicBossesKilled");

                    b.Property<int>("NormalBossesKilled");

                    b.Property<int>("TotalBosses");

                    b.HasKey("Summary");

                    b.ToTable("ProgressionDetails");
                });

            modelBuilder.Entity("Residuum.Services.Entities.RaidProgress", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Faction");

                    b.Property<DateTime>("LastUpdated");

                    b.Property<string>("Realm");

                    b.Property<string>("Region");

                    b.Property<string>("URL");

                    b.HasKey("Name");

                    b.ToTable("RaidProgress");
                });

            modelBuilder.Entity("Residuum.Services.Entities.BestMythicRun", b =>
                {
                    b.HasOne("Residuum.Services.Entities.Mythic", "MythicRun")
                        .WithMany()
                        .HasForeignKey("MythicRunId");
                });

            modelBuilder.Entity("Residuum.Services.Entities.Progression", b =>
                {
                    b.HasOne("Residuum.Services.Entities.ProgressionDetails", "Details")
                        .WithMany()
                        .HasForeignKey("DetailsSummary");

                    b.HasOne("Residuum.Services.Entities.RaidProgress")
                        .WithMany("RaidInfo")
                        .HasForeignKey("RaidProgressName");
                });
#pragma warning restore 612, 618
        }
    }
}
