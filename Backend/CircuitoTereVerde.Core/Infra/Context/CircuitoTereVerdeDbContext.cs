using CircuitoTereVerde.Core.Domain;
using CircuitoTereVerde.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CircuitoTereVerde.Core.Infra.Context;

public sealed class CircuitoTereVerdeDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public CircuitoTereVerdeDbContext(DbContextOptions<CircuitoTereVerdeDbContext> options)
        : base(options)
    {
    }

    public DbSet<Parque> Parques
    {
        get { return Set<Parque>(); }
    }

    public DbSet<TipoPostagem> TiposPostagem
    {
        get { return Set<TipoPostagem>(); }
    }

    public DbSet<Postagem> Postagens
    {
        get { return Set<Postagem>(); }
    }

    public DbSet<FotoPostagem> FotosPostagem
    {
        get { return Set<FotoPostagem>(); }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Parque>().HasData(
            new Parque { Id = 1, Codigo = CodigosCatalogo.Parques.Pnso, Nome = "Parque Nacional da Serra dos Órgãos" },
            new Parque { Id = 2, Codigo = CodigosCatalogo.Parques.Pnmmt, Nome = "Parque Natural Municipal Montanhas de Teresópolis" },
            new Parque { Id = 3, Codigo = CodigosCatalogo.Parques.Petp, Nome = "Parque Estadual dos Três Picos" });

        builder.Entity<TipoPostagem>().HasData(
            new TipoPostagem { Id = 1, Codigo = CodigosCatalogo.Tipos.Trilhas, Nome = CodigosCatalogo.Tipos.Trilhas },
            new TipoPostagem { Id = 2, Codigo = CodigosCatalogo.Tipos.Cachoeiras, Nome = CodigosCatalogo.Tipos.Cachoeiras },
            new TipoPostagem { Id = 3, Codigo = CodigosCatalogo.Tipos.Biodiversidade, Nome = CodigosCatalogo.Tipos.Biodiversidade },
            new TipoPostagem { Id = 4, Codigo = CodigosCatalogo.Tipos.Eventos, Nome = CodigosCatalogo.Tipos.Eventos },
            new TipoPostagem { Id = 5, Codigo = CodigosCatalogo.Tipos.Novidades, Nome = CodigosCatalogo.Tipos.Novidades },
            new TipoPostagem { Id = 6, Codigo = CodigosCatalogo.Tipos.Atracoes, Nome = CodigosCatalogo.Tipos.Atracoes },
            new TipoPostagem { Id = 7, Codigo = CodigosCatalogo.Tipos.Sobre, Nome = CodigosCatalogo.Tipos.Sobre });

        builder.Entity<Postagem>().Navigation(item => item.Parque).AutoInclude();
        builder.Entity<Postagem>().Navigation(item => item.TipoPostagem).AutoInclude();
        builder.Entity<Postagem>().Navigation(item => item.FotoPostagem).AutoInclude();
        builder.Entity<Postagem>()
            .HasOne(item => item.Parque)
            .WithMany()
            .HasForeignKey(item => item.ParqueId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Postagem>()
            .HasOne(item => item.TipoPostagem)
            .WithMany()
            .HasForeignKey(item => item.TipoPostagemId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Postagem>()
            .HasOne(item => item.FotoPostagem)
            .WithOne()
            .HasForeignKey<Postagem>(item => item.FotoPostagemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
