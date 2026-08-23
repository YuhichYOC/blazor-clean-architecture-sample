using Sample.Persistence;   // IBomDataAccess(低レベル側が所有する抽象)

namespace Sample.Application.Deletion;

/// <summary>
/// 品番削除ユースケース(2フェーズ)。
///
/// フェーズ1 <see cref="BeginAsync"/>:
///   構成部品が「全て孤児(他品番から参照されない)」なら、マスタ削除の可否をユーザーに問うため
///   <see cref="ItemDeletionResult.MasterDeletionConfirmationRequired"/> を返す(まだ何も消さない)。
///   そうでなければ、聞かずに Item と Bom 行だけ削除して
///   <see cref="ItemDeletionResult.Deleted"/> を返す。
///
/// フェーズ2 <see cref="CompleteAsync"/>:
///   ユーザー回答 YES → マスタまでカスケード削除。
///   NO           → Item と Bom 行だけ削除(マスタは残す)。
///
/// 「孤児なら同意を取ってマスタも消す」は不変条件ではなく“手続き/方針”。だからドメインではなく
/// このアプリケーション層に置く。FK 順(Bom→Material→Item)は永続化の詳細なので、この層は知らない。
///
/// ── ステップ1(DIP の反転除去)による変更点 ────────────────────────────────
///   before: DeleteItemUseCase(IBomRepository repository)
///           ポートの業務語彙メソッドを呼んでいた:
///             GetComponentMaterialCodesAsync / GetOrphanMaterialCodesAsync /
///             DeleteItemAndBomRowsAsync / DeleteItemWithMaterialsCascadeAsync
///           これらを Persistence の実名へ翻訳していたのが BomRepositoryAdapter。
///   after : DeleteItemUseCase(IBomDataAccess dataAccess)
///           翻訳層(アダプタ)が消えたため、Persistence の実名を直接呼ぶ:
///             GetMaterialCodesOfItemAsync / GetOrphanMaterialCodesAsync /
///             DeleteItemKeepMaterialsAsync / DeleteItemWithMaterialsAsync
///           → 永続化側の語彙(メソッド名)が、そのまま業務手続きに露出する。
/// </summary>
public sealed class DeleteItemUseCase(IBomDataAccess dataAccess)
{
    /// <summary>
    /// 削除の第1フェーズ。孤児判定を行い、確認が要るかどうかを返す。
    /// </summary>
    public async Task<ItemDeletionResult> BeginAsync(
        string itemCode, CancellationToken ct = default)
    {
        // 旧: repository.GetComponentMaterialCodesAsync(itemCode, ct)
        var all = await dataAccess.GetMaterialCodesOfItemAsync(itemCode, ct);
        var orphans = await dataAccess.GetOrphanMaterialCodesAsync(itemCode, ct);

        // 構成部品が1件以上あり、その全てが孤児になる場合のみユーザーに確認する。
        if (all.Count > 0 && orphans.Count == all.Count)
            return new ItemDeletionResult.MasterDeletionConfirmationRequired(itemCode, orphans);

        // 一部でも他品番が使っている(=孤児でない)なら、マスタは残し、聞かずに削除。
        // 旧: repository.DeleteItemAndBomRowsAsync(itemCode, ct)
        await dataAccess.DeleteItemKeepMaterialsAsync(itemCode, ct);
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
            // 旧: repository.DeleteItemWithMaterialsCascadeAsync(itemCode, materialCodes, ct)
            await dataAccess.DeleteItemWithMaterialsAsync(itemCode, materialCodes, ct);
        else
            // 旧: repository.DeleteItemAndBomRowsAsync(itemCode, ct)
            await dataAccess.DeleteItemKeepMaterialsAsync(itemCode, ct);
    }
}
