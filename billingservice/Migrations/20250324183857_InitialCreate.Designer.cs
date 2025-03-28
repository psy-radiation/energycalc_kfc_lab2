﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using billingservice;

#nullable disable

namespace billingservice.Migrations
{
    [DbContext(typeof(AddDbContext))]
    [Migration("20250324183857_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("billingservice.MeterReading", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("CurrentDay")
                        .HasColumnType("REAL");

                    b.Property<double>("CurrentNight")
                        .HasColumnType("REAL");

                    b.Property<string>("MeterId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("PreviousDay")
                        .HasColumnType("REAL");

                    b.Property<double>("PreviousNight")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("ReadingDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("MeterId");

                    b.ToTable("MeterReadings");
                });
#pragma warning restore 612, 618
        }
    }
}
