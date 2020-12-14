using System;
using Petrol_Station.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Petrol_Station
{
    public partial class Petrol_StationContext : DbContext
    {
        public Petrol_StationContext(DbContextOptions<Petrol_StationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Containers> Containers { get; set; }
        public virtual DbSet<Costs> Costs { get; set; }
        public virtual DbSet<Gsm> Gsm { get; set; }
        public virtual DbSet<IncomeAndExpensesOfGsm> IncomeAndExpensesOfGsm { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Containers>(entity =>
            {
                entity.HasKey(e => e.ContainerId)
                    .HasName("PK__Containe__F10C3DB8C00F69F2");

                entity.Property(e => e.ContainerId).HasColumnName("containerId");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.Property(e => e.TankCapacity).HasColumnName("tankCapacity");

                entity.Property(e => e.TypeOfGsmid).HasColumnName("typeOfGSMId");

                entity.HasOne(d => d.TypeOfGsm)
                    .WithMany(p => p.Containers)
                    .HasForeignKey(d => d.TypeOfGsmid)
                    .HasConstraintName("FK__Container__typeO__403A8C7D");
            });

            modelBuilder.Entity<Costs>(entity =>
            {
                entity.HasKey(e => e.CostId)
                    .HasName("PK__Costs__6214B2839DBA312B");

                entity.Property(e => e.CostId).HasColumnName("costId");

                entity.Property(e => e.DateOfCostGsm)
                    .HasColumnName("dateOfCostGSM")
                    .HasColumnType("datetime");

                entity.Property(e => e.PricePerLiter).HasColumnName("pricePerLiter");

                entity.Property(e => e.TypeOfGsmid).HasColumnName("typeOfGSMId");

                entity.HasOne(d => d.TypeOfGsm)
                    .WithMany(p => p.Costs)
                    .HasForeignKey(d => d.TypeOfGsmid)
                    .HasConstraintName("FK__Costs__typeOfGSM__3F466844");
            });

            modelBuilder.Entity<Gsm>(entity =>
            {
                entity.ToTable("GSM");

                entity.Property(e => e.GSmid).HasColumnName("gSMId");

                entity.Property(e => e.TTkofType)
                    .HasColumnName("tTKOfType")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TypeOfGsm)
                    .HasColumnName("typeOfGSM")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<IncomeAndExpensesOfGsm>(entity =>
            {
                entity.HasKey(e => e.IncomeAndExpenseOfGsmid)
                    .HasName("PK__IncomeAn__8EF985BCABA58302");

                entity.ToTable("IncomeAndExpensesOfGSM");

                entity.Property(e => e.IncomeAndExpenseOfGsmid).HasColumnName("incomeAndExpenseOfGSMId");

                entity.Property(e => e.ContainerId).HasColumnName("containerId");

                entity.Property(e => e.DateAndTimeOfTheOperationIncomeOrExpense)
                    .HasColumnName("dateAndTimeOfTheOperationIncomeOrExpense")
                    .HasColumnType("datetime");

                entity.Property(e => e.IncomeOrExpensePerliter).HasColumnName("incomeOrExpensePerliter");

                entity.Property(e => e.NumberOfCapacity).HasColumnName("numberOfCapacity");

                entity.Property(e => e.ResponsibleForTheOperation)
                    .HasColumnName("responsibleForTheOperation")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.StaffId).HasColumnName("staffId");

                entity.HasOne(d => d.Container)
                    .WithMany(p => p.IncomeAndExpensesOfGsm)
                    .HasForeignKey(d => d.ContainerId)
                    .HasConstraintName("FK__IncomeAnd__conta__412EB0B6");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.IncomeAndExpensesOfGsm)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK__IncomeAnd__staff__4222D4EF");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.Property(e => e.StaffId).HasColumnName("staffId");

                entity.Property(e => e.FullName)
                    .HasColumnName("fullName")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.StaffAge).HasColumnName("staffAge");

                entity.Property(e => e.StaffFunction)
                    .HasColumnName("staffFunction")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WorkingHoursForAweek)
                    .HasColumnName("workingHoursForAWeek");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
