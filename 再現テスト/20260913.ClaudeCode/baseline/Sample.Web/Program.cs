using Microsoft.EntityFrameworkCore;
using Sample.Application.RepositoryAdapters;
using Sample.Application.UseCases;
using Sample.Persistence;
using Sample.Persistence.RepositoryAdapters;
using Sample.Persistence.Repositories;
using Sample.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ORACLE Database への接続（Docker ネットワーク経由）
var oracleConnectionString = builder.Configuration.GetConnectionString("OracleDb");
if (string.IsNullOrWhiteSpace(oracleConnectionString))
{
    throw new InvalidOperationException("接続文字列 'ConnectionStrings:OracleDb' が設定されていません。");
}

builder.Services.AddDbContext<SampleDbContext>(options => options.UseOracle(oracleConnectionString));

// パーシステント層：リポジトリ
builder.Services.AddScoped<ItemRepository>();
builder.Services.AddScoped<MaterialRepository>();
builder.Services.AddScoped<BomRepository>();

// パーシステント層：リポジトリアダプター（アプリケーション層インターフェースの実装）
builder.Services.AddScoped<IBomListRepositoryAdapter, BomListRepositoryAdapter>();
builder.Services.AddScoped<IPartsListRegistrationRepositoryAdapter, PartsListRegistrationRepositoryAdapter>();
builder.Services.AddScoped<IPartsListDeletionRepositoryAdapter, PartsListDeletionRepositoryAdapter>();

// アプリケーション層：ユースケース
builder.Services.AddScoped<GetBomListUseCase>();
builder.Services.AddScoped<RegisterPartsListUseCase>();
builder.Services.AddScoped<PrepareItemDeletionUseCase>();
builder.Services.AddScoped<DeleteItemUseCase>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
