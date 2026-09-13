using Sample.Application.RepositoryAdapters;
using Sample.Domain.Deletion;
using Sample.Persistence.Repositories;

namespace Sample.Persistence.RepositoryAdapters;

/// <summary>
/// <see cref="IPartsListDeletionRepositoryAdapter"/> の実装。
/// docs/design/画面イメージ-レコード削除パターン1・2.png 参照。
/// </summary>
public sealed class PartsListDeletionRepositoryAdapter : IPartsListDeletionRepositoryAdapter
{
    private readonly SampleDbContext _context;
    private readonly ItemRepository _itemRepository;
    private readonly MaterialRepository _materialRepository;
    private readonly BomRepository _bomRepository;

    public PartsListDeletionRepositoryAdapter(
        SampleDbContext context,
        ItemRepository itemRepository,
        MaterialRepository materialRepository,
        BomRepository bomRepository)
    {
        _context = context;
        _itemRepository = itemRepository;
        _materialRepository = materialRepository;
        _bomRepository = bomRepository;
    }

    public async Task<IReadOnlyList<MaterialDeletionCandidate>> GetMaterialDeletionCandidatesAsync(
        string itemCode,
        CancellationToken cancellationToken = default)
    {
        var bomRecords = await _bomRepository.GetByItemCodeAsync(itemCode, cancellationToken);
        var materialCodes = bomRecords.Select(b => b.MItemCode).Distinct().ToArray();

        if (materialCodes.Length == 0)
        {
            return Array.Empty<MaterialDeletionCandidate>();
        }

        var usedByOtherItems = await _bomRepository.GetMaterialCodesUsedByOtherItemsAsync(
            materialCodes, itemCode, cancellationToken);

        return materialCodes
            .Select(code => new MaterialDeletionCandidate(code, usedByOtherItems.Contains(code)))
            .ToArray();
    }

    public async Task DeleteAsync(
        string itemCode,
        IReadOnlyList<string> materialCodesToDelete,
        CancellationToken cancellationToken = default)
    {
        // Bom（削除対象品番の明細）と Item は常に削除する。
        var bomRecords = await _bomRepository.GetByItemCodeAsync(itemCode, cancellationToken);
        _bomRepository.RemoveRange(bomRecords);

        var itemRecord = await _itemRepository.FindAsync(itemCode, cancellationToken);
        if (itemRecord is not null)
        {
            _itemRepository.Remove(itemRecord);
        }

        // Material は指定された部品品番（他品番で未使用、かつユーザーが YES と回答したもの）のみ削除する。
        if (materialCodesToDelete.Count > 0)
        {
            var materialRecords = await _materialRepository.FindManyAsync(materialCodesToDelete, cancellationToken);
            _materialRepository.RemoveRange(materialRecords);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
