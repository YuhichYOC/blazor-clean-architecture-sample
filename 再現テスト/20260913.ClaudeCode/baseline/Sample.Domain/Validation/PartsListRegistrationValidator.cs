using Sample.Domain.Entities;

namespace Sample.Domain.Validation;

/// <summary>
/// 部品構成表の新規登録内容を検証する。
/// エラー入力1〜7（docs/design/画面イメージ-エラー入力1〜7.png）に対応：
///   1. 品番が未入力
///   2. 品名が未入力
///   3. 部品品番が未入力
///   4. 部品品名が未入力
///   5. 所要量が0以下
///   6. 構成部品が1件も無い
///   7. 構成部品内で部品品番が重複している
/// </summary>
public static class PartsListRegistrationValidator
{
    public static IReadOnlyList<ValidationError> Validate(PartsListRegistration registration)
    {
        var errors = new List<ValidationError>();

        // エラー入力1：品番が未入力
        if (string.IsNullOrWhiteSpace(registration.ItemCode))
        {
            errors.Add(new ValidationError(ValidationFields.ItemCode, "品番を入力してください。"));
        }

        // エラー入力2：品名が未入力
        if (string.IsNullOrWhiteSpace(registration.ItemName))
        {
            errors.Add(new ValidationError(ValidationFields.ItemName, "品名を入力してください。"));
        }

        // エラー入力6：構成部品が1件も無い
        if (registration.ConstituentParts.Count == 0)
        {
            errors.Add(new ValidationError(ValidationFields.ConstituentParts, "構成部品を1件以上入力してください。"));
            return errors;
        }

        for (var i = 0; i < registration.ConstituentParts.Count; i++)
        {
            var part = registration.ConstituentParts[i];

            // エラー入力3：部品品番が未入力
            if (string.IsNullOrWhiteSpace(part.MaterialCode))
            {
                errors.Add(new ValidationError(ValidationFields.MaterialCode, "部品品番を入力してください。", i));
            }

            // エラー入力4：部品品名が未入力
            if (string.IsNullOrWhiteSpace(part.MaterialName))
            {
                errors.Add(new ValidationError(ValidationFields.MaterialName, "部品品名を入力してください。", i));
            }

            // エラー入力5：所要量が0以下
            if (part.Requirement <= 0)
            {
                errors.Add(new ValidationError(ValidationFields.Requirement, "所要量は1以上を入力してください。", i));
            }
        }

        // エラー入力7：構成部品内で部品品番が重複している
        var duplicateIndexes = registration.ConstituentParts
            .Select((part, index) => (part.MaterialCode, index))
            .Where(x => !string.IsNullOrWhiteSpace(x.MaterialCode))
            .GroupBy(x => x.MaterialCode)
            .Where(group => group.Count() > 1)
            .SelectMany(group => group.Select(x => x.index));

        foreach (var index in duplicateIndexes)
        {
            errors.Add(new ValidationError(ValidationFields.MaterialCode, "部品品番が重複しています。", index));
        }

        return errors;
    }
}
