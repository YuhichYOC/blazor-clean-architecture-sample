// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// Item.cs
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

namespace Sample.Api.Entities;

/// <summary>品番マスタ (Item)。</summary>
public class Item
{
    /// <summary>品番。</summary>
    public string ItemCode { get; set; } = default!;

    /// <summary>品名。</summary>
    public string ItemName { get; set; } = default!;

    /// <summary>この品番を構成する部品構成表レコード。</summary>
    public ICollection<Bom> Boms { get; set; } = new List<Bom>();
}
