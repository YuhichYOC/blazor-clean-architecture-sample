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

using Sample.Domain.Repository;

namespace Sample.Application.UseCases.DeleteBomPreCheck;

public sealed class DeleteBomPreCheckUseCase
    : IDeleteBomPreCheckUseCase
{
    private readonly IItemRepository _repository;

    public DeleteBomPreCheckUseCase(
        IItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeleteBomPreCheckResponse> ExecuteAsync(
        DeleteBomPreCheckRequest request)
    {
        var result =
            await _repository.CheckDeleteTargetAsync(
                request.ItemCode);

        return new DeleteBomPreCheckResponse
        {
            NeedConfirm = result.NeedConfirm,
            MaterialCodes = result.MaterialCodes
        };
    }
}