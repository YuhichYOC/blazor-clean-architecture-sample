using Microsoft.EntityFrameworkCore;
using Sample.Domain.Dtos;
using Sample.Persistence.Records;

namespace Sample.Persistence.Repositories;

/// <summary>
/// Bom テーブルへの LINQ によるアクセスを行うリポジトリ。
/// </summary>
public sealed class BomRepository
{
    private readonly SampleDbContext _context;

    public BomRepository(SampleDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 部品構成表登録画面の一覧表示データを取得する
    /// （docs/design/画面イメージ-画面のロード時.png のデータ取得ロジックに準拠）。
    /// Bom を基準に Item・Material を左外部結合し、単純な SELECT を
    /// <see cref="BomListItemDto"/> へ射影する。
    /// 品番（Item.item_code）→ 部品品番（Material.item_code）の順でソートする。
    /// </summary>
    public Task<List<BomListItemDto>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var query =
            from bom in _context.Boms
            join item in _context.Items on bom.ItemCode equals item.ItemCode into itemGroup
            from item in itemGroup.DefaultIfEmpty()
            join material in _context.Materials on bom.MItemCode equals material.ItemCode into materialGroup
            from material in materialGroup.DefaultIfEmpty()
            orderby item.ItemCode, material.ItemCode
            select new BomListItemDto(
                item.ItemCode,
                item.ItemName,
                material.ItemCode,
                material.ItemName,
                bom.Requirement);

        return query.ToListAsync(cancellationToken);
    }

    /// <summary>指定した品番が構成する Bom 明細を取得する。</summary>
    public Task<List<BomRecord>> GetByItemCodeAsync(string itemCode, CancellationToken cancellationToken = default)
        => _context.Boms.Where(b => b.ItemCode == itemCode).ToListAsync(cancellationToken);

    /// <summary>
    /// 指定した部品品番のうち、<paramref name="excludingItemCode"/> 以外の品番でも
    /// 使用されているものを取得する（削除パターン1/2の判定に使用）。
    /// </summary>
    public async Task<HashSet<string>> GetMaterialCodesUsedByOtherItemsAsync(
        IReadOnlyCollection<string> materialCodes,
        string excludingItemCode,
        CancellationToken cancellationToken = default)
    {
        var used = await _context.Boms
            .Where(b => b.ItemCode != excludingItemCode && materialCodes.Contains(b.MItemCode))
            .Select(b => b.MItemCode)
            .Distinct()
            .ToListAsync(cancellationToken);

        return used.ToHashSet();
    }

    public void Add(BomRecord record) => _context.Boms.Add(record);

    public void AddRange(IEnumerable<BomRecord> records) => _context.Boms.AddRange(records);

    public void RemoveRange(IEnumerable<BomRecord> records) => _context.Boms.RemoveRange(records);
}
