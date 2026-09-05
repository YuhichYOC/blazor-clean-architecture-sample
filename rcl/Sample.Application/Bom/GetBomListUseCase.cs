// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// GetBomListUseCase.cs
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
using Sample.Application.ReadModels;

namespace Sample.Application.Query;

/// <summary>
/// 一覧取得ユースケース(画面ロード)。
///
/// 実装はポートへの委譲だけ。読み取りは不変条件を要さないため、
/// ドメインを通さずポート→読み取りモデルで直行する(CQRS の読み側)。
///
/// 正直な注記:
///   現状これは委譲のみの“薄い”ラッパで、ほぼ ceremony に近い。
///   UI がポートを直接叩いても機能は変わらない。
///   このクラスが価値を持ち始めるのは、読み取りに「認可・整形・複数ソース統合」などの
///   方針が乗ったとき。今は将来そこに手続きが乗る受け皿として置いておくが、
///   不要と判断するなら削ってポートを直接使ってよい(過剰な抽象は足さない)。
/// </summary>
public sealed class GetBomListUseCase(IBomRepository repository)
{
    public Task<IReadOnlyList<BomRow>> ExecuteAsync(CancellationToken ct = default)
        => repository.GetBomListAsync(ct);
}
