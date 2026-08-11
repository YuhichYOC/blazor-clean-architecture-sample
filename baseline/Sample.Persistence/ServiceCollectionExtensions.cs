using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sample.Application.Abstractions;

namespace Sample.Persistence;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// パーシステント層をDIへ登録する。接続文字列は呼び出し側(サーバーホスト)から渡す。
    /// ライブラリ自体は接続文字列を保持しない。
    ///
    /// このライブラリはサーバー専用依存。Blazor の Auto/WebAssembly 構成では、
    /// このメソッドはサーバーホスト側でのみ呼び出し、Client(WASM)プロジェクトからは参照しないこと。
    /// </summary>
    public static IServiceCollection AddBomPersistence(
        this IServiceCollection services, string connectionString)
    {
        services.AddDbContextFactory<BomDbContext>(options =>
            options.UseOracle(connectionString));

        services.AddScoped<IBomDataAccess, BomRepository>();

        services.AddScoped<Sample.Application.Abstractions.IBomRepository, BomRepositoryAdapter>(); // ← 追加

        return services;
    }

    public static IServiceCollection AddUserPersistence(
        this IServiceCollection services, string connectionString)
    {
        services.AddDbContextFactory<BomDbContext>(options =>
            options.UseOracle(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
