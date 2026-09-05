// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// RegisterItemResult.cs
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

namespace Sample.Application.Registration;

/// <summary>
/// 登録ユースケースの結果。
///
/// 業務エラー(不変条件違反)は例外のまま UI へ飛ばさず、結果型として返す。
/// = DomainException を「想定内の業務エラー(400 相当)」としてユースケースが受け止める。
///   (DbException 等の“想定外”エラーは握りつぶさず伝播させ、境界で 500 相当に扱う。)
///
/// パターンマッチで分岐できるよう、閉じた継承(sealed record 派生)にしている。
/// </summary>
public abstract record RegisterItemResult
{
    private RegisterItemResult() { }

    /// <summary>登録成功。</summary>
    public sealed record Success : RegisterItemResult;

    /// <summary>
    /// 不変条件違反。Message はそのまま画面に出せる業務エラー文言
    /// (例: 「品番は必須です。」「所要量は0より大きい必要があります…」)。
    /// </summary>
    public sealed record ValidationError(string Message) : RegisterItemResult;

    /// <summary>一意制約違反。</summary>
    public sealed record Conflict(string Message) : RegisterItemResult;
}
