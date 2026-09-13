using Sample.Domain.Deletion;

namespace Sample.Application.UseCases;

/// <summary>
/// <see cref="PrepareItemDeletionUseCase"/> の実行結果。
/// </summary>
/// <param name="Candidates">削除対象の品番が構成していた部品品番の一覧と他品番での使用状況</param>
/// <param name="RequiresConfirmation">
/// 「部品マスタも削除しますか？」の確認ダイアログをユーザーに出す必要があるか
/// （docs/design/画面イメージ-レコード削除パターン1.png 参照）。
/// false の場合は確認なしで <see cref="DeleteItemUseCase"/> を実行してよい
/// （部品マスタは削除されない＝パターン2）。
/// </param>
public sealed record DeletionPreparation(
    IReadOnlyList<MaterialDeletionCandidate> Candidates,
    bool RequiresConfirmation);
