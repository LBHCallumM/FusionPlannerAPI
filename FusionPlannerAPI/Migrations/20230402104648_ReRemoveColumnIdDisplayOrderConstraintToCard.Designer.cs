﻿// <auto-generated />
using System;
using FusionPlannerAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FusionPlannerAPI.Migrations
{
    [DbContext(typeof(PlannerDbContext))]
    [Migration("20230402104648_ReRemoveColumnIdDisplayOrderConstraintToCard")]
    partial class ReRemoveColumnIdDisplayOrderConstraintToCard
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FusionPlannerAPI.Infrastructure.Board", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("OwnerId")
                        .HasColumnType("integer")
                        .HasColumnName("owner_id");

                    b.HasKey("Id")
                        .HasName("pk_boards");

                    b.HasIndex("OwnerId")
                        .HasDatabaseName("ix_boards_owner_id");

                    b.ToTable("boards", (string)null);
                });

            modelBuilder.Entity("FusionPlannerAPI.Infrastructure.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ColumnId")
                        .HasColumnType("integer")
                        .HasColumnName("column_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<int>("CreatedById")
                        .HasColumnType("integer")
                        .HasColumnName("created_by_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("integer")
                        .HasColumnName("display_order");

                    b.Property<bool>("IsArchived")
                        .HasColumnType("boolean")
                        .HasColumnName("is_archived");

                    b.Property<DateTime>("LastEditedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_edited_at");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_cards");

                    b.HasIndex("ColumnId")
                        .HasDatabaseName("ix_cards_column_id");

                    b.HasIndex("CreatedById")
                        .HasDatabaseName("ix_cards_created_by_id");

                    b.ToTable("cards", (string)null);
                });

            modelBuilder.Entity("FusionPlannerAPI.Infrastructure.Column", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BoardId")
                        .HasColumnType("integer")
                        .HasColumnName("board_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_columns");

                    b.HasIndex("BoardId")
                        .HasDatabaseName("ix_columns_board_id");

                    b.ToTable("columns", (string)null);
                });

            modelBuilder.Entity("FusionPlannerAPI.Infrastructure.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("FusionPlannerAPI.Infrastructure.Board", b =>
                {
                    b.HasOne("FusionPlannerAPI.Infrastructure.User", "Owner")
                        .WithMany("Boards")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_boards_users_owner_id");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("FusionPlannerAPI.Infrastructure.Card", b =>
                {
                    b.HasOne("FusionPlannerAPI.Infrastructure.Column", "Column")
                        .WithMany("Cards")
                        .HasForeignKey("ColumnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_cards_columns_column_id");

                    b.HasOne("FusionPlannerAPI.Infrastructure.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_cards_users_created_by_id");

                    b.Navigation("Column");

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("FusionPlannerAPI.Infrastructure.Column", b =>
                {
                    b.HasOne("FusionPlannerAPI.Infrastructure.Board", "Board")
                        .WithMany("Columns")
                        .HasForeignKey("BoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_columns_boards_board_id");

                    b.Navigation("Board");
                });

            modelBuilder.Entity("FusionPlannerAPI.Infrastructure.Board", b =>
                {
                    b.Navigation("Columns");
                });

            modelBuilder.Entity("FusionPlannerAPI.Infrastructure.Column", b =>
                {
                    b.Navigation("Cards");
                });

            modelBuilder.Entity("FusionPlannerAPI.Infrastructure.User", b =>
                {
                    b.Navigation("Boards");
                });
#pragma warning restore 612, 618
        }
    }
}
