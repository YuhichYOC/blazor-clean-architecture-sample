// RegisterBomUseCase.cs
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
using Sample.Domain.Repository;
using Sample.Domain.Validation;
using Sample.Application.Validation;

namespace Sample.Application.UseCases.RegisterBom;

public sealed class RegisterBomUseCase : IRegisterBomUseCase
{
    private readonly IItemRepository _repository;
    private readonly IRegisterItemValidator _domainValidator;
    private readonly IRegisterBomRequestValidator _requestValidator;

    public RegisterBomUseCase(
        IItemRepository repository,
        IRegisterItemValidator domainValidator,
        IRegisterBomRequestValidator requestValidator)
    {
        _repository = repository;
        _domainValidator = domainValidator;
        _requestValidator = requestValidator;
    }

    public async Task<RegisterBomResponse> ExecuteAsync(
        RegisterBomRequest request)
    {
        List<ValidationError> errors =
        [
            .._requestValidator.Validate(request)
        ];

        if (errors.Count > 0)
        {
            return new RegisterBomResponse
            {
                Success = false,
                Errors = errors.AsReadOnly()
            };
        }

        RegisterItemModel model = new(
            request.ItemCode,
            request.ItemName,
            request.Materials
                .Select(x =>
                    new RegisterMaterialModel(
                        x.MaterialCode,
                        x.MaterialName,
                        x.Requirement))
                .ToList());

        errors.AddRange(_domainValidator.Validate(model));

        if (errors.Count > 0)
        {
            return new RegisterBomResponse
            {
                Success = false,
                Errors = errors.AsReadOnly()
            };
        }

        await _repository.RegisterAsync(model);

        return new RegisterBomResponse
        {
            Success = true,
            Errors = Array.Empty<ValidationError>()
        };
    }
}