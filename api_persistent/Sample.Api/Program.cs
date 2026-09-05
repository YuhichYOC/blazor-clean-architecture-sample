using Sample.Api;
using Sample.Api.Data.Bom;
using Sample.Api.Data.Login;
using Microsoft.EntityFrameworkCore;

// ─────────────────────────────────────────────────────────────
// 新DBアクセスAPI(完全独立の ASP.NET サーバー)の合成ルート。
// baseline で Sample.Web(ホスト)にあった EF Core / Oracle の登録が、こちらへ移設される。
// Sample.sln 側にはもう EF/Oracle は無い(Sample.Persistence は HttpClient だけを持つ)。
// ─────────────────────────────────────────────────────────────
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("Bom")
    ?? throw new InvalidOperationException("接続文字列 'Bom' が未設定です(appsettings.json)。");

// baseline の BomRepository と同じ IDbContextFactory パターンを踏襲。
//   (このAPIも並行要求を受けるため、短命 DbContext を都度生成するのが安全)
builder.Services.AddDbContextFactory<BomDbContext>(o => o.UseOracle(connectionString));
builder.Services.AddScoped<BomDataService>();
builder.Services.AddDbContextFactory<UserDbContext>(o => o.UseOracle(connectionString));
builder.Services.AddScoped<LoginDataService>();

// ★要確認(セキュリティ): これは「生のDBアクセスを HTTP で公開する」サーバー。
//   外部に晒す設計なら、最低限どれかは必須:
//     - ネットワーク隔離(内部ネットワーク/コンテナネットワークのみ、外部LBに出さない)
//     - APIキー or mTLS or JWT による認証
//   baseline では Cookie 認証が Blazor 層にあったが、この層は別途守る必要がある。
// builder.Services.AddAuthentication(...);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
