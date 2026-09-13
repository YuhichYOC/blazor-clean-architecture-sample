# Webアプリケーションを作成しようとしています。
## 使用する道具
- ASP.NET Core 8
    - 言語
        - C#
- Blazor Web App
    - レンダリングモード
        - InteractiveServer
    - Bootstrap 5
- Entity Framework Core
    - 8.23.40
    - Microsoft.EntityFrameworkCore.Relational
        - 8.0.11
- ORACLE Database
    - 23ai

## 構築
- ASP.NET による Web アプリケーションサーバーと ORACLE が稼働するサーバーをそれぞれ Docker コンテナで用意する
- ASP.NET と ORACLE 間の通信は Docker ネットワークで行う
- ASP.NET のアプリケーション待ち受けポートだけ外部に露出

## 生成するコードのスタイルについて
- クリーンアーキテクチャーを採用
    - 以下 4 層のレイヤーを用意
        - ドメイン層プロジェクト ( Sample.Domain )
        - アプリケーション層プロジェクト ( Sample.Application )
        - パーシステント層プロジェクト ( Sample.Persistence )
        - プレゼンテーション層および合成ルート ( Sample.Web )
- 非同期処理
    - パーシステント層の DB アクセスは async, await で非同期処理とする
    - プレゼンテーション層で画面再描画を長くブロッキングするメソッドにより止めないようにする
        - 処理時間が長くなりそうなものは非同期処理
- パーシステント層から ORACLE へアクセス
    - テーブルレコードの入れ物を用意
    - LINQ でアクセスを行う
    - 単純な SELECT は DTO への射影
    - 複数テーブルの更新は change tracking & SaveChanges でトランザクション制御を行う
- DIP について
    - パーシステント層
        - テーブルレコードの入れ物 & LINQ を使用したリポジトリを定義
        - DI コンテナ経由でアプリケーション層から呼び出せる機能を定義したリポジトリアダプターを定義
            - パーシステント層からドメイン層やアプリケーション層へデータを渡す必要がある場合
                - リポジトリアダプター内でリポジトリを呼び出し、ドメイン層で定義した DTO へ翻訳して返すメソッドを定義
            - パーシステント層からドメイン層やアプリケーション層へデータを返す必要がない場合 ( UPDATE や DELETE など更新を行うとき )
                - リポジトリアダプター内でリポジトリを呼び出すが、返すものはない。Task を返す ( 非同期処理セクションで触れたとおり戻り値 = 空の async が返す Task だけ )
    - アプリケーション層
        - リポジトリアダプターのインターフェースを定義
- DI コンテナについて
    - Sample.Web の合成ルートで DI コンテナへユースケースを登録
    - Sample.Web の合成ルートで DI コンテナへリポジトリを登録
    - Sample.Web の合成ルートで DI コンテナへリポジトリアダプターを登録

## 作りたいもの
### DB
- データベースの構造については以下の通り
    - @.claude/design/データベース.png

### アプリケーション ( 画面 )
- 実現したい画面の挙動について記したファイルの名前は以下の通り
    - @.claude/design/画面イメージ-画面のロード時.png
    - @.claude/design/画面イメージ-追加ボタン・新規登録.png
    - @.claude/design/画面イメージ-レコード削除パターン1.png
    - @.claude/design/画面イメージ-レコード削除パターン2.png
    - @.claude/design/画面イメージ-エラー入力1.png
    - @.claude/design/画面イメージ-エラー入力2.png
    - @.claude/design/画面イメージ-エラー入力3.png
    - @.claude/design/画面イメージ-エラー入力4.png
    - @.claude/design/画面イメージ-エラー入力5.png
    - @.claude/design/画面イメージ-エラー入力6.png
    - @.claude/design/画面イメージ-エラー入力7.png
