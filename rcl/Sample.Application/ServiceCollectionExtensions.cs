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

using Microsoft.Extensions.DependencyInjection;
using Sample.Application.Deletion;
using Sample.Application.Query;
using Sample.Application.Registration;
using Sample.Application.UseCases;

namespace Sample.Application;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// アプリケーション層のユースケースを DI へ登録する。
    ///
    /// ポート <c>IBomRepository</c> の実装(アダプタ)はここでは登録しない。
    /// それは永続化側の AddBomPersistence が担う。合成ルート(サーバーホストの Program.cs)で
    ///   services.AddBomApplication();
    ///   services.AddBomPersistence(connectionString);
    /// の両方を呼ぶことで、はじめてポートと実装が結線される。
    ///
    /// この分担により、Application は具体的な永続化実装(EF/Oracle)を知らないまま保てる。
    /// </summary>
    public static IServiceCollection AddBomApplication(this IServiceCollection services)
    {
        services.AddScoped<GetBomListUseCase>();
        services.AddScoped<RegisterItemUseCase>();
        services.AddScoped<DeleteItemUseCase>();
        return services;
    }

    /*
     * 機能追加時にアプリケーション層でやること
     * 3. ServiceCollectionExtensions への DI 定義の追加
     * IUserRepository での説明にある通り、アプリケーション層のユースケースを DI へ追加する
     * このメソッドはアプリケーション全体のエントリポイント ( このサンプルの場合は Sample.Web の Program.cs ) で呼び出される
     */
    public static IServiceCollection AddUserApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthenticateUseCase>();
        return services;
    }
}
