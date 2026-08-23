using Sample.Domain;
using Sample.Persistence;         // IBomDataAccess … 低レベル側(Persistence)が所有する抽象。
                                  //   ★ステップ1の要点: これはもう Application 所有のポートではない。
                                  //     依存の向きは Application → Persistence(外向き)に反転している。
using Sample.Persistence.Dtos;    // ItemRegistrationDto / ComponentDto
                                  //   ★DIP(反転)を外した代償: 永続化の DTO が Application まで漏れてくる。
                                  //     反転がある間は、この変換は Persistence 側の BomRepositoryAdapter が
                                  //     吸収していたため、Application はこれらの型を一切知らずに済んでいた。

namespace Sample.Application.Registration;

/// <summary>
/// 品番登録ユースケース。
///
/// 役割は「オーケストレーション」だけ:
///   1. 入力(コマンド)からドメイン集約 Item を組み立てる。
///      → このとき Requirement / ComponentLine / Item の各コンストラクタが不変条件を検査する。
///        画面のエラー5種は、すべて「集約が生成を拒否する」形でここに集約される:
///          ・品番が空          → Item コンストラクタ
///          ・品名が空          → Item コンストラクタ
///          ・所要量が0以下     → Requirement コンストラクタ
///          ・構成部品が0件     → Item コンストラクタ
///          ・部品品番が重複     → Item コンストラクタ
///        ユースケースはこれらを一切“再チェックしない”。検査の在処はドメインである。
///   2. 生成できたら永続化へ渡す。
///
/// ── ステップ1(DIP の反転除去)による変更点 ────────────────────────────────
///   before: RegisterItemUseCase(IBomRepository repository)
///           → Application 所有のポートに依存。実装(EF)は Persistence 側のアダプタが提供し、
///             ドメイン集約 → 永続化DTO の変換もアダプタが担っていた。
///   after : RegisterItemUseCase(IBomDataAccess dataAccess)
///           → Persistence 所有の抽象に直接依存。ポートとアダプタが消えたため、
///             「集約 → DTO」の変換の行き場がこのユースケース本体しか無くなった(下記 ★変換 参照)。
///
///   つまり「反転」を外した瞬間、境界の翻訳が消え、永続化の語彙(DTO)が業務手続きに混入する。
///   これが DIP が買っていたもの(語彙の隔離)の可視化である。
/// </summary>
public sealed class RegisterItemUseCase(IBomDataAccess dataAccess)
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

        // ★変換(旧 BomRepositoryAdapter.RegisterAsync より移設):
        //   ドメイン集約 Item → 永続化DTO ItemRegistrationDto。
        //   Requirement 値オブジェクト → decimal は c.Requirement.Value で取り出す。
        var registration = new ItemRegistrationDto(
            item.ItemCode,
            item.ItemName,
            item.Components
                .Select(c => new ComponentDto(
                    c.MaterialCode,
                    c.MaterialName,
                    c.Requirement.Value))
                .ToList());

        // ここに来た時点で item は妥当(全不変条件を満たす)。あとは永続化するだけ。
        await dataAccess.RegisterAsync(registration, ct);
        return new RegisterItemResult.Success();
    }
}
