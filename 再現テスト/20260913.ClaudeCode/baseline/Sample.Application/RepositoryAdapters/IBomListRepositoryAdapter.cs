using Sample.Domain.Dtos;

namespace Sample.Application.RepositoryAdapters;

/// <summary>
/// 部品構成表登録画面の一覧表示データを取得するリポジトリアダプター。
/// 実装はパーシステント層（Item / Bom / Material の結合クエリ）で行う。
/// </summary>
public interface IBomListRepositoryAdapter
{
    /// <summary>
    /// 一覧表示用データを取得する（Bom を基準に Item・Material を結合、
    /// 品番→部品品番の順でソート）。
    /// </summary>
    Task<IReadOnlyList<BomListItemDto>> GetListAsync(CancellationToken cancellationToken = default);
}
