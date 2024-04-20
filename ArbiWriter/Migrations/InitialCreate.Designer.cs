﻿// <auto-generated />
using System;
using ArbiWriter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ArbiWriter.Migrations
{
    [DbContext(typeof(ArbiDbContext))]
    [Migration("20240420095527_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ArbiWriter.Models.ExchangeEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Url")
                        .HasColumnType("longtext");

                    b.Property<bool>("Working")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Exchanges");
                });

            modelBuilder.Entity("ArbiWriter.Models.ExchangeToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<double?>("Ask")
                        .HasColumnType("double");

                    b.Property<double?>("AskVolume")
                        .HasColumnType("double");

                    b.Property<double?>("Bid")
                        .HasColumnType("double");

                    b.Property<double?>("BidVolume")
                        .HasColumnType("double");

                    b.Property<double?>("DayVolumeUSDT")
                        .HasColumnType("double");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ExchangeId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FullSymbolName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Updated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValueSql("current_timestamp(6)");

                    b.HasKey("Id");

                    b.HasIndex("ExchangeId");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("ArbiWriter.Models.ExchangeToken", b =>
                {
                    b.HasOne("ArbiWriter.Models.ExchangeEntity", "Exchange")
                        .WithMany("Tokens")
                        .HasForeignKey("ExchangeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exchange");
                });

            modelBuilder.Entity("ArbiWriter.Models.ExchangeEntity", b =>
                {
                    b.Navigation("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}