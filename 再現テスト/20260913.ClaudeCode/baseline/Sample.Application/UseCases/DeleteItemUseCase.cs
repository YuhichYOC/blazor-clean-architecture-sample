using Sample.Application.RepositoryAdapters;
using Sample.Domain.Deletion;

namespace Sample.Application.UseCases;

/// <summary>
/// 品番の削除を実行するユースケース。
/// <see cref="PrepareItemDeletionUseCase"/> で取得した候補と、
/// （確認ダイアログを出した場合の）ユーザーの回答を受け取り、
/// 削除パターン1/2に従って Item・Bom・Material を削除する。
/// </summary>
public sealed class DeleteItemUseCase
{
    private readonly IPartsListDeletionRepositoryAdapter _deletionRepositoryAdapter;

    public DeleteItemUseCase(IPartsListDeletionRepositoryAdapter deletionRepositoryAdapter)
    {
        _deletionRepositoryAdapter = deletionRepositoryAdapter;
    }

    /// <param name="itemCode">削除対象の品番</param>
    /// <param name="candidates">
    /// <see cref="PrepareItemDeletionUseCase"/> で取得した削除候補。
    /// </param>
    /// <param name="confirmedByUser">
    /// 確認ダイアログでユーザーが YES と回答したか
    /// （<see cref="DeletionPreparation.RequiresConfirmation"/> が false の場合は無視される）。
    /// </param>
    public Task ExecuteAsync(
        string itemCode,
        IReadOnlyList<MaterialDeletionCandidate> candidates,
        bool confirmedByUser,
        CancellationToken cancellationToken = default)
    {
        var materialCodesToDelete = MaterialDeletionPlanner.DetermineMaterialCodesToDelete(candidates, confirmedByUser);
        return _deletionRepositoryAdapter.DeleteAsync(itemCode, materialCodesToDelete, cancellationToken);
    }
}
