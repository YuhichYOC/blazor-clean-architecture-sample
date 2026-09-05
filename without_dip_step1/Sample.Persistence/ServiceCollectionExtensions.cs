// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// ServiceCollectionExtensions.cs
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}
