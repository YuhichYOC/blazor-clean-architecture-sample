using Sample.Domain.Entities;

namespace Sample.Application.RepositoryAdapters;

/// <summary>
/// 部品構成表の新規登録を行うリポジトリアダプター。
/// 実装はパーシステント層で Item・Material・Bom への挿入を
/// change tracking & SaveChanges による単一トランザクションで行う。
/// </summary>
public interface IPartsListRegistrationRepositoryAdapter
{
    /// <summary>
    /// 登録内容を Item・Material・Bom へ書き込む。
    /// 呼び出し側で <see cref="Sample.Domain.Validation.PartsListRegistrationValidator"/> による
    /// 検証を通過していることを前提とする。
    /// </summary>
    Task RegisterAsync(PartsListRegistration registration, CancellationToken cancellationToken = default);
}
