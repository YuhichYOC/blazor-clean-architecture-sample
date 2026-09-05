// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// Material.cs
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

namespace Sample.Persistence.Entities;

/// <summary>部品マスタ (Material)。item_code が部品品番を表す。</summary>
public class Material
{
    /// <summary>部品品番。</summary>
    public string ItemCode { get; set; } = default!;

    /// <summary>部品品名。</summary>
    public string ItemName { get; set; } = default!;

    /// <summary>この部品を使用している部品構成表レコード。</summary>
    public ICollection<Bom> Boms { get; set; } = new List<Bom>();
}
