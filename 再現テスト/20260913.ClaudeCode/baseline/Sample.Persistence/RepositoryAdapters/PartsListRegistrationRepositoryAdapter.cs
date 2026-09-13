using Sample.Application.RepositoryAdapters;
using Sample.Domain.Entities;
using Sample.Persistence.Records;
using Sample.Persistence.Repositories;

namespace Sample.Persistence.RepositoryAdapters;

/// <summary>
/// <see cref="IPartsListRegistrationRepositoryAdapter"/> の実装。
/// Item・Material・Bom への書き込みを change tracking & SaveChanges による
/// 単一トランザクションで行う（docs/design/画面イメージ-追加ボタン・新規登録.png 参照）。
/// </summary>
public sealed class PartsListRegistrationRepositoryAdapter : IPartsListRegistrationRepositoryAdapter
{
    private readonly SampleDbContext _context;
    private readonly ItemRepository _itemRepository;
    private readonly MaterialRepository _materialRepository;
    private readonly BomRepository _bomRepository;

    public PartsListRegistrationRepositoryAdapter(
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

    public async Task RegisterAsync(PartsListRegistration registration, CancellationToken cancellationToken = default)
    {
        _itemRepository.Add(new ItemRecord
        {
            ItemCode = registration.ItemCode,
            ItemName = registration.ItemName,
        });

        var distinctParts = registration.ConstituentParts
            .DistinctBy(part => part.MaterialCode)
            .ToList();

        var existingMaterialCodes = (await _materialRepository.FindManyAsync(
                distinctParts.Select(part => part.MaterialCode).ToArray(),
                cancellationToken))
            .Select(material => material.ItemCode)
            .ToHashSet();

        // 部品マスタは他品番と共有され得るため、既に登録済みの部品品番は追加しない。
        foreach (var part in distinctParts.Where(part => !existingMaterialCodes.Contains(part.MaterialCode)))
        {
            _materialRepository.Add(new MaterialRecord
            {
                ItemCode = part.MaterialCode,
                ItemName = part.MaterialName,
            });
        }

        _bomRepository.AddRange(registration.ConstituentParts.Select(part => new BomRecord
        {
            ItemCode = registration.ItemCode,
            MItemCode = part.MaterialCode,
            Requirement = part.Requirement,
        }));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
