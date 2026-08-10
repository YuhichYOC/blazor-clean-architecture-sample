using Microsoft.EntityFrameworkCore;
using Sample.Persistence.Dtos;
using Sample.Persistence.Entities;

namespace Sample.Persistence;

/// <summary>
/// Blazor(Auto/Server)で安全に使うため、IDbContextFactory から操作ごとに短命な DbContext を生成する。
/// (回線に紐づく長命なスコープド DbContext を共有しないことで、並行アクセス例外・変更追跡の肥大化を回避)
/// </summary>
public partial class BomRepository(IDbContextFactory<BomDbContext> factory) : IBomDataAccess
{
    // ── 画面ロード ──────────────────────────────────────────────
    // Item 左外部結合 Bom、Bom→Material で名称を引く。品番→部品品番でソート。
    // DTO への射影のため変更追跡は行われない(AsNoTracking 不要)。
    public async Task<IReadOnlyList<BomRowDto>> GetBomListAsync(CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();

        var query =
            from item in db.Items
            join bom in db.Boms on item.ItemCode equals bom.ItemCode into bomGroup
            from bom in bomGroup.DefaultIfEmpty()                 // 左外部結合
            join mat in db.Materials on bom.MItemCode equals mat.ItemCode into matGroup
            from mat in matGroup.DefaultIfEmpty()
            orderby item.ItemCode, bom.MItemCode
            select new BomRowDto(
                item.ItemCode,
                item.ItemName,
                bom != null ? bom.MItemCode : null,
                mat != null ? mat.ItemName : null,
                bom != null ? bom.Requirement : (decimal?)null);

        return await query.ToListAsync(ct);
    }

    // ── 追加登録 ────────────────────────────────────────────────
    // Item 1件・Material N件・Bom N件を1トランザクションで挿入。
    public async Task RegisterAsync(ItemRegistrationDto registration, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        db.Items.Add(new Item
        {
            ItemCode = registration.ItemCode,
            ItemName = registration.ItemName
        });

        foreach (var c in registration.Components)
        {
            // このサンプルでは部品品番は品番ごとに一意(共有しない)前提。
            // 部品を複数品番で共有し得る運用では、
            // 「既存 Material を検索し、無ければ挿入」に変更すること(重複PK回避)。
            db.Materials.Add(new Material
            {
                ItemCode = c.MItemCode,
                ItemName = c.MItemName
            });

            db.Boms.Add(new Bom
            {
                ItemCode = registration.ItemCode,
                MItemCode = c.MItemCode,
                Requirement = c.Requirement
            });
        }

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    // ── 削除 ────────────────────────────────────────────────────
    // 指定品番について Bom → Material → Item の順に削除(FKの子から先に消す)。全体を1トランザクション。
    public async Task DeleteItemsAsync(IEnumerable<string> itemCodes, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        foreach (var itemCode in itemCodes.Distinct())
        {
            // この品番を構成する部品品番を先に把握しておく
            var materialCodes = await db.Boms
                .Where(b => b.ItemCode == itemCode)
                .Select(b => b.MItemCode)
                .Distinct()
                .ToListAsync(ct);

            // 1) 部品構成表(Bom)を削除
            await db.Boms
                .Where(b => b.ItemCode == itemCode)
                .ExecuteDeleteAsync(ct);

            // 2) 構成部品(Material)を削除
            await db.Materials
                .Where(m => materialCodes.Contains(m.ItemCode))
                .ExecuteDeleteAsync(ct);

            // 3) 品番(Item)を削除
            await db.Items
                .Where(i => i.ItemCode == itemCode)
                .ExecuteDeleteAsync(ct);
        }

        await tx.CommitAsync(ct);
    }
}
