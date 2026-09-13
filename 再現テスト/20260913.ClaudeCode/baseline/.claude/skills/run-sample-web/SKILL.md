---
name: run-sample-web
description: Sample.Web（Blazor Web App）をOracle Database（Docker, oracle:1521/FREEPDB1）に接続した状態で起動し、部品構成表登録画面の動作を確認する。「動かして確認して」「runして」等、このアプリを起動・動作確認したいときに使う。
---

# Sample.Web 起動・動作確認

このアプリは ASP.NET Core（Sample.Web）+ ORACLE Database の構成（CLAUDE.md 参照）。
Docker上のOracleコンテナと同じ `bom-net` ネットワークに、このdevcontainer自身も
`--network=bom-net` で参加しているため、`dotnet run` するだけで直接Oracleへ到達できる
（別途 `docker compose up` 等は不要）。

## 前提

- Oracleコンテナ：ホスト名 `oracle`、ポート `1521`、サービス名（PDB）`FREEPDB1`
- 接続文字列は `Sample.Web/appsettings.Development.json` の
  `ConnectionStrings:OracleDb` に設定済み（User Id=DEV_USER、パスワードは同ファイル参照。
  このスキルファイルには書かない）。接続情報が変わった場合はそのファイルを更新すること。
- テーブル（Item / Material / Bom）は EF Core Migrations で作成済み
  （`Sample.Persistence/Migrations/`）。未作成環境では後述の「テーブル作成」を先に実行する。

## 起動手順

1. ビルド

   ```bash
   cd /workspaces/src
   dotnet build
   ```

2. ポート 5299 が空いているか確認する。このdevcontainerには `lsof` / `ss` / `netstat`
   が入っていないため、`/proc/net/tcp` を直接読む（5299 = 16進 `14B3`）。

   ```bash
   grep -i ":14B3" /proc/net/tcp && echo "使用中" || echo "空いている"
   ```

   使用中だった場合、掴んでいるプロセスを特定して止める（前回の起動が残っていることが多い）。

   ```bash
   # 上の行の最後のカラム（inode）を使って、そのソケットを開いている PID を探す
   inode=<grepで出たinode>
   for pid in /proc/[0-9]*; do
     ls -l "$pid/fd" 2>/dev/null | grep -q "$inode" && echo "$(basename "$pid"): $(cat "$pid/cmdline" 2>/dev/null | tr '\0' ' ')"
   done
   kill -9 <上で見つかったPID>
   ```

3. `Development` 環境でバックグラウンド起動し、実際に応答するまで待つ

   ```bash
   cd /workspaces/src/Sample.Web
   ASPNETCORE_ENVIRONMENT=Development nohup dotnet run --no-build --urls http://localhost:5299 > /tmp/web-run.log 2>&1 &
   timeout 30 bash -c 'until curl -s -o /dev/null -w "%{http_code}" http://localhost:5299/ | grep -qE "^[0-9]+$"; do sleep 1; done'
   ```

   `dotnet run &` の `$!` は `dotnet` プロセス自体のPIDなので、後で止めるときは
   手順2と同じ方法（`/proc/net/tcp` → inode → `/proc/*/fd`）でポートの実際の
   保持プロセスを特定すること。

4. 起動確認

   ```bash
   curl -s -o /tmp/resp.html -w "HTTP %{http_code}\n" http://localhost:5299/
   grep -iE "error|exception" /tmp/web-run.log
   ```

   `HTTP 200` かつログにエラーが無ければ起動成功。`ORA-12154` 等のOracle接続エラーが
   出た場合は `appsettings.Development.json` の接続文字列とOracleコンテナの稼働状況を疑う。

## 動作確認（駆動して確認する）

このdevcontainerには `chromium-cli` も Node.js / Playwright も入っていない
（.NET SDKのみの最小構成）ため、ブラウザスクリーンショットではなく、
サーバーサイドレンダリングされたHTMLを直接検査して確認する。

```bash
grep -n "<table\|<tbody\|<td>" /tmp/resp.html
```

`部品構成表登録` の一覧テーブルで、以下を確認する
（`docs/design/画面イメージ-画面のロード時.png` の再現になっているか）：

- Item・Bom・Material が品番 → 部品品番の順で結合・ソートされている
- 同一品番の2行目以降は品番・品名のセルが空欄になっている（結合セル風の表示）
- 各行頭のチェックボックスはグループ先頭行にのみ存在する
- 日本語（品名・部品品名）が文字化けせず表示されている

ブラウザで実際に目視・操作したい場合は、VS Code のポートフォワードで 5299 番を開き
`http://localhost:5299/` にアクセスする（追加・削除の一連の操作もそこから試せる）。

## テーブル作成・マイグレーション（初回のみ／スキーマ変更時）

```bash
# 初回のみ：dotnet-ef がなければインストール
export PATH="$PATH:$HOME/.dotnet/tools"
dotnet tool install --global dotnet-ef --version 8.0.11   # 既に入っていればスキップ

cd /workspaces/src

# Domain/Persistenceのマッピングを変更したとき
dotnet ef migrations add <MigrationName> --project Sample.Persistence --startup-project Sample.Web --output-dir Migrations

# Oracleへ反映
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update --project Sample.Persistence --startup-project Sample.Web
```

**注意**：`dotnet ef` はスタートアッププロジェクト（`Sample.Web`）にも
`Microsoft.EntityFrameworkCore.Design` パッケージの参照を要求する。
`Sample.Persistence` だけに入れても
`Your startup project 'Sample.Web' doesn't reference Microsoft.EntityFrameworkCore.Design`
というエラーになる。両プロジェクトに追加済み（`PrivateAssets=all` で開発時のみの依存として）。

## サンプルデータ投入

テーブルが空の状態で見た目を確認したい場合、`docs/design/画面イメージ-画面のロード時.png`
と同じデータ（i1/i2, m11/m21/m22/m23）を入れると比較しやすい。
`SampleDbContext` を直接使う簡単な使い捨てコンソールアプリ（scratchpad配下）で
`ItemRecord` / `MaterialRecord` / `BomRecord` を `Add` → `SaveChangesAsync` するのが手早い
（`sqlplus` 等のOracleクライアントツールはこのdevcontainerに入っていない）。

## 停止

```bash
grep -i ":14B3" /proc/net/tcp   # 5299 を保持しているソケットの inode を確認
# 手順2と同じ方法で PID を特定し
kill -9 <PID>
```

## ハマりどころ

- `lsof` / `ss` / `netstat` がこのdevcontainerには無い。ポート使用状況は
  `/proc/net/tcp` を直接読み、ポート番号を16進に変換してgrepする
  （例：5299 → `14B3`）。
- 一度起動したプロセスを止め忘れたまま次を起動すると
  `Failed to bind to address ... address already in use` で失敗する。
  再起動の前に必ずポート解放を確認する。
- `chromium-cli` / Node.js / Playwright が無いため、UIの見た目（ピクセル）確認はできない。
  HTMLレスポンスの文字列検査で代替する。
