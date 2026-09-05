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
        this IServiceCollection services, string apiBaseUrl)
    {
        // ── 内側の継ぎ目 IBomDataAccess を「型付き HttpClient」で実装 ──
        //   IHttpClientFactory がハンドラのライフサイクル(ソケット枯渇/DNS更新)を管理する。
        services.AddHttpClient<IBomDataAccess, BomApiHttpClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);   // 例: "https://localhost:5991/"
            // ★要確認: Timeout / 既定ヘッダ(APIキー等) / リトライ(Polly: AddStandardResilienceHandler)はここで付ける。
        });

        // ── ポート → アダプタ(baseline と同一。中身が EF→HTTP に変わったことを上位は知らない) ──
        services.AddScoped<IBomRepository, BomRepositoryAdapter>();

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
        this IServiceCollection services, string apiBaseUrl)
    {
        services.AddHttpClient<IUserDataAccess, UserApiHttpClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        services.AddScoped<IUserRepository, UserRepositoryAdapter>();

        return services;
    }
}
