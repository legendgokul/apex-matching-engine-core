using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace apexLedger.DataAccess.Models;

public partial class LedgerContext : DbContext
{
    public LedgerContext()
    {
    }

    public LedgerContext(DbContextOptions<LedgerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Buyledger> Buyledgers { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Sellledger> Sellledgers { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Transactionrecord> Transactionrecords { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userholding> Userholdings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ledgerdb;Username=postgres;Password=1234;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Buyledger>(entity =>
        {
            entity.HasKey(e => e.LedgerId).HasName("buyledger_pkey");

            entity.ToTable("buyledger");

            entity.Property(e => e.LedgerId)
                .ValueGeneratedNever()
                .HasColumnName("ledger_id");
            entity.Property(e => e.Createdon).HasColumnName("createdon");
            entity.Property(e => e.StockId).HasColumnName("stock_id");
            entity.Property(e => e.Stockcount).HasColumnName("stockcount");
            entity.Property(e => e.Stockprice).HasColumnName("stockprice");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Stock).WithMany(p => p.Buyledgers)
                .HasForeignKey(d => d.StockId)
                .HasConstraintName("buyledger_stock_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Buyledgers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("buyledger_user_id_fkey");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("company_pkey");

            entity.ToTable("company");

            entity.Property(e => e.CompanyId)
                .ValueGeneratedNever()
                .HasColumnName("company_id");
            entity.Property(e => e.CompanyName).HasColumnName("company_name");
            entity.Property(e => e.CompanyUrl).HasColumnName("company_url");
            entity.Property(e => e.Createdon).HasColumnName("createdon");
        });

        modelBuilder.Entity<Sellledger>(entity =>
        {
            entity.HasKey(e => e.LedgerId).HasName("sellledger_pkey");

            entity.ToTable("sellledger");

            entity.Property(e => e.LedgerId)
                .ValueGeneratedNever()
                .HasColumnName("ledger_id");
            entity.Property(e => e.Createdon).HasColumnName("createdon");
            entity.Property(e => e.StockId).HasColumnName("stock_id");
            entity.Property(e => e.Stockcount).HasColumnName("stockcount");
            entity.Property(e => e.Stockprice).HasColumnName("stockprice");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Stock).WithMany(p => p.Sellledgers)
                .HasForeignKey(d => d.StockId)
                .HasConstraintName("sellledger_stock_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Sellledgers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("sellledger_user_id_fkey");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.StockId).HasName("stock_pkey");

            entity.ToTable("stock");

            entity.Property(e => e.StockId)
                .ValueGeneratedNever()
                .HasColumnName("stock_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Createdon).HasColumnName("createdon");
            entity.Property(e => e.StockName).HasColumnName("stock_name");
            entity.Property(e => e.StockSymbol)
                .HasMaxLength(5)
                .HasColumnName("stock_symbol");

            entity.HasOne(d => d.Company).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("stock_company_id_fkey");
        });

        modelBuilder.Entity<Transactionrecord>(entity =>
        {
            entity.HasKey(e => e.TradeId).HasName("transactionrecord_pkey");

            entity.ToTable("transactionrecord");

            entity.Property(e => e.TradeId)
                .ValueGeneratedNever()
                .HasColumnName("trade_id");
            entity.Property(e => e.BuyerId).HasColumnName("buyer_id");
            entity.Property(e => e.Saleprice).HasColumnName("saleprice");
            entity.Property(e => e.SellerId).HasColumnName("seller_id");
            entity.Property(e => e.StockId).HasColumnName("stock_id");
            entity.Property(e => e.Tnxtime).HasColumnName("tnxtime");

            entity.HasOne(d => d.Buyer).WithMany(p => p.TransactionrecordBuyers)
                .HasForeignKey(d => d.BuyerId)
                .HasConstraintName("transactionrecord_buyer_id_fkey");

            entity.HasOne(d => d.Seller).WithMany(p => p.TransactionrecordSellers)
                .HasForeignKey(d => d.SellerId)
                .HasConstraintName("transactionrecord_seller_id_fkey");

            entity.HasOne(d => d.Stock).WithMany(p => p.Transactionrecords)
                .HasForeignKey(d => d.StockId)
                .HasConstraintName("transactionrecord_stock_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.Createdon).HasColumnName("createdon");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Username).HasColumnName("username");
        });

        modelBuilder.Entity<Userholding>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.StockId }).HasName("userholding_pk");

            entity.ToTable("userholding");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.StockId).HasColumnName("stock_id");
            entity.Property(e => e.Boughtprice).HasColumnName("boughtprice");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Stock).WithMany(p => p.Userholdings)
                .HasForeignKey(d => d.StockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userholding_stock_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Userholdings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userholding_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
