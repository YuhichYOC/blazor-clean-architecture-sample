// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// IUserRepository.cs
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

namespace Sample.Application.Abstractions;

/*
 * 機能追加時にアプリケーション層でやること
 * 2. リポジトリのインターフェースを追加
 *     リポジトリ = 大雑把にパーシステント層と把握していい
 *
 * よくある疑問 ... なぜここにリポジトリのインターフェースが存在するか？
 * 大雑把な理由 : アプリケーション層がパーシステント層を参照してはいけないから
 *     クリーンアーキテクチャの DIP の都合による
 * ここにリポジトリのインターフェースだけ登録しておき、アプリケーション層 & パーシステント層はこのインターフェースを基準に機能を実装する
 * アプリケーション層のユースケースとパーシステント層のリポジトリの紐づけは DI を利用して行う
 *     ユースケースを DI に登録するステップは ServiceCollectionExtensions に存在する
 * 実際に「起きること・やらなくてはならない業務」は、アプリケーション層からパーシステント層のデータを参照することであり、業務上はアプリケーション層からパーシステント層へ依存している
 * DIP で依存関係を逆転させているのは上っ面の話となる。コードと DI のトリックでそのように見せかけているだけ。クリーンな状態のために、「コードの上でそう見えること」が重要だと自分を腹落ちさせるしかない
 */
public interface IUserRepository
{
    // ログイン機能で使用するパーシステント層の機能は USERS 表の検索だけなので、ここに定義するメソッドはこの一つだけになる
    Task<User?> FindByIdAsync(string userId, CancellationToken ct = default);
}
