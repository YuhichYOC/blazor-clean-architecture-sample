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

        services.AddScoped<IBomRepository, BomRepositoryAdapter>(); // ← 追加

        return services;
    }

    /*
     * 機能追加時にパーシステント層でやること
     * 6. ServiceCollectionExtensions への DI 定義の追加
     * アプリケーション層のユースケースが DI 経由でリポジトリの実体 ( UserRepository ) へたどり着けるように DIP で用意した各種インターフェースとクラスを紐付ける
     * このメソッドはアプリケーション全体のエントリポイント ( このサンプルの場合は Sample.Web の Program.cs ) で呼び出される
     *
     * 以下の例はログイン処理で使用するリポジトリ関連の DI 定義追加
     */
    public static IServiceCollection AddUserPersistence(
        this IServiceCollection services, string connectionString)
    {
        // 6-1. DbContext の追加
        // 型パラメータに注意
        //     この例ではログイン処理に使用する DI 定義なので UserDbContext が正解
        services.AddDbContextFactory<UserDbContext>(options =>
            options.UseOracle(connectionString));

        // 6-2. パーシステント層で定義した IUserDataAccess と UserRepository の紐づけ定義
        services.AddScoped<IUserDataAccess, UserRepository>();

        // 6-3. アプリケーション層で定義した IUserRepository とパーシステント層で定義した UserRepositoryAdapter の紐づけ定義
        services.AddScoped<IUserRepository, UserRepositoryAdapter>();

        return services;
    }
}
