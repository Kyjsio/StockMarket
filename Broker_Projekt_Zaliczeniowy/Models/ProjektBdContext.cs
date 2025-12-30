using Broker_Projekt_Zaliczeniowy.ModelsDto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Broker_Projekt_Zaliczeniowy.Models;

public partial class ProjektBdContext : DbContext
{
    public ProjektBdContext()
    {
    }

    public ProjektBdContext(DbContextOptions<ProjektBdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Asset> Assets { get; set; }

    public virtual DbSet<MarketDatum> MarketData { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WalletLog> WalletLogs { get; set; }

    public virtual DbSet<AdminUserReportResult> AdminUserReportResults { get; set; }
    public virtual DbSet<SystemStatsResult> SystemStatsResults { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       

        modelBuilder.Entity<WalletLog>(entity =>
        {
            
            entity.Property(e => e.OldBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NewBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ChangeDate)
                .HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Accounts__3214EC07FC35669B");

            entity.HasIndex(e => e.UserId, "UQ__Accounts__1788CC4DF7CABB39").IsUnique();

            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.User).WithOne(p => p.Account)
                .HasForeignKey<Account>(d => d.UserId)
                .HasConstraintName("FK__Accounts__UserId__693CA210");
        });

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Assets__3214EC074958D7CC");

            entity.HasIndex(e => e.Ticker, "UQ__Assets__42AC12F0AFB66916").IsUnique();

            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Ticker)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Stock");
        });

        modelBuilder.Entity<MarketDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MarketDa__3214EC07A8247469");

            entity.HasIndex(e => new { e.AssetId, e.DataDate }, "UQ__MarketDa__266A55EF0343F2FF").IsUnique();

            entity.Property(e => e.Close).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.High).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Low).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Open).HasColumnType("decimal(18, 4)");

            entity.HasOne(d => d.Asset).WithMany(p => p.MarketData)
                .HasForeignKey(d => d.AssetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MarketDat__Asset__619B8048");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Position__3214EC072568E82B");

            entity.HasIndex(e => new { e.AccountId, e.AssetId }, "UQ__Position__00A93792BD49D5A0").IsUnique();

            entity.Property(e => e.AverageCost).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.Account).WithMany(p => p.Positions)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Positions__Accou__6EF57B66");

            entity.HasOne(d => d.Asset).WithMany(p => p.Positions)
                .HasForeignKey(d => d.AssetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Positions__Asset__6FE99F9F");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("trg_LogTransactionAction"));
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC074F8E3788");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Account).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Transacti__Accou__73BA3083");

            entity.HasOne(d => d.Asset).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AssetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__Asset__74AE54BC");
            entity.Property(e => e.Profit)
                .HasColumnType("decimal(18, 2)");

        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0765D55A91");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053449AA4DA3").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AdminUserReportResult>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null);

            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.TotalRealizedProfit).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<SystemStatsResult>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null);

            entity.Property(e => e.TotalSystemCash).HasColumnType("decimal(18, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
