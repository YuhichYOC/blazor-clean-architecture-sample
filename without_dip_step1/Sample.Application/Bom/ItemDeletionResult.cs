namespace Sample.Application.Deletion;

/// <summary>
/// 削除ユースケース第1フェーズ(<see cref="DeleteItemUseCase.BeginAsync"/>)の結果。
///
/// ユーザー対話(「品番マスタと部品マスタも削除しますか？」)を挟むため、削除は本質的に2フェーズになる。
/// </summary>
public abstract record ItemDeletionResult
{
    private ItemDeletionResult() { }

    /// <summary>
    /// 確認不要で削除完了。
    /// (構成部品の少なくとも1つが他品番でも使われている等、マスタを消せないケース)
    /// </summary>
    public sealed record Deleted : ItemDeletionResult;

    /// <summary>
    /// 構成部品が全て孤児になるため、マスタも消すかユーザー確認が必要。
    /// この時点ではまだ何も削除していない。
    /// UI は YES/NO を得て第2フェーズ(<see cref="DeleteItemUseCase.CompleteAsync"/>)を呼ぶ。
    /// </summary>
    public sealed record MasterDeletionConfirmationRequired(
        string ItemCode,
        IReadOnlyList<string> DeletableMaterialCodes) : ItemDeletionResult;
}
