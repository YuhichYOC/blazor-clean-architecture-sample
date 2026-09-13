using Microsoft.EntityFrameworkCore;
using Sample.Persistence.Records;

namespace Sample.Persistence.Repositories;

/// <summary>
/// Material テーブルへの LINQ によるアクセスを行うリポジトリ。
/// </summary>
public sealed class MaterialRepository
{
    private readonly SampleDbContext _context;

    public MaterialRepository(SampleDbContext context)
    {
        _context = context;
    }

    public Task<MaterialRecord?> FindAsync(string itemCode, CancellationToken cancellationToken = default)
        => _context.Materials.FirstOrDefaultAsync(m => m.ItemCode == itemCode, cancellationToken);

    public Task<List<MaterialRecord>> FindManyAsync(
        IReadOnlyCollection<string> itemCodes,
        CancellationToken cancellationToken = default)
        => _context.Materials.Where(m => itemCodes.Contains(m.ItemCode)).ToListAsync(cancellationToken);

    public void Add(MaterialRecord record) => _context.Materials.Add(record);

    public void RemoveRange(IEnumerable<MaterialRecord> records) => _context.Materials.RemoveRange(records);
}
