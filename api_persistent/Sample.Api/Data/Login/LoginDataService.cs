// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// LoginDataService.cs
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

using Sample.Api.Contracts.Login;
using Sample.Api.Entities;
using Microsoft.EntityFrameworkCore;

/*
 * このクラスの内容は baseline での UserRepository と同じ
 */
namespace Sample.Api.Data.Login;

public sealed class LoginDataService(IDbContextFactory<UserDbContext> factory)
{
    public async Task<UserResponse?> FindByIdAsync(string userId, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();

        return await db.Users
            .Where(u => u.UserId == userId)
            .Select(u => new UserResponse(u.UserId, u.UserName, u.Password))
            .FirstOrDefaultAsync(ct); // userId = USERS.USER_ID でヒットする行が存在しないときに null を返す, FirstAsync ではヒットする行が存在しないときに実行時例外になる
    }
}
