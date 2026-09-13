namespace Sample.Domain.Deletion;

/// <summary>
/// 品番削除時に、部品マスタ（Material）削除の対象になりうる部品品番1件と、
/// その部品品番が削除対象の品番以外でも使用されているかどうか。
/// 「他の品番で使用されているか」の判定は Bom テーブルを参照する必要があるため、
/// 呼び出し側（アプリケーション層）が事前に調べて渡す。
/// </summary>
/// <param name="MaterialCode">部品品番</param>
/// <param name="UsedByOtherItems">削除対象の品番以外の Bom レコードでも使用されているか</param>
public sealed record MaterialDeletionCandidate(string MaterialCode, bool UsedByOtherItems);
