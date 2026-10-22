using CircuitoTereVerde.Core.Infra.Context;
using CircuitoTereVerde.Core.Models;
using CircuitoTereVerdeWebApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace CircuitoTereVerde.Tests;

[Collection(ApiTesteCollection.Nome)]
public sealed class InicializacaoBancoTests
{
    private readonly ApiTesteFixture _fixture;

    public InicializacaoBancoTests(ApiTesteFixture fixture)
    {
        _fixture = fixture;
    }

    #region Migrações e dados iniciais

    [Fact]
    public async Task InicializacaoPreparaBancoEAdministradorSemDuplicarDados()
    {
        await using var escopo = _fixture.Factory.Services.CreateAsyncScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<CircuitoTereVerdeDbContext>();
        var usuarios = escopo.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var perfis = escopo.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var inicializador = escopo.ServiceProvider.GetRequiredService<DatabaseInitializer>();

        // A fixture já iniciou a API; ela precisa ter aplicado a Inicial no SQLite configurado.
        Assert.Equal("Microsoft.EntityFrameworkCore.Sqlite", contexto.Database.ProviderName);
        var migracoes = await contexto.Database.GetAppliedMigrationsAsync();
        // A Inicial existe e qualquer migração criada depois dela também é aplicada na partida.
        Assert.Contains(migracoes, migracao => migracao.EndsWith("_Inicial"));
        Assert.Empty(await contexto.Database.GetPendingMigrationsAsync());
        Assert.False(contexto.Database.HasPendingModelChanges());

        // Todas as entidades com chave inteira recebem o ID automaticamente ao salvar.
        foreach (var tipo in new[]
        {
            typeof(Parque), typeof(TipoPostagem), typeof(Postagem), typeof(FotoPostagem),
            typeof(IdentityUserClaim<string>), typeof(IdentityRoleClaim<string>)
        })
        {
            var chave = contexto.Model.FindEntityType(tipo)!.FindProperty("Id")!;
            Assert.Equal(ValueGenerated.OnAdd, chave.ValueGenerated);
        }

        // Os catálogos fixos e os dois perfis precisam existir depois da inicialização.
        Assert.Equal(3, await contexto.Parques.CountAsync());
        Assert.Equal(7, await contexto.TiposPostagem.CountAsync());
        Assert.True(await perfis.RoleExistsAsync("USUARIO"));
        Assert.True(await perfis.RoleExistsAsync("ADMINISTRADOR"));

        // O administrador vem do appsettings, recebe o perfil correto e tem a senha protegida por hash.
        var administrador = await usuarios.FindByEmailAsync(_fixture.Factory.EmailAdministrador);
        Assert.NotNull(administrador);
        Assert.True(Guid.TryParse(administrador.Id, out _));
        foreach (var idPerfil in await contexto.Roles.Select(perfil => perfil.Id).ToListAsync())
        {
            Assert.True(Guid.TryParse(idPerfil, out _));
        }
        Assert.True(await usuarios.IsInRoleAsync(administrador, "ADMINISTRADOR"));
        Assert.True(await usuarios.CheckPasswordAsync(administrador, _fixture.Factory.SenhaAdministrador));
        Assert.NotEqual(_fixture.Factory.SenhaAdministrador, administrador.PasswordHash);

        // Cada postagem aponta para um usuário existente e o banco impede excluir esse usuário por engano.
        var entidadePostagem = contexto.Model.FindEntityType(typeof(Postagem))!;
        var propriedadeCriadoPor = entidadePostagem.FindProperty(nameof(Postagem.CriadoPorId))!;
        var relacionamentoCriadoPor = entidadePostagem.FindForeignKeys(propriedadeCriadoPor).Single();
        Assert.Equal(typeof(ApplicationUser), relacionamentoCriadoPor.PrincipalEntityType.ClrType);
        Assert.Equal(DeleteBehavior.Restrict, relacionamentoCriadoPor.DeleteBehavior);

        var idAdministrador = administrador.Id;
        var senhaProtegida = administrador.PasswordHash;
        var quantidadeUsuarios = await contexto.Users.CountAsync();
        var quantidadePerfis = await contexto.Roles.CountAsync();

        // Repete a preparação no mesmo banco para validar os próximos inícios da API.
        await inicializador.InitializeAsync();
        contexto.ChangeTracker.Clear();

        // A segunda inicialização conserva IDs, senha e quantidades, sem recriar os dados existentes.
        var administradorPersistido = await usuarios.FindByEmailAsync(_fixture.Factory.EmailAdministrador);
        Assert.NotNull(administradorPersistido);
        Assert.Equal(idAdministrador, administradorPersistido.Id);
        Assert.Equal(senhaProtegida, administradorPersistido.PasswordHash);
        Assert.Equal(quantidadeUsuarios, await contexto.Users.CountAsync());
        Assert.Equal(quantidadePerfis, await contexto.Roles.CountAsync());
        Assert.Equal(3, await contexto.Parques.CountAsync());
        Assert.Equal(7, await contexto.TiposPostagem.CountAsync());
        Assert.Empty(await contexto.Database.GetPendingMigrationsAsync());
    }

    #endregion
}
