// ItemRepositoryAdapter.cs
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

using Sample.Domain.DTO;
using Sample.Domain.Models;
using Sample.Domain.Repository;
using Sample.Persistence.Repository;

namespace Sample.Persistence.Adapter;

public sealed class ItemRepositoryAdapter : IItemRepository
{
    private readonly ItemRepository _repository;

    public ItemRepositoryAdapter(ItemRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<ItemBomDto>> GetListAsync()
        => _repository.GetListAsync();

    public Task RegisterAsync(RegisterItemModel model)
        => _repository.RegisterAsync(model);

    public Task<DeleteBomPreCheckResult> CheckDeleteTargetAsync(
        string itemCode)
        => _repository.CheckDeleteTargetAsync(itemCode);

    public Task ExecuteDeleteAsync(
        string itemCode,
        bool deleteMaterials)
        => _repository.ExecuteDeleteAsync(
            itemCode,
            deleteMaterials);
}