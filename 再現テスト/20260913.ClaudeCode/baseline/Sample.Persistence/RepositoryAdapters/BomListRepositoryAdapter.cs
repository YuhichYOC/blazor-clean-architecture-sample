using Sample.Application.RepositoryAdapters;
using Sample.Domain.Dtos;
using Sample.Persistence.Repositories;

namespace Sample.Persistence.RepositoryAdapters;

/// <summary>
/// <see cref="IBomListRepositoryAdapter"/> の実装。
/// </summary>
public sealed class BomListRepositoryAdapter : IBomListRepositoryAdapter
{
    private readonly BomRepository _bomRepository;

    public BomListRepositoryAdapter(BomRepository bomRepository)
    {
        _bomRepository = bomRepository;
    }

    public async Task<IReadOnlyList<BomListItemDto>> GetListAsync(CancellationToken cancellationToken = default)
        => await _bomRepository.GetListAsync(cancellationToken);
}
