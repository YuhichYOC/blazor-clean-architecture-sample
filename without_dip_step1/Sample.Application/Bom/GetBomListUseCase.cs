using Sample.Application.ReadModels;   // BomRow(Application 所有の読み取りモデル。UI はこれを使う)
using Sample.Persistence;              // IBomDataAccess(低レベル側が所有する抽象)
using Sample.Persistence.Dtos;         // BomRowDto ← ★DIP(反転)を外した代償: 永続化DTOがここまで漏れる

namespace Sample.Application.Query;

/// <summary>
/// 一覧取得ユースケース(画面ロード)。
///
/// 読み取りは不変条件を要さないため、ドメインを通さず抽象→読み取りモデルで直行する(CQRS の読み側)。
///
/// ── ステップ1(DIP の反転除去)による変更点 ────────────────────────────────
///   before: GetBomListUseCase(IBomRepository repository)
///           repository.GetBomListAsync() は既に BomRow(Application 型)を返していた。
///           BomRowDto → BomRow の詰め替えは Persistence 側の BomRepositoryAdapter が担当。
///   after : GetBomListUseCase(IBomDataAccess dataAccess)
///           dataAccess.GetBomListAsync() は BomRowDto(Persistence 型)を返す。
///           よって「DTO → 読み取りモデル」の詰め替えがこのユースケースへ移動した(下記 ★変換)。
///
/// 正直な注記(ベースライン時点のものを維持):
///   この読み側は薄い。UI が抽象を直接叩いても機能は変わらない。
///   価値を持ち始めるのは、読み取りに「認可・整形・複数ソース統合」等の方針が乗ったとき。
///   ただし反転を外した今、UI が dataAccess を直接叩くと UI まで BomRowDto を知ることになる。
///   このユースケースが DTO を BomRow に閉じ込める“防波堤”として機能している点に注意。
/// </summary>
public sealed class GetBomListUseCase(IBomDataAccess dataAccess)
{
    public async Task<IReadOnlyList<BomRow>> ExecuteAsync(CancellationToken ct = default)
    {
        var rows = await dataAccess.GetBomListAsync(ct);

        // ★変換(旧 BomRepositoryAdapter.GetBomListAsync より移設):
        //   永続化DTO BomRowDto → 読み取りモデル BomRow。
        //   MItemCode/MItemName(部品品番/部品品名) → MaterialCode/MaterialName へ名称対応。
        return rows
            .Select(d => new BomRow(
                d.ItemCode,
                d.ItemName,
                d.MItemCode,
                d.MItemName,
                d.Requirement))
            .ToList();
    }
}
