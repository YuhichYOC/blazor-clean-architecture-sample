namespace Sample.Domain.Deletion;

/// <summary>
/// 品番削除時に、その品番が構成していた部品品番（Material）を削除してよいかを判定する。
/// docs/design/画面イメージ-レコード削除パターン1・2.png に対応：
///   ・削除対象の品番を構成する部品品番が「すべて」他の品番で未使用の場合のみ、
///     「部品マスタも削除しますか？」の確認ダイアログを出す（パターン1）。
///     ユーザーが YES と回答した場合のみ Material を削除する。
///   ・1件でも他の品番で使用中の部品品番があれば確認ダイアログは出さず、
///     Material は一切削除しない（パターン2）。
/// なお Item・Bom の削除は、この判定に関わらず常に行われる（削除対象品番のレコードのため）。
/// </summary>
public static class MaterialDeletionPlanner
{
    /// <summary>
    /// 「部品マスタも削除しますか？」の確認ダイアログをユーザーに出す必要があるか。
    /// </summary>
    public static bool RequiresConfirmation(IReadOnlyList<MaterialDeletionCandidate> candidates)
        => candidates.Count > 0 && candidates.All(c => !c.UsedByOtherItems);

    /// <summary>
    /// 実際に削除すべき部品品番の一覧を決定する。
    /// </summary>
    /// <param name="candidates">削除対象の品番が構成していた部品品番の一覧</param>
    /// <param name="confirmedByUser">
    /// 確認ダイアログでユーザーが YES と回答したか。
    /// <see cref="RequiresConfirmation"/> が false の場合（＝確認ダイアログを出さない場合）は無視される。
    /// </param>
    public static IReadOnlyList<string> DetermineMaterialCodesToDelete(
        IReadOnlyList<MaterialDeletionCandidate> candidates,
        bool confirmedByUser)
    {
        if (!RequiresConfirmation(candidates))
        {
            // 他の品番で使用中の部品品番が1件でもあれば、部品マスタは削除しない（パターン2）。
            return Array.Empty<string>();
        }

        return confirmedByUser
            ? candidates.Select(c => c.MaterialCode).ToArray()
            : Array.Empty<string>();
    }
}
