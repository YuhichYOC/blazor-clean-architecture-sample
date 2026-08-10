namespace Sample.Persistence.Dtos;

/// <summary>追加モーダルの入力内容(品番1件＋その構成部品)。</summary>
public record ItemRegistrationDto(
    string ItemCode,                            // 品番
    string ItemName,                            // 品名
    IReadOnlyList<ComponentDto> Components      // 構成部品
);

/// <summary>構成部品1行分。</summary>
public record ComponentDto(
    string MItemCode,       // 部品品番
    string MItemName,       // 部品品名
    decimal Requirement     // 所要量
);
