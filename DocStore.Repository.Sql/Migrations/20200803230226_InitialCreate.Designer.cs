﻿// <auto-generated />
using System;
using DocStore.Repository.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DocStore.Repository.Sql.Migrations
{
    [DbContext(typeof(FileSystem))]
    [Migration("20200803230226_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6");

            modelBuilder.Entity("DocStore.Repository.Sql.FileSystem+Content", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasColumnType("BLOB")
                        .HasMaxLength(2147483647);

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(128);

                    b.Property<int>("Size")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Content");
                });

            modelBuilder.Entity("DocStore.Repository.Sql.FileSystem+FileSystemItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ItemType")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(128);

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.ToTable("FileSystemItems");

                    b.HasDiscriminator<string>("Discriminator").HasValue("FileSystemItem");
                });

            modelBuilder.Entity("DocStore.Repository.Sql.FileSystem+Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ItemId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2147483647);

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("DocStore.Repository.Sql.FileSystem+Directory", b =>
                {
                    b.HasBaseType("DocStore.Repository.Sql.FileSystem+FileSystemItem");

                    b.HasIndex("ParentId");

                    b.HasDiscriminator().HasValue("Directory");
                });

            modelBuilder.Entity("DocStore.Repository.Sql.FileSystem+File", b =>
                {
                    b.HasBaseType("DocStore.Repository.Sql.FileSystem+FileSystemItem");

                    b.Property<Guid>("ContentId")
                        .HasColumnType("TEXT");

                    b.HasIndex("ContentId");

                    b.HasIndex("ParentId");

                    b.HasDiscriminator().HasValue("File");
                });

            modelBuilder.Entity("DocStore.Repository.Sql.FileSystem+Tag", b =>
                {
                    b.HasOne("DocStore.Repository.Sql.FileSystem+FileSystemItem", "Item")
                        .WithMany("Tags")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DocStore.Repository.Sql.FileSystem+Directory", b =>
                {
                    b.HasOne("DocStore.Repository.Sql.FileSystem+Directory", "ParentDirectory")
                        .WithMany("Directories")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("DocStore.Repository.Sql.FileSystem+File", b =>
                {
                    b.HasOne("DocStore.Repository.Sql.FileSystem+Content", "Content")
                        .WithMany()
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DocStore.Repository.Sql.FileSystem+Directory", "ParentDirectory")
                        .WithMany("Files")
                        .HasForeignKey("ParentId");
                });
#pragma warning restore 612, 618
        }
    }
}