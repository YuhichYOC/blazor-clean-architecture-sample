// ItemRepository.cs
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

using Microsoft.EntityFrameworkCore;
using Sample.Application.UseCases.DeleteBomExecute;
using Sample.Application.UseCases.DeleteBomPreCheck;
using Sample.Application.UseCases.RegisterBom;
using Sample.Domain.DTO;
using Sample.Domain.Models;
using Sample.Persistence.Context;
using Sample.Persistence.Records;

namespace Sample.Persistence.Repository;

public sealed class ItemRepository(OracleContext _context)
{
    public async Task<IReadOnlyList<ItemBomDto>> GetListAsync()
    {
        return await _context.Boms
            .AsNoTracking()
            .Include(x => x.Item)
            .Include(x => x.Material)
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

    public async Task RegisterAsync(RegisterItemModel model)
    {
        ItemRecord? item = await _context.Items.FindAsync(model.ItemCode);

        if (item is null)
        {
            item = new ItemRecord
            {
                ItemCode = model.ItemCode,
                ItemName = model.ItemName
            };

            _context.Items.Add(item);
        }
        
        foreach (var material in model.Materials)
        {
            MaterialRecord? record =
                await _context.Materials.FindAsync(
                    material.MaterialCode);

            if (record is null)
            {
                record = new MaterialRecord
                {
                    MaterialCode = material.MaterialCode,
                    MaterialName = material.MaterialName
                };

                _context.Materials.Add(record);
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
        var materialCodes =
            await _context.Boms
                .Where(x => x.ItemCode == itemCode)
                .Select(x => x.MaterialCode)
                .ToListAsync();
        
        bool needConfirm = false;
        
        foreach (string materialCode in materialCodes)
        {
            bool usedByOther =
                await _context.Boms
                    .AnyAsync(x =>
                        x.MaterialCode == materialCode &&
                        x.ItemCode != itemCode);

            if (!usedByOther)
            {
                needConfirm = true;
                break;
            }
        }
        
        return new DeleteBomPreCheckResult
        {
            NeedConfirm = needConfirm,
            MaterialCodes = materialCodes
        };
    }

    public async Task ExecuteDeleteAsync(string itemCode, bool deleteMaterials)
    {
        List<BomRecord> boms =
            await _context.Boms
                .Where(x => x.ItemCode == itemCode)
                .ToListAsync();
        
        _context.Boms.RemoveRange(boms);
        
        ItemRecord item =
            await _context.Items.FindAsync(itemCode);

        _context.Items.Remove(item);
        
        if (deleteMaterials)
        {
            foreach (BomRecord bom in boms)
            {
                MaterialRecord material =
                    await _context.Materials.FindAsync(
                        bom.MaterialCode);

                _context.Materials.Remove(material);
            }
        }
        
        await _context.SaveChangesAsync();
    }
}