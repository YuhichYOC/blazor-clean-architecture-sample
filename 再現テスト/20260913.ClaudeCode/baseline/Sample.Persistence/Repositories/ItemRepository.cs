using Microsoft.EntityFrameworkCore;
using Sample.Persistence.Records;

namespace Sample.Persistence.Repositories;

/// <summary>
/// Item テーブルへの LINQ によるアクセスを行うリポジトリ。
/// </summary>
public sealed class ItemRepository
{
    private readonly SampleDbContext _context;

    public ItemRepository(SampleDbContext context)
    {
        _context = context;
    }

    public Task<ItemRecord?> FindAsync(string itemCode, CancellationToken cancellationToken = default)
        => _context.Items.FirstOrDefaultAsync(i => i.ItemCode == itemCode, cancellationToken);

    public void Add(ItemRecord record) => _context.Items.Add(record);

    public void Remove(ItemRecord record) => _context.Items.Remove(record);
}
