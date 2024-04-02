using GestioneNotifiche.Core.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace GestioneNotifiche.Core.Database;

public partial class BdmonitorContext : DbContext
{
    private static string _connString = "";
    public BdmonitorContext()
    {
    }

    public BdmonitorContext(string connectionString)
    {
        _connString = connectionString;
    }

    public BdmonitorContext(DbContextOptions<BdmonitorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BdmEsecuzioneServiziStudi> BdmEsecuzioneServiziStudis { get; set; }
    public virtual DbSet<StudiParametri> StudiParametris { get; set; }
    public virtual DbSet<OreAttivitaUtentiStudio> OreAttivitaUtentiStudios { get; set; }
    public virtual DbSet<BdmServizi> BdmServizis { get; set; }
    public virtual DbSet<BdmEsecuzioneServiziStudiDettagli> BdmEsecuzioneServiziStudiDettaglis { get; set; }
    public virtual DbSet<ImpegniReminder> ImpegniReminders { get; set; }
    public virtual DbSet<BdmEsecuzioneReminderImpegni> BdmEsecuzioneReminderImpegnis { get; set; }
    public virtual DbSet<BdmEsecuzioneReminderImpegniDettagli> BdmEsecuzioneReminderImpegniDettaglis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(_connString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AS");

        modelBuilder.Entity<BdmEsecuzioneReminderImpegniDettagli>(entity =>
        {
            entity.HasKey(e => e.IdEsecuzioneDettaglio);

            entity.ToTable("BDM_EsecuzioneReminderImpegniDettagli");
        });

        modelBuilder.Entity<BdmEsecuzioneReminderImpegni>(entity =>
        {
            entity.HasKey(e => e.IdEsecuzione);

            entity.ToTable("BDM_EsecuzioneReminderImpegni");

            entity.Property(e => e.DataExec).HasColumnType("datetime");
        });

        modelBuilder.Entity<ImpegniReminder>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<BdmEsecuzioneServiziStudiDettagli>(entity =>
        {
            entity.HasKey(e => e.IdEsecuzioneDettaglio);

            entity.ToTable("BDM_EsecuzioneServiziStudiDettagli");

            entity.Property(e => e.Utente).HasMaxLength(256);
        });

        modelBuilder.Entity<BdmServizi>(entity =>
        {
            entity.HasKey(e => e.IdServizio);

            entity.ToTable("BDM_Servizi");

            entity.Property(e => e.Descrizione).IsUnicode(false);
        });

        modelBuilder.Entity<OreAttivitaUtentiStudio>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<StudiParametri>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<BdmEsecuzioneServiziStudi>(entity =>
        {
            entity.HasKey(e => e.IdEsecuzione);

            entity.ToTable("BDM_EsecuzioneServiziStudi");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
