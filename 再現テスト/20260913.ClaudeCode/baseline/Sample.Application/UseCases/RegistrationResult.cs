using Sample.Domain.Validation;

namespace Sample.Application.UseCases;

/// <summary>
/// <see cref="RegisterPartsListUseCase"/> の実行結果。
/// </summary>
public sealed record RegistrationResult(bool Success, IReadOnlyList<ValidationError> Errors)
{
    public static RegistrationResult Ok() => new(true, Array.Empty<ValidationError>());

    public static RegistrationResult Fail(IReadOnlyList<ValidationError> errors) => new(false, errors);
}
