// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// User.cs
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

namespace Sample.Domain;

/*
 * 機能追加時にドメイン層でやること
 * 1. ドメインモデルの作成
 *     ドメイン = 業務要件としてのロジック
 *     他要素 ( アプリケーション層・パーシステント層 ) とドメイン層に書くべきロジックの切り分けは厳密に行ったほうがいい
 * この例ではログイン業務に関する要件の追加
 */
public sealed class User
{
    public string UserId { get; }
    public string UserName { get; }
    private string Password { get; }   // 平文（サンプル前提）。外部へは公開しない

    // 業務要件の実装
    public User(string userId, string password, string userName)
    {
        // 要件 1. ユーザー ID がブランクである状態は許容されない
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("userId は必須です", nameof(userId));
        // 要件 2. パスワードがブランクである状態は許容されない
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("userName は必須です", nameof(userName));

        UserId = userId;
        UserName = userName;
        Password = password ?? string.Empty;
    }

    /*
     * 要件 3. 以下は一致しなければならない
     *     3-1. ユーザーが入力したパスワード
     *     3-2. USERS 表に登録されているパスワード
     */
    public bool VerifyPassword(string presented)
        => string.Equals(Password, presented, StringComparison.Ordinal);
}
