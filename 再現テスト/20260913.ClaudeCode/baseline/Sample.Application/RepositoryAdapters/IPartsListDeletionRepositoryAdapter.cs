using Sample.Domain.Deletion;

namespace Sample.Application.RepositoryAdapters;

/// <summary>
/// 部品構成表のレコード削除を行うリポジトリアダプター。
/// 実装はパーシステント層で Bom テーブルを参照して行う。
/// </summary>
public interface IPartsListDeletionRepositoryAdapter
{
    /// <summary>
    /// 削除対象の品番が構成する部品品番と、それぞれが他の品番でも
    /// 使用されているかどうかを取得する（削除パターン1/2の判定に使用）。
    /// </summary>
    Task<IReadOnlyList<MaterialDeletionCandidate>> GetMaterialDeletionCandidatesAsync(
        string itemCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 品番を削除する。
    /// Bom（削除対象品番の明細）・Item（削除対象品番）は常に削除し、
    /// <paramref name="materialCodesToDelete"/> に指定された部品品番のみ
    /// Material からも削除する（0件の場合は部品マスタを削除しない＝パターン2）。
    /// change tracking & SaveChanges による単一トランザクションで行う。
    /// </summary>
    Task DeleteAsync(
        string itemCode,
        IReadOnlyList<string> materialCodesToDelete,
        CancellationToken cancellationToken = default);
}
