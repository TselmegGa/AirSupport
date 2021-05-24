﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pitstop.LuggageManagment.DataAccess;

namespace Pitstop.LuggageManagment.Migrations
{
    [DbContext(typeof(LuggageManagmentDBContext))]
    [Migration("20210524193839_LuggageManagment")]
    partial class LuggageManagment
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Pitstop.LuggageManagment.Model.Luggage", b =>
                {
                    b.Property<int>("LuggageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PassengerId")
                        .HasColumnType("int");

                    b.Property<int>("Weight")
                        .HasColumnType("int");

                    b.HasKey("LuggageId");

                    b.HasIndex("PassengerId");

                    b.ToTable("Luggage");
                });

            modelBuilder.Entity("Pitstop.LuggageManagment.Model.Passenger", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<bool>("Arrived")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Passenger");
                });

            modelBuilder.Entity("Pitstop.LuggageManagment.Model.Luggage", b =>
                {
                    b.HasOne("Pitstop.LuggageManagment.Model.Passenger", null)
                        .WithMany("Luggage")
                        .HasForeignKey("PassengerId");
                });

            modelBuilder.Entity("Pitstop.LuggageManagment.Model.Passenger", b =>
                {
                    b.Navigation("Luggage");
                });
#pragma warning restore 612, 618
        }
    }
}