// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// AuthenticateUseCase.cs
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

using Sample.Application.Abstractions;
using Sample.Domain;

namespace Sample.Application.UseCases;

/*
 * 機能追加時にアプリケーション層でやること
 * 1. 追加機能用ユースケースの追加
 *     ユースケース ... 大雑把にドメイン層 ( 業務要件に基づくロジック ), パーシステント層 ( データベースとのデータのやり取り ) 以外のアプリケーションの動作を規定するものと把握していい
 *     ユースケースという言葉が意味するもの ... ユーザーの意図する？期待する？アプリの一連の相互作用
 *         大雑把に、「意味的にひと塊の処理の流れ」と把握していい
 *         以下はログインボタンクリックからログイン成功もしくはログイン失敗までのひと塊を規定している
 * この例はログイン機能の追加
 */
public sealed class AuthenticateUseCase
{
    private readonly IUserRepository _users;
    public AuthenticateUseCase(IUserRepository users) => _users = users;

    // ログイン処理
    // ユーザーが入力したユーザー ID を引数に USERS を検索
    // ログイン成功 : レコードあり & パスワード一致した場合 User インスタンスを返す
    // ログイン失敗 : レコードなし, パスワードが一致しないどちらかのケースで null を返す
    public async Task<User?> ExecuteAsync(string userId, string password, CancellationToken ct = default)
    {
        // ユーザーが入力したユーザー ID を引数にデータベースから USERS レコードを検索
        var user = await _users.FindByIdAsync(userId, ct);
        // このステップで user が null ならログインエラー
        //     ユーザー ID ヒットなし
        if (user is null) return null;
        // ユーザーが入力したパスワードは USERS に登録されたものと一致するか検証
        return user.VerifyPassword(password) ? user : null;
    }
}
