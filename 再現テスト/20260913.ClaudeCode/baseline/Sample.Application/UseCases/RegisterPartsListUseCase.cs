using Sample.Application.RepositoryAdapters;
using Sample.Domain.Entities;
using Sample.Domain.Validation;

namespace Sample.Application.UseCases;

/// <summary>
/// 部品構成表を新規登録するユースケース
/// （docs/design/画面イメージ-追加ボタン・新規登録.png、エラー入力1〜7 参照）。
/// </summary>
public sealed class RegisterPartsListUseCase
{
    private readonly IPartsListRegistrationRepositoryAdapter _registrationRepositoryAdapter;

    public RegisterPartsListUseCase(IPartsListRegistrationRepositoryAdapter registrationRepositoryAdapter)
    {
        _registrationRepositoryAdapter = registrationRepositoryAdapter;
    }

    public async Task<RegistrationResult> ExecuteAsync(
        PartsListRegistration registration,
        CancellationToken cancellationToken = default)
    {
        var errors = PartsListRegistrationValidator.Validate(registration);
        if (errors.Count > 0)
        {
            return RegistrationResult.Fail(errors);
        }

        await _registrationRepositoryAdapter.RegisterAsync(registration, cancellationToken);
        return RegistrationResult.Ok();
    }
}
