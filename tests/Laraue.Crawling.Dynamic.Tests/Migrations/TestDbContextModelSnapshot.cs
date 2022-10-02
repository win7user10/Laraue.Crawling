﻿// <auto-generated />
using System;
using Laraue.Crawling.Dynamic.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Laraue.Crawling.Dynamic.Tests.Migrations
{
    [DbContext(typeof(TestDbContext))]
    partial class TestDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Laraue.Crawling.Dynamic.Tests.CianPage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FloorNumber")
                        .HasColumnType("integer");

                    b.Property<int?>("MinutesToStop")
                        .HasColumnType("integer");

                    b.Property<int>("OneMeterPrice")
                        .HasColumnType("integer");

                    b.Property<string>("PublicTransportStop")
                        .HasColumnType("text");

                    b.Property<int>("RoomsCount")
                        .HasColumnType("integer");

                    b.Property<decimal>("Square")
                        .HasColumnType("numeric");

                    b.Property<int>("TotalFloorsNumber")
                        .HasColumnType("integer");

                    b.Property<int>("TotalPrice")
                        .HasColumnType("integer");

                    b.Property<int?>("TransportDistanceType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("CianPages");
                });
#pragma warning restore 612, 618
        }
    }
}