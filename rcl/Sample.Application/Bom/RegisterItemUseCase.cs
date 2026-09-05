// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// RegisterItemUseCase.cs
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

namespace Sample.Application.Registration;

/// <summary>
/// 品番登録ユースケース。
///
/// 役割は「オーケストレーション」だけ:
///   1. 入力(コマンド)からドメイン集約 Item を組み立てる。
///      → このとき Requirement / ComponentLine / Item の各コンストラクタが不変条件を検査する。
///        画面のエラー5種は、すべて「集約が生成を拒否する」形でここに集約される:
///          ・品番が空          → Item コンストラクタ         (Image 6)
///          ・品名が空          → Item コンストラクタ         (Image 7)
///          ・所要量が0以下     → Requirement コンストラクタ  (Image 8)
///          ・構成部品が0件     → Item コンストラクタ         (Image 9)
///          ・部品品番が重複     → Item コンストラクタ         (Image 10)
///        ユースケースはこれらを一切“再チェックしない”。検査の在処はドメインである。
///   2. 生成できたら永続化ポートへ渡す。
///
/// つまり不変条件はドメインが持ち、手続き(組み立て→永続化)だけをここが持つ。
/// </summary>
public sealed class RegisterItemUseCase(IBomRepository repository)
{
    public async Task<RegisterItemResult> ExecuteAsync(
        RegisterItemCommand command, CancellationToken ct = default)
    {
        Item item;
        try
        {
            var components = (command.Components ?? [])
                .Select(c => new ComponentLine(
                    c.MaterialCode,
                    c.MaterialName,
                    new Requirement(c.Requirement)))
                .ToList();

            // 構成部品0件・部品品番重複・品番/品名の空は、この生成で弾かれる。
            item = new Item(command.ItemCode, command.ItemName, components);
        }
        catch (DomainException ex)
        {
            // 想定内の業務エラー。画面へ文言を返す。
            return new RegisterItemResult.ValidationError(ex.Message);
        }

        // ここに来た時点で item は妥当(全不変条件を満たす)。あとは永続化するだけ。
        await repository.RegisterAsync(item, ct);
        return new RegisterItemResult.Success();
    }
}
