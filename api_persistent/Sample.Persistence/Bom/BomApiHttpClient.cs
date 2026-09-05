// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// BomApiHttpClient.cs
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

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Sample.Application.Abstractions;   // WriteOutcome
using Sample.Persistence.Dtos;
 
namespace Sample.Persistence;
 
/// <summary>
/// 差し替え版 <see cref="IBomDataAccess"/> 実装(結果型版)。
///
/// EnsureSuccessStatusCode() で一律に例外化するのをやめ、
/// 409 Conflict(想定内の業務衝突)を <see cref="WriteOutcome.Conflict"/> へ翻訳して“返す”のが RegisterAsync。
/// それ以外の失敗(400/401/5xx/ネットワーク=想定外)は、従来どおり例外で境界へ伝播させる。
///
/// 読み取り系・削除系は「想定内の業務エラー」を持たないため、失敗=例外のまま。
/// </summary>
public sealed class BomApiHttpClient(HttpClient http) : IBomDataAccess
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
 
    // ── 読み ──
    public async Task<IReadOnlyList<BomRowDto>> GetBomListAsync(CancellationToken ct = default)
    {
        var rows = await http.GetFromJsonAsync<List<BomRowDto>>("api/bom/rows", Json, ct);
        return rows ?? [];
    }
 
    // ── 登録(結果型版) ─────────────────────────────────────────────
    public async Task<WriteOutcome> RegisterAsync(ItemRegistrationDto registration, CancellationToken ct = default)
    {
        using var res = await http.PostAsJsonAsync("api/bom/items", registration, Json, ct);
 
        if (res.IsSuccessStatusCode)
            return new WriteOutcome.Success();
 
        // 想定内の業務衝突(重複PK等)だけを結果型へ翻訳する。
        if (res.StatusCode == HttpStatusCode.Conflict)
        {
            var message = await ReadProblemDetailAsync(res, ct)
                ?? "同じ品番が既に登録されています。";   // ★要確認: サーバーが本文を返さない場合の既定文言
            return new WriteOutcome.Conflict(message);
        }
 
        // それ以外は“想定外”。握りつぶさず例外で境界(→ 500 相当)へ。
        res.EnsureSuccessStatusCode();
        return new WriteOutcome.Success();   // 到達不能。直前で必ず throw される(コンパイラ用)。
    }
 
    // ── 一括カスケード削除(失敗=例外) ──
    public async Task DeleteItemsAsync(IEnumerable<string> itemCodes, CancellationToken ct = default)
    {
        var body = new DeleteItemsBody(itemCodes.ToArray());
        using var res = await http.PostAsJsonAsync("api/bom/items/delete", body, Json, ct);
        res.EnsureSuccessStatusCode();
    }
 
    // ── 削除フローの判定材料(読み) ──
    public async Task<IReadOnlyList<string>> GetMaterialCodesOfItemAsync(
        string itemCode, CancellationToken ct = default)
    {
        var codes = await http.GetFromJsonAsync<List<string>>(
            $"api/bom/items/{Uri.EscapeDataString(itemCode)}/material-codes", Json, ct);
        return codes ?? [];
    }
 
    public async Task<IReadOnlyList<string>> GetOrphanMaterialCodesAsync(
        string itemCode, CancellationToken ct = default)
    {
        var codes = await http.GetFromJsonAsync<List<string>>(
            $"api/bom/items/{Uri.EscapeDataString(itemCode)}/orphan-material-codes", Json, ct);
        return codes ?? [];
    }
 
    // ── 削除の2経路(失敗=例外) ──
    public async Task DeleteItemKeepMaterialsAsync(string itemCode, CancellationToken ct = default)
    {
        using var res = await http.PostAsync(
            $"api/bom/items/{Uri.EscapeDataString(itemCode)}/delete", content: null, ct);
        res.EnsureSuccessStatusCode();
    }
 
    public async Task DeleteItemWithMaterialsAsync(
        string itemCode, IEnumerable<string> materialCodes, CancellationToken ct = default)
    {
        var body = new DeleteWithMaterialsBody(materialCodes.ToArray());
        using var res = await http.PostAsJsonAsync(
            $"api/bom/items/{Uri.EscapeDataString(itemCode)}/delete-with-materials", body, Json, ct);
        res.EnsureSuccessStatusCode();
    }
 
    // ── ProblemDetails から画面用文言を取り出す(失敗しても null で握る) ──
    private static async Task<string?> ReadProblemDetailAsync(HttpResponseMessage res, CancellationToken ct)
    {
        try
        {
            var pd = await res.Content.ReadFromJsonAsync<ProblemDetail>(Json, ct);
            return pd?.Detail ?? pd?.Title;
        }
        catch
        {
            return null;   // ProblemDetails でない/空 → 既定文言にフォールバック
        }
    }
 
    // RFC 7807 ProblemDetails の必要フィールドだけ受ける最小型。
    private sealed record ProblemDetail(string? Title, string? Detail, int? Status);
 
    // 送信専用の小さな本文型。
    private sealed record DeleteItemsBody(IReadOnlyList<string> ItemCodes);
    private sealed record DeleteWithMaterialsBody(IReadOnlyList<string> MaterialCodes);
}
