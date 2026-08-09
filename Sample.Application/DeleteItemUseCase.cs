using Sample.Application.Abstractions;

namespace Sample.Application.Deletion;

/// <summary>
/// 品番削除ユースケース(2フェーズ)。
///
/// フェーズ1 <see cref="BeginAsync"/>:
///   構成部品が「全て孤児(他品番から参照されない)」なら、マスタ削除の可否をユーザーに問うため
///   <see cref="ItemDeletionResult.MasterDeletionConfirmationRequired"/> を返す(まだ何も消さない)。
///   そうでなければ、聞かずに Item と Bom 行だけ削除して
///   <see cref="ItemDeletionResult.Deleted"/> を返す(Image 5 のパターン)。
///
/// フェーズ2 <see cref="CompleteAsync"/>:
///   ユーザー回答 YES → マスタまでカスケード削除(Image 4 のパターン)。
///   NO           → Item と Bom 行だけ削除(マスタは残す)。
///
/// 「孤児なら同意を取ってマスタも消す」は不変条件ではなく“手続き/方針”。
/// 孤児のマスタが残っていても DB は正しい状態のままだし、同意を取る分岐自体が流れの一部。
/// だからドメインではなく、このアプリケーション層に置く。
/// FK 順(Bom→Material→Item)は永続化の詳細なので、この層は一切知らない。
/// </summary>
public sealed class DeleteItemUseCase(IBomRepository repository)
{
    /// <summary>
    /// 削除の第1フェーズ。孤児判定を行い、確認が要るかどうかを返す。
    /// </summary>
    public async Task<ItemDeletionResult> BeginAsync(
        string itemCode, CancellationToken ct = default)
    {
        var all = await repository.GetComponentMaterialCodesAsync(itemCode, ct);
        var orphans = await repository.GetOrphanMaterialCodesAsync(itemCode, ct);

        // 構成部品が1件以上あり、その全てが孤児になる場合のみユーザーに確認する。
        if (all.Count > 0 && orphans.Count == all.Count)
            return new ItemDeletionResult.MasterDeletionConfirmationRequired(itemCode, orphans);

        // 一部でも他品番が使っている(=孤児でない)なら、マスタは残し、聞かずに削除。
        await repository.DeleteItemAndBomRowsAsync(itemCode, ct);
        return new ItemDeletionResult.Deleted();
    }

    /// <summary>
    /// 削除の第2フェーズ。第1フェーズが確認要求を返したときのみ、ユーザー回答を受けて呼ぶ。
    /// </summary>
    /// <param name="alsoDeleteMasters">ユーザー回答が YES なら true。</param>
    public async Task CompleteAsync(
        string itemCode, bool alsoDeleteMasters, IEnumerable<string> materialCodes, CancellationToken ct = default)
    {
        if (alsoDeleteMasters)
            await repository.DeleteItemWithMaterialsCascadeAsync(itemCode, materialCodes, ct);
        else
            await repository.DeleteItemAndBomRowsAsync(itemCode, ct);
    }
}
