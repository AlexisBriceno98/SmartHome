﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SharedSmartLibrary.Contexts;

#nullable disable

namespace SharedSmartLibrary.Migrations
{
    [DbContext(typeof(SmartHomeDbContext))]
    partial class SmartHomeDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.12");

            modelBuilder.Entity("SharedSmartLibrary.Entities.SmartAppSettings", b =>
                {
                    b.Property<string>("ConnectionString")
                        .HasColumnType("TEXT");

                    b.HasKey("ConnectionString");

                    b.ToTable("Settings");
                });
#pragma warning restore 612, 618
        }
    }
}
