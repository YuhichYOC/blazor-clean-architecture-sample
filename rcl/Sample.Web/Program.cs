using Microsoft.AspNetCore.Authentication.Cookies;
using Sample.Application;      // AddBomApplication
using Sample.Persistence;      // AddBomPersistence
using Sample.Web.Components;   // App(標準テンプレートのルートコンポーネント)

/*
 * Program.cs に宣言できるもの
 *
 * ビルド前 : builder.xxx
 * DI コンテナへインスタンス生成方法を伝える宣言 : builder.Services.AddBomApplication など
 * 設定ファイル読み込み : builder.Configuration.GetConnectionString("Bom")
 * ロギングの設定
 * ホスト／サーバー設定 ... 待ち受けポートや URL など
 *
 * ビルド後 : app.xxx
 * ミドルウェアパイプラインの設定
 * エンドポイント／ルーティングの登録 : app.MapRazorComponents
 */

var builder = WebApplication.CreateBuilder(args);

// Blazor Web App(サーバー対話式)。この画面は InteractiveServer で動かす。
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

/*
 * app.UseAuthentication(); と合わせて、Cookie 認証ミドルウェアを追加
 * DI に CookieAuthenticationHandler を登録
 *     CookieAuthenticationHandler とは AddCookie(o => ... 部分
 * 実装メモ ... ユーザーの権限検証を追加する場合
 *     Login.razor で発行する Claim に new Claim(ClaimTypes.Role, "Admin"); を追加
 *     権限を必要とするページの Authorize に権限指定を記入
 *         @attribute [Authorize(Roles = "Admin")]
 *     以下の順で権限の検証が行われる
 *         [Authorize(Roles = "Admin")] → AuthorizeRouteView → IAuthorizationService → RolesAuthorizationRequirement → ClaimsPrincipal.IsInRole("Admin")
 */
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/login";
        o.ExpireTimeSpan = TimeSpan.FromHours(1);
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// ─────────────────────────────────────────────────────────────
// 合成ルート(Composition Root)。
//   ここではじめて「ポート」と「実装」が結線される。
//     AddBomApplication : ユースケースを登録
//                         (GetBomListUseCase / RegisterItemUseCase / DeleteItemUseCase)
//     AddBomPersistence : ポート IBomRepository の実装 = BomRepositoryAdapter を登録
//                         + EF Core / Oracle(DbContextFactory)を登録
//   この2つを両方呼ぶことで、Application は具体実装(EF/Oracle)を知らないまま結線される。
//
// この Program.cs(=ホスト)は Application と Persistence の両方を参照してよい。
// 合成ルートは配線の場所であり、すべてを知ることが許される唯一の層。
// 一方、各コンポーネントは Sample.Application.* の型しか使わない(規約)。
// ─────────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("Bom")
    ?? throw new InvalidOperationException("接続文字列 'Bom' が未設定です(appsettings.json)。");

/*
 * 機能追加時に合成ルートでやること
 * 1. 機能で使用するクラス生成方法の登録
 *     スコープ
 *         アプリ全体で一つ ( AddSingleton<T> )
 *         スコープごとに一つ ( AddScoped<T> )
 *         要求ごとに新規作成 ( AddTransient<T> )
 *     作成方法
 *         コンテナに作らせる
 *         ファクトリ移譲
 *         作成済みインスタンスを渡す ( AddSingleton 限定 )
 */
builder.Services.AddBomApplication();
builder.Services.AddUserApplication();
builder.Services.AddBomPersistence(connectionString);
builder.Services.AddUserPersistence(connectionString);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();   // wwwroot/css/bom.css 等を配信(.NET 8)
/*
 * builder.Services.AddAuthentication と合わせて Cookie による認証ミドルウェアの有効化
 * パイプラインを通るすべての HTTP リクエストで
 *     ※ SignalR の通信は除く
 * 例
 *     Login で認証 → クライアント側ブラウザへ認証済み Principal を書き込んだ Cookie を保存
 *         → 次のリクエストでクライアントから Cookie が送り返される
 *         クライアントから送られてきた Cookie が改ざんされていると、未認証の状態に戻る
 */
app.UseAuthentication();
/*
 * Cookie から復号した Principal に対して権限の検証を有効化する
 */
app.UseAuthorization();
app.UseAntiforgery();

/*
 * RCL 版 : 機能追加時に合成ルートでやること
 * 2. プレゼンテーション層 ( Sample.Presentation.Components.Pages 内 ) で定義した機能を AdditionalAssemblies へ追加
 */
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(Sample.Presentation.Components.Pages.Bom.BomPage).Assembly);

app.Run();
