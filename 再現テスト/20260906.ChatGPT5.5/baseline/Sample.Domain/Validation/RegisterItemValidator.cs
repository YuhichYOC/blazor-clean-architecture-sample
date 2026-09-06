// RegisterItemValidator.cs
// 
// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Sample.Domain.Models;

namespace Sample.Domain.Validation;

public sealed class RegisterItemValidator : IRegisterItemValidator
{
    public IReadOnlyList<ValidationError> Validate(RegisterItemModel model)
    {
        List<ValidationError> errors = [];

        if (!model.Materials.Any())
        {
            errors.Add(
                new ValidationError(
                    "ERR001",
                    "構成部品がありません"));
        }

        foreach (var material in model.Materials)
        {
            if (material.Requirement <= 0)
            {
                errors.Add(
                    new ValidationError(
                        "ERR002",
                        "所要量は0より大きくしてください"));
            }
        }

        var duplicatedCodes =
            model.Materials
                .GroupBy(x => x.MaterialCode)
                .Where(x => x.Count() > 1);

        if (duplicatedCodes.Any())
        {
            errors.Add(
                new ValidationError(
                    "ERR003",
                    "部品品番が重複しています"));
        }
        
        return errors.AsReadOnly();
    }
}