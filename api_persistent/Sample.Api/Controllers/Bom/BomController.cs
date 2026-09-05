// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// BomController.cs
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

using Sample.Api.Contracts.Bom;
using Sample.Api.Data.Bom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sample.Api.Controllers.Bom;

/// <summary>
/// DBアクセスAPIの公開面。Sample.Persistence.IBomDataAccess の各メソッドと 1:1 対応する。
///
/// 設計方針:
///   読み取り = GET、変更 = POST に寄せた RPC 風。純 REST リソース設計ではなく、
///   「粗粒度の継ぎ目(=各メソッドが1トランザクション)」をそのまま写すことを優先している。
///   これにより HTTP をまたぐトランザクションを作らずに済む(境界はすべてサーバー側)。
/// </summary>
[ApiController]
[Route("api/bom")]
public sealed class BomController(BomDataService data) : ControllerBase
{
    // GET /api/bom/rows
    [HttpGet("rows")]
    public async Task<IReadOnlyList<BomRowResponse>> GetRows(CancellationToken ct)
        => await data.GetBomListAsync(ct);
 
    // POST /api/bom/items  (登録:サーバー側で1トランザクション)
    [HttpPost("items")]
    public async Task<IActionResult> Register([FromBody] ItemRegistrationRequest req, CancellationToken ct)
    {
        try
        {
            await data.RegisterAsync(req, ct);
            return NoContent();
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            return Problem(
                title: "duplicate_item",
                detail: $"品番 '{req.ItemCode}' は既に登録されています。",
                statusCode: StatusCodes.Status409Conflict);
        }
    }
 
    // POST /api/bom/items/delete  (温存:一括カスケード削除)
    [HttpPost("items/delete")]
    public async Task<IActionResult> DeleteItems([FromBody] DeleteItemsRequest req, CancellationToken ct)
    {
        await data.DeleteItemsAsync(req.ItemCodes, ct);
        return NoContent();
    }
 
    // GET /api/bom/items/{itemCode}/material-codes
    [HttpGet("items/{itemCode}/material-codes")]
    public async Task<IReadOnlyList<string>> GetMaterialCodes(string itemCode, CancellationToken ct)
        => await data.GetMaterialCodesOfItemAsync(itemCode, ct);
 
    // GET /api/bom/items/{itemCode}/orphan-material-codes
    [HttpGet("items/{itemCode}/orphan-material-codes")]
    public async Task<IReadOnlyList<string>> GetOrphanCodes(string itemCode, CancellationToken ct)
        => await data.GetOrphanMaterialCodesAsync(itemCode, ct);
 
    // POST /api/bom/items/{itemCode}/delete  (部品マスタ温存)
    [HttpPost("items/{itemCode}/delete")]
    public async Task<IActionResult> DeleteKeepMaterials(string itemCode, CancellationToken ct)
    {
        await data.DeleteItemKeepMaterialsAsync(itemCode, ct);
        return NoContent();
    }
 
    // POST /api/bom/items/{itemCode}/delete-with-materials  (孤児部品も削除)
    [HttpPost("items/{itemCode}/delete-with-materials")]
    public async Task<IActionResult> DeleteWithMaterials(
        string itemCode, [FromBody] DeleteWithMaterialsRequest req, CancellationToken ct)
    {
        await data.DeleteItemWithMaterialsAsync(itemCode, req.MaterialCodes, ct);
        return NoContent();
    }

    // Oracle: ORA-00001 = 一意制約違反
    private static bool IsUniqueViolation(DbUpdateException ex)
        => ex.InnerException is Oracle.ManagedDataAccess.Client.OracleException { Number: 1 };
    // ★要確認: Number 判定は実機で要確認。ORA番号での判定が確実。
}
