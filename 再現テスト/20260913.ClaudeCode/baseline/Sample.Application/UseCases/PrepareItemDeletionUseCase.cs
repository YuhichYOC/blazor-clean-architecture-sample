using Sample.Application.RepositoryAdapters;
using Sample.Domain.Deletion;

namespace Sample.Application.UseCases;

/// <summary>
/// 「チェックしたアイテムを削除」ボタン押下時に、削除の前段として
/// 部品マスタ削除の確認要否と削除候補を求めるユースケース。
/// </summary>
public sealed class PrepareItemDeletionUseCase
{
    private readonly IPartsListDeletionRepositoryAdapter _deletionRepositoryAdapter;

    public PrepareItemDeletionUseCase(IPartsListDeletionRepositoryAdapter deletionRepositoryAdapter)
    {
        _deletionRepositoryAdapter = deletionRepositoryAdapter;
    }

    public async Task<DeletionPreparation> ExecuteAsync(
        string itemCode,
        CancellationToken cancellationToken = default)
    {
        var candidates = await _deletionRepositoryAdapter.GetMaterialDeletionCandidatesAsync(itemCode, cancellationToken);
        var requiresConfirmation = MaterialDeletionPlanner.RequiresConfirmation(candidates);

        return new DeletionPreparation(candidates, requiresConfirmation);
    }
}
