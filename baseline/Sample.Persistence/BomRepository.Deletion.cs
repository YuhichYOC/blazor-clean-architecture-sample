using Microsoft.EntityFrameworkCore;

namespace Sample.Persistence;

// ─────────────────────────────────────────────────────────────────────────────
// 既存 BomRepository.cs の宣言を
//     public class    BomRepository(IDbContextFactory<BomDbContext> factory) : IBomRepository
//   → public partial class BomRepository(IDbContextFactory<BomDbContext> factory) : IBomRepository
// に変更する(partial を足すだけ)。プライマリコンストラクタの factory は
// この partial 側からもそのまま参照できる。
//
// 既存の DeleteItemsAsync(全カスケード) は「来月案件の参考」としてそのまま温存する。
// ここで足すのは (A) 孤児判定クエリ 2本 と (B) 条件分岐した削除 2本。
// 「いつ聞くか / どちらを呼ぶか」の判断はここには書かない(= アプリケーション層の責務)。
// ─────────────────────────────────────────────────────────────────────────────
public partial class BomRepository
{
    // ── (A) 孤児判定クエリ ───────────────────────────────────────────────

    /// <summary>
    /// 指定品番が構成部品として使っている部品品番の一覧(重複なし)。
    /// アプリケーション層はこの件数と孤児件数を比較して「全部孤児か?」を判定する。
    /// </summary>
    public async Task<IReadOnlyList<string>> GetMaterialCodesOfItemAsync(
        string itemCode, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();

        return await db.Boms
            .Where(b => b.ItemCode == itemCode)
            .Select(b => b.MItemCode)
            .Distinct()
            .ToListAsync(ct);
    }

    /// <summary>
    /// 指定品番の構成部品のうち、他の品番の Bom から参照されていない(= 孤児となる)部品品番。
    /// 「自分の行を除いた他 Bom」を見るのがポイント。削除してから空振りに気づく事態を避ける。
    /// </summary>
    public async Task<IReadOnlyList<string>> GetOrphanMaterialCodesAsync(
        string itemCode, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();

        // この品番が使う部品品番
        var mine = db.Boms
            .Where(b => b.ItemCode == itemCode)
            .Select(b => b.MItemCode)
            .Distinct();

        // 他の品番がまだ使っている部品品番
        var usedByOthers = db.Boms
            .Where(b => b.ItemCode != itemCode)
            .Select(b => b.MItemCode)
            .Distinct();

        // 差集合(= 他で使われていない = 孤児)。EF Core が NOT IN 相当へ変換する。
        return await mine
            .Where(m => !usedByOthers.Contains(m))
            .ToListAsync(ct);
    }

    // ── (B) 条件分岐した削除 ─────────────────────────────────────────────
    //
    // 設計上の注意(重要):
    //   要件は「品番マスタと部品マスタも削除しますか?」と両者を条件付きにしているが、
    //   ドメイン不変条件「品番は構成部品を1件以上持つ」により、Bom を全削除した品番の
    //   Item マスタを残すと “0件の品番” = 復元不能な不正状態が DB に残る。
    //   そのため Item マスタは常に Bom 行と不可分に削除する。
    //   → 条件分岐で本当に揺れるのは「共有マスタである 部品マスタ を消すか否か」だけ。
    //
    // どちらのメソッドも FK 制約(DeleteBehavior.Restrict)を満たす順序で消す:
    //   Bom → Material → Item

    /// <summary>
    /// 指定品番の Bom 行と Item マスタを削除する(部品マスタは残す)。
    /// 「聞かない場合」および「聞いて NO の場合」に使う。
    /// </summary>
    public async Task DeleteItemKeepMaterialsAsync(
        string itemCode, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        await db.Boms .Where(b => b.ItemCode == itemCode).ExecuteDeleteAsync(ct);
        await db.Items.Where(i => i.ItemCode == itemCode).ExecuteDeleteAsync(ct);

        await tx.CommitAsync(ct);
    }

    /// <summary>
    /// 指定品番の Bom 行・Item マスタ・指定した孤児部品マスタをまとめて削除する。
    /// 「聞いて YES の場合」に使う。
    /// materialCodes には GetOrphanMaterialCodesAsync で「他品番から参照されていないと
    /// 確認済みの」部品品番のみを渡すこと(共有中の部品を渡すと FK 制約で失敗する)。
    /// </summary>
    public async Task DeleteItemWithMaterialsAsync(
        string itemCode, IEnumerable<string> materialCodes, CancellationToken ct = default)
    {
        var codes = materialCodes?.Distinct().ToList() ?? new List<string>();

        await using var db = factory.CreateDbContext();
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        await db.Boms.Where(b => b.ItemCode == itemCode).ExecuteDeleteAsync(ct);

        if (codes.Count > 0)
            await db.Materials.Where(m => codes.Contains(m.ItemCode)).ExecuteDeleteAsync(ct);

        await db.Items.Where(i => i.ItemCode == itemCode).ExecuteDeleteAsync(ct);

        await tx.CommitAsync(ct);
    }
}
