using Sample.Application.RepositoryAdapters;
using Sample.Domain.Dtos;

namespace Sample.Application.UseCases;

/// <summary>
/// 部品構成表登録画面の初期表示・再読み込み時に一覧データを取得するユースケース
/// （docs/design/画面イメージ-画面のロード時.png 参照）。
/// </summary>
public sealed class GetBomListUseCase
{
    private readonly IBomListRepositoryAdapter _bomListRepositoryAdapter;

    public GetBomListUseCase(IBomListRepositoryAdapter bomListRepositoryAdapter)
    {
        _bomListRepositoryAdapter = bomListRepositoryAdapter;
    }

    public Task<IReadOnlyList<BomListItemDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        => _bomListRepositoryAdapter.GetListAsync(cancellationToken);
}
