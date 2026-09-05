using Sample.Api.Contracts.Bom;
using Sample.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Sample.Api.Data.Bom;

/// <summary>
/// baseline の BomRepository / BomRepository.Deletion を、このサーバーへほぼ逐語的に移設したもの。
/// 差分は「引数・戻り値が Sample.Persistence の DTO → このAPI独自の契約型」に変わった点だけ。
///
/// 最重要:
///   BeginTransactionAsync ... CommitAsync の境界はここ(サーバー側)に残る。
///   1エンドポイント = 1 Unit of Work。HTTP はトランザクションをまたげないため、
///   各メソッドを原子的に実装することが差し替え版の前提になる。
/// </summary>
public sealed class BomDataService(IDbContextFactory<BomDbContext> factory)
{
    // ── 一覧 ────────────────────────────────────────────────────
    public async Task<IReadOnlyList<BomRowResponse>> GetBomListAsync(CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();
 
        var query =
            from item in db.Items
            join bom in db.Boms on item.ItemCode equals bom.ItemCode into bomGroup
            from bom in bomGroup.DefaultIfEmpty()
            join mat in db.Materials on bom.MItemCode equals mat.ItemCode into matGroup
            from mat in matGroup.DefaultIfEmpty()
            orderby item.ItemCode, bom.MItemCode
            select new BomRowResponse(
                item.ItemCode,
                item.ItemName,
                bom != null ? bom.MItemCode : null,
                mat != null ? mat.ItemName : null,
                bom != null ? bom.Requirement : (decimal?)null);
 
        return await query.ToListAsync(ct);
    }
 
    // ── 登録(1トランザクション) ──────────────────────────────────
    public async Task RegisterAsync(ItemRegistrationRequest reg, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();
        await using var tx = await db.Database.BeginTransactionAsync(ct);
 
        db.Items.Add(new Item { ItemCode = reg.ItemCode, ItemName = reg.ItemName });
 
        foreach (var c in reg.Components)
        {
            // ★注意: baseline 同様「部品品番は品番ごとに一意」前提。共有運用では upsert に変更。
            db.Materials.Add(new Material { ItemCode = c.MItemCode, ItemName = c.MItemName });
            db.Boms.Add(new Sample.Api.Entities.Bom { ItemCode = reg.ItemCode, MItemCode = c.MItemCode, Requirement = c.Requirement });
        }
 
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }
 
    // ── 一括カスケード削除(Bom → Material → Item、1トランザクション) ──
    public async Task DeleteItemsAsync(IEnumerable<string> itemCodes, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();
        await using var tx = await db.Database.BeginTransactionAsync(ct);
 
        foreach (var itemCode in itemCodes.Distinct())
        {
            var materialCodes = await db.Boms
                .Where(b => b.ItemCode == itemCode)
                .Select(b => b.MItemCode).Distinct().ToListAsync(ct);
 
            await db.Boms.Where(b => b.ItemCode == itemCode).ExecuteDeleteAsync(ct);
            await db.Materials.Where(m => materialCodes.Contains(m.ItemCode)).ExecuteDeleteAsync(ct);
            await db.Items.Where(i => i.ItemCode == itemCode).ExecuteDeleteAsync(ct);
        }
 
        await tx.CommitAsync(ct);
    }
 
    // ── 孤児判定 ────────────────────────────────────────────────
    public async Task<IReadOnlyList<string>> GetMaterialCodesOfItemAsync(
        string itemCode, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();
        return await db.Boms
            .Where(b => b.ItemCode == itemCode)
            .Select(b => b.MItemCode).Distinct().ToListAsync(ct);
    }
 
    public async Task<IReadOnlyList<string>> GetOrphanMaterialCodesAsync(
        string itemCode, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();
 
        var mine = db.Boms.Where(b => b.ItemCode == itemCode).Select(b => b.MItemCode).Distinct();
        var usedByOthers = db.Boms.Where(b => b.ItemCode != itemCode).Select(b => b.MItemCode).Distinct();
 
        return await mine.Where(m => !usedByOthers.Contains(m)).ToListAsync(ct);
    }
 
    // ── 条件分岐削除(いずれも Bom → (Material) → Item、1トランザクション) ──
    public async Task DeleteItemKeepMaterialsAsync(string itemCode, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();
        await using var tx = await db.Database.BeginTransactionAsync(ct);
 
        await db.Boms.Where(b => b.ItemCode == itemCode).ExecuteDeleteAsync(ct);
        await db.Items.Where(i => i.ItemCode == itemCode).ExecuteDeleteAsync(ct);
 
        await tx.CommitAsync(ct);
    }
 
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
