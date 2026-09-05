// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// UserRepository.cs
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
using Sample.Domain;

namespace Sample.Persistence;

/*
 * 機能追加時にパーシステント層でやること
 * 4. リポジトリの追加
 * ここに定義するものはアプリケーション層で定義した IUserRepository の実体
 * User, UserDbContext に定義したレコードの受け皿およびレコードセットを使って以下の処理の実体を記述する
 *     ・アプリケーション層が必要とするデータを返す
 *     ・データベースの更新をする
 *
 * クラスの宣言で指定する型パラメータが正しいか注意
 *     この例はログイン処理のリポジトリなので UserDbContext が正解
 */
internal sealed class UserRepository(IDbContextFactory<UserDbContext> factory) : IUserDataAccess
{
    public async Task<User?> FindByIdAsync(string userId, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();

        return await db.Users
            .Where(u => u.UserId == userId)
            .Select(u => new User(u.UserId, u.Password, u.UserName))
            .FirstOrDefaultAsync(ct); // userId = USERS.USER_ID でヒットする行が存在しないときに null を返す, FirstAsync ではヒットする行が存在しないときに実行時例外になる
    }
}
