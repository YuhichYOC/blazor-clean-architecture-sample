using Sample.Persistence.Dtos;

namespace Sample.Persistence;

/// <summary>部品構成表登録画面のデータアクセス。</summary>
public interface IBomDataAccess
{
    /// <summary>画面ロード: 全 Bom を Item/Material と結合し、品番→部品品番でソートして取得。</summary>
    Task<IReadOnlyList<BomRowDto>> GetBomListAsync(CancellationToken ct = default);

    /// <summary>追加登録: Item 1件・Material N件・Bom N件を1トランザクションで挿入。</summary>
    Task RegisterAsync(ItemRegistrationDto registration, CancellationToken ct = default);

    /// <summary>削除: 指定品番の Item・その構成部品(Material)・紐づく Bom を1トランザクションで削除。</summary>
    Task DeleteItemsAsync(IEnumerable<string> itemCodes, CancellationToken ct = default);

    Task<IReadOnlyList<string>> GetMaterialCodesOfItemAsync(string itemCode, CancellationToken ct = default);

    Task<IReadOnlyList<string>> GetOrphanMaterialCodesAsync(string itemCode, CancellationToken ct = default);

    Task DeleteItemKeepMaterialsAsync(string itemCode, CancellationToken ct = default);

    Task DeleteItemWithMaterialsAsync(string itemCode, IEnumerable<string> materialCodes, CancellationToken ct = default);
}