using Sample.Application.ReadModels;
using Sample.Persistence.Dtos;

// 2つの IBomRepository を区別するための別名。
//   PortIBomRepository … Application が所有するポート(内側)。このクラスが「実装する」相手。
//   inner の型         … Sample.Persistence.IBomRepository(昨日の EF/Oracle 実装)。「委譲する」相手。
using PortIBomRepository = Sample.Application.Abstractions.IBomRepository;
using DomainItem = Sample.Domain.Item;

namespace Sample.Persistence;

/// <summary>
/// 永続化アダプタ。
///
/// 向きに注意:
///   このクラスは「Application → Persistence へアクセスする道具」ではない。
///   Persistence 側に置かれ、Application が定義したポート <see cref="PortIBomRepository"/> を
///   “実装する”。依存の矢印は Persistence → Application(内向き)。
///   Application はこのクラスの存在を知らない。
///
/// 2つの面を持つプラグ:
///   片面 = Application のソケット(ポート)。戻り値・引数はドメイン型 / 読み取りモデル。
///   もう片面 = 昨日の <see cref="BomRepository"/>(EF/Oracle)。
///   中央で両者の語彙を翻訳する(だから「アダプタ＝変換器」)。
///
/// 既存の BomRepository / BomRepository.Deletion は一切書き換えず、delegate 先として温存する。
/// </summary>
public sealed class BomRepositoryAdapter(IBomDataAccess inner) : PortIBomRepository
{
    // ── 一覧取得(読み側): 永続化DTO → Application 読み取りモデルへ写す ──
    public async Task<IReadOnlyList<BomRow>> GetBomListAsync(CancellationToken ct = default)
    {
        var rows = await inner.GetBomListAsync(ct);
        return rows
            .Select(d => new BomRow(
                d.ItemCode,
                d.ItemName,
                d.MItemCode,     // ★要確認: BomRowDto の「部品品番」プロパティ名
                d.MItemName,     // ★要確認: BomRowDto の「部品品名」プロパティ名
                d.Requirement))
            .ToList();
    }

    // ── 登録(書き側): ドメイン集約 Item → 永続化DTO へ写す ──
    public Task RegisterAsync(DomainItem item, CancellationToken ct = default)
    {
        // ★要確認: ItemRegistrationDto / ComponentDto の生成方法。
        //   ここでは位置引数(positional record)を仮定。
        //   もし init プロパティのクラスなら object initializer に置き換える。
        var dto = new ItemRegistrationDto(
            item.ItemCode,
            item.ItemName,
            item.Components
                .Select(c => new ComponentDto(
                    c.MaterialCode,
                    c.MaterialName,
                    c.Requirement.Value))   // Requirement 値オブジェクト → decimal
                .ToList());

        return inner.RegisterAsync(dto, ct);
    }

    // ── 削除フローの判定材料 ──
    // ★要確認: 昨日 BomRepository.Deletion.cs に付けたメソッド名に合わせる(下記は sketch 時の仮名)。
    //   戻り値が List<string> でも、await して IReadOnlyList<string> として返せば型は合う。
    public async Task<IReadOnlyList<string>> GetComponentMaterialCodesAsync(
        string itemCode, CancellationToken ct = default)
        => await inner.GetMaterialCodesOfItemAsync(itemCode, ct);

    public async Task<IReadOnlyList<string>> GetOrphanMaterialCodesAsync(
        string itemCode, CancellationToken ct = default)
        => await inner.GetOrphanMaterialCodesAsync(itemCode, ct);

    // ── 削除の2経路 ──
    // Item と Bom 行だけ削除(Material は温存)。
    public Task DeleteItemAndBomRowsAsync(string itemCode, CancellationToken ct = default)
        => inner.DeleteItemKeepMaterialsAsync(itemCode, ct);          // ★要確認: 実名

    // 構成部品(Material)まで含めてカスケード削除。FK 順は inner 側の詳細。
    public Task DeleteItemWithMaterialsCascadeAsync(string itemCode, IEnumerable<string> materialCodes, CancellationToken ct = default)
        => inner.DeleteItemWithMaterialsAsync(itemCode, materialCodes, ct); // ★要確認: 実名
}