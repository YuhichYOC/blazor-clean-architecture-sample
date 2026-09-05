// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// ComponentLine.cs
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

namespace Sample.Domain;

/// <summary>
/// 構成部品(部品構成表の1行)。ある品番を構成する部品への参照と、その所要量。
/// Item 集約の内部要素であり、単独では存在しない。
///
/// 部品品名(MaterialName)は、登録時に部品マスタ(Material)へ書き込むために保持する。
/// 名称そのものは業務規則ではないが「空でないこと」は要求する。
/// (Material を独立した集約と見るなら、本来は MaterialCode の参照だけを持ち、
///  名称は Material 側の責務。ここでは登録ユースケースの都合で名称も同伴させている。
///  「Material は所有か共有か」の判断は保留中で、この同伴は暫定的な割り切り。)
/// </summary>
public sealed class ComponentLine
{
    public string MaterialCode { get; }   // 部品品番 (Material.item_code / Bom.m_item_code)
    public string MaterialName { get; }   // 部品品名 (Material.item_name)
    public Requirement Requirement { get; }

    public ComponentLine(string materialCode, string materialName, Requirement requirement)
    {
        if (string.IsNullOrWhiteSpace(materialCode))
            throw new DomainException("部品品番は必須です。");
        if (string.IsNullOrWhiteSpace(materialName))
            throw new DomainException("部品品名は必須です。");

        MaterialCode = materialCode;
        MaterialName = materialName;
        Requirement = requirement;
    }
}
