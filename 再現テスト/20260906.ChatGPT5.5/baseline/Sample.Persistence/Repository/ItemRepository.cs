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

using Microsoft.EntityFrameworkCore;
using Sample.Domain.DTO;
using Sample.Domain.Models;
using Sample.Persistence.Context;
using Sample.Persistence.Records;

namespace Sample.Persistence.Repository;

public sealed class ItemRepository
{
    private readonly OracleContext _context;

    public ItemRepository(
        OracleContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ItemBomDto>> GetListAsync()
    {
        return await _context.Boms
            .AsNoTracking()
            .OrderBy(x => x.ItemCode)
            .ThenBy(x => x.MaterialCode)
            .Select(x => new ItemBomDto
            {
                ItemCode = x.ItemCode,
                ItemName = x.Item.ItemName,
                MaterialCode = x.MaterialCode,
                MaterialName = x.Material.MaterialName,
                Requirement = x.Requirement
            })
            .ToListAsync();
    }

    public async Task RegisterAsync(
        RegisterItemModel model)
    {
        ItemRecord? item =
            await _context.Items
                .SingleOrDefaultAsync(x =>
                    x.ItemCode == model.ItemCode);

        if (item is null)
        {
            item = new ItemRecord
            {
                ItemCode = model.ItemCode,
                ItemName = model.ItemName
            };

            _context.Items.Add(item);
        }

        foreach (RegisterMaterialModel material
                 in model.Materials)
        {
            MaterialRecord? materialRecord =
                await _context.Materials
                    .SingleOrDefaultAsync(x =>
                        x.MaterialCode == material.MaterialCode);

            if (materialRecord is null)
            {
                materialRecord = new MaterialRecord
                {
                    MaterialCode = material.MaterialCode,
                    MaterialName = material.MaterialName
                };

                _context.Materials.Add(materialRecord);
            }

            _context.Boms.Add(
                new BomRecord
                {
                    ItemCode = model.ItemCode,
                    MaterialCode = material.MaterialCode,
                    Requirement = material.Requirement
                });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<DeleteBomPreCheckResult> CheckDeleteTargetAsync(string itemCode)
    {
        var materials =
            await _context.Boms
                .Where(x => x.ItemCode == itemCode)
                .Select(x => x.MaterialCode)
                .ToListAsync();
        
        var usedByOthers =
            await _context.Boms
                .Where(x =>
                    x.ItemCode != itemCode &&
                    materials.Contains(x.MaterialCode))
                .Select(x => x.MaterialCode)
                .Distinct()
                .ToListAsync();
        
        bool needConfirm =
            materials.Except(usedByOthers).Any();
        
        return new DeleteBomPreCheckResult
        {
            NeedConfirm = needConfirm,
            MaterialCodes = materials
        };
    }

    public async Task ExecuteDeleteAsync(string itemCode, bool deleteMaterials)
    {
        List<BomRecord> boms =
            await _context.Boms
                .Where(x => x.ItemCode == itemCode)
                .ToListAsync();
        
        var materialCodes =
            boms.Select(x => x.MaterialCode)
                .ToList();
        
        _context.Boms.RemoveRange(boms);
        
        ItemRecord item =
            await _context.Items
                .SingleAsync(x => x.ItemCode == itemCode);

        _context.Items.Remove(item);
        
        if (deleteMaterials)
        {
            var materials =
                await _context.Materials
                    .Where(x =>
                        materialCodes.Contains(
                            x.MaterialCode))
                    .ToListAsync();

            _context.Materials.RemoveRange(materials);
        }
        
        await _context.SaveChangesAsync();
    }
}