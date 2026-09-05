// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// IUserDataAccess.cs
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

using Sample.Domain;

namespace Sample.Persistence;

/*
 * 機能追加時にパーシステント層でやること
 * 3. リポジトリのインターフェースを追加
 * アプリケーション層で作成した IUserRepository と同じものを定義する
 *     便宜上名前だけは変えておく
 *
 * これも DIP のために DI で使用するインターフェース
 * 後でパーシステント層の ServiceCollectionExtensions にて使用する
 */
public interface IUserDataAccess
{
    Task<User?> FindByIdAsync(string userId, CancellationToken ct = default);
}
