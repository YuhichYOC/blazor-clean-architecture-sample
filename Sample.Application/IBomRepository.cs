using Sample.Application.ReadModels;
using Sample.Domain;

namespace Sample.Application.Abstractions;

/// <summary>
/// 部品構成表ユースケースが必要とする永続化操作の「ポート(境界インターフェース)」。
///
/// このインターフェースを Application 側に置くのが要点。
/// 依存の向きは Persistence → Application(内向き)になり、
/// Application は EF Core / Oracle を一切知らないまま保てる。
///
/// 引数・戻り値はドメイン型(<see cref="Item"/>)と Application 所有の読み取りモデル
/// (<see cref="BomRow"/>)だけで表現する。
/// EF のエンティティ・IQueryable・追跡状態・DbContext は決してこの境界を越えない。
///
/// 実装(アダプタ)は永続化側に置き、昨日の BomRepository / BomRepository.Deletion へ
/// 委譲する。削除系メソッド名の細部の突き合わせも、そのアダプタ1箇所で吸収する。
/// </summary>
public interface IBomRepository
{
    /// <summary>
    /// 画面ロード用の一覧取得。Item×Bom×Material を左外部結合し、
    /// 品番→部品品番でソート済みの行を返す。
    /// 読み取りは不変条件を必要としないため、ドメインを経由しない(CQRS の読み側)。
    /// </summary>
    Task<IReadOnlyList<BomRow>> GetBomListAsync(CancellationToken ct = default);

    /// <summary>
    /// 品番集約を1トランザクションで登録する(Item 1件・Material N件・Bom N件)。
    /// 集約は生成できた時点で全不変条件を満たしている前提。
    /// アダプタ側は集約→エンティティへ写して挿入するだけでよい。
    /// </summary>
    Task RegisterAsync(Item item, CancellationToken ct = default);

    /// <summary>指定品番を構成する部品品番の一覧(削除フローの判定材料)。</summary>
    Task<IReadOnlyList<string>> GetComponentMaterialCodesAsync(
        string itemCode, CancellationToken ct = default);

    /// <summary>
    /// 指定品番の構成部品のうち、他の品番の Bom から参照されていない
    /// (＝この品番を消すと孤児になる)部品品番の一覧。
    /// 判定は「自分の行を除いた他 Bom」を見るのがポイント。
    /// </summary>
    Task<IReadOnlyList<string>> GetOrphanMaterialCodesAsync(
        string itemCode, CancellationToken ct = default);

    /// <summary>
    /// 指定品番の Item と、それに紐づく Bom 行だけを削除する(部品マスタは残す)。
    /// マスタを消さない削除経路。
    /// </summary>
    Task DeleteItemAndBomRowsAsync(string itemCode, CancellationToken ct = default);

    /// <summary>
    /// 指定品番を、構成部品(Material)まで含めてカスケード削除する。
    /// FK 順(Bom → Material → Item)は永続化側の詳細であり、この境界には現れない。
    /// 昨日温存した既存カスケードロジックを、条件付きで呼ぶための経路。
    /// </summary>
    Task DeleteItemWithMaterialsCascadeAsync(
        string itemCode, IEnumerable<string> materialCodes, CancellationToken ct = default);
}
