using Sample.Application;      // AddBomApplication
using Sample.Persistence;      // AddBomPersistence
using Sample.Web.Components;   // App(標準テンプレートのルートコンポーネント)

var builder = WebApplication.CreateBuilder(args);

// Blazor Web App(サーバー対話式)。この画面は InteractiveServer で動かす。
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

builder.Services.AddBomApplication();
builder.Services.AddBomPersistence(connectionString);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();   // wwwroot/css/bom.css 等を配信(.NET 8)
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
