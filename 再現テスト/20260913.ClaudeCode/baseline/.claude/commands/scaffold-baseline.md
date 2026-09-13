---
description: Clean Architecture 4層の空ソリューションを生成し、参照グラフを内向きに配線する（足場のみ）
argument-hint: [solution-name]
allowed-tools: Bash(dotnet:*), Bash(rm:*)
---

# baseline スキャフォールド

Clean Architecture 4層（Domain / Application / Persistence / Web）の **空の足場** を作る。
業務ロジックは一切書かないこと。目的は「ビルドが通る正しい構造」だけを用意すること。

## 前提

- 以下のコマンド中の `Sample` は既定のプレフィックス。`$1` が渡された場合は、このファイル中のすべての `Sample` をその名前に読み替えて実行する。
- カレントディレクトリ（リポジトリ直下）で実行する。
- .NET 8（`net8.0`）を対象にする。

## 参照グラフ（確定した設計判断。勝手に変えないこと）

内向き・非循環。次の4本だけを張る：

- Application → Domain
- Persistence → Application, Domain
- Web → Application, Persistence
- Domain → （参照ゼロ。何も張らない）

補足：
- `Web → Domain` は **明示的に張らない**（Application / Persistence 経由で推移的に見えるため）。
- `Domain` は参照ゼロを維持する（EF Core など永続の詳細を漏らさないため）。

## 手順

1. ソリューション作成
   - `dotnet new sln -n Sample`

2. 4プロジェクト作成
   - `dotnet new classlib -n Sample.Domain      -f net8.0`
   - `dotnet new classlib -n Sample.Application -f net8.0`
   - `dotnet new classlib -n Sample.Persistence -f net8.0`
   - `dotnet new blazor   -n Sample.Web         -f net8.0 --interactivity Server`

3. classlib が生成する既定の `Class1.cs`（Domain / Application / Persistence の3つ）を削除する。

4. ソリューションに4プロジェクトを登録する
   - `dotnet sln Sample.sln add Sample.Domain/Sample.Domain.csproj Sample.Application/Sample.Application.csproj Sample.Persistence/Sample.Persistence.csproj Sample.Web/Sample.Web.csproj`

5. 参照を「参照グラフ」どおりに張る
   - `dotnet add Sample.Application/Sample.Application.csproj reference Sample.Domain/Sample.Domain.csproj`
   - `dotnet add Sample.Persistence/Sample.Persistence.csproj reference Sample.Application/Sample.Application.csproj Sample.Domain/Sample.Domain.csproj`
   - `dotnet add Sample.Web/Sample.Web.csproj reference Sample.Application/Sample.Application.csproj Sample.Persistence/Sample.Persistence.csproj`

## 検証（ここまでやって完了とする）

1. `dotnet build` が成功すること（循環参照がない証拠）。
2. 各プロジェクトの宣言参照を `dotnet list <csproj> reference` で表示し、グラフと一致するか確認する。特に：
   - `Sample.Domain` は参照ゼロ
   - `Sample.Web` は Application と Persistence のみ（Domain は含まない）
3. 反証テスト（規則＝物理的禁止が効いている証拠）：
   - `dotnet add Sample.Domain/Sample.Domain.csproj reference Sample.Application/Sample.Application.csproj` を試みる。
   - **循環参照エラーで拒否される** ことを確認する。これが確認できれば「Domain → 外側は物理的に不可能」の実証になる。
   - エラーになれば参照は追加されない。万一追加されていた場合は必ず取り消し、`Domain` を参照ゼロに戻すこと。

## 報告

- 生成したプロジェクトと張った参照を一覧で示す。
- `dotnet build` と `dotnet list ... reference` の結果を要約する。
- 業務ロジックは書いていないことを明記し、次のステップ（ドメインモデルの実装など）は指示を待つこと。