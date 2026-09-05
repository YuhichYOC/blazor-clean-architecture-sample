// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// DomainException.cs
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

/// <summary>
/// ドメインの不変条件に違反したときに投げる例外。
/// インフラ由来の例外(DbException 等)と区別するための専用型にしておくと、
/// アプリケーション層で「業務エラー(400 相当)」と「システムエラー(500 相当)」を切り分けやすい。
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
