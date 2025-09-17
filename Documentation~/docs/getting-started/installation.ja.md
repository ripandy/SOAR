# インストール

## 始める前に

SOAR は Unity パッケージであり、以下のいずれかの方法でインストールできます。**Unity 6.0 以降**が必要です。

SOAR は独立して機能することもできますが、[R3] ライブラリで拡張できるように設計されています。SOAR のポテンシャルを最大限に引き出すために、R3 をインストールすることを強くお勧めします。

## OpenUPM

__スコープ付きレジストリ経由でインポートし、パッケージマネージャから更新します。__

プロジェクトに OpenUPM を追加するには：

- `Edit/Project Settings/Package Manager` を開きます。
- 新しいスコープ付きレジストリを追加します：
```
Name: OpenUPM
URL:  https://package.openupm.com/
Scope(s):
  - com.ripandy
  - com.cysharp.r3 (推奨)
```
- `Save` をクリックします。
- `Window/Package Manager` を開きます。
- 左上のドロップダウンで `My Registries` を選択します。
- `SOAR` を選択し、`Install` をクリックします。
- `R3` を選択し、`Install` をクリックします（推奨）（注を参照）。

!!! note
    [R3] のインストールには、NuGet からの依存関係のインポートが必要です。詳細については、[R3 Unity Installation](https://github.com/Cysharp/R3?tab=readme-ov-file#unity) を参照してください。

## GitHub URL

__インポートには GitHub リンクを使用します。Unity 2021.2 以降ではパッケージマネージャを使用して更新します。__

SOAR は GitHub から直接追加できます。
Unity 2021.2 以降では、パッケージマネージャを使用して main ブランチの最新バージョンに更新できます。
それ以外の場合は、パッケージを削除してから再度追加して手動で更新する必要があります。

- パッケージマネージャを開きます。
- `+` アイコンをクリックします。
- `Add from Git URL` オプションを選択します。
- 次の URL を貼り付けます： `https://github.com/ripandy/SOAR.git`
- `Add` をクリックします。

特定のバージョンをインストールするには、SOAR のリリースタグを参照してください。
例： `https://github.com/ripandy/SOAR.git#1.0.0`

## ローカルフォルダにクローン

__SOAR を独立したプロジェクトとしてローカルフォルダにクローンします。__

SOAR はローカルフォルダにクローンして、独立したプロジェクトとして扱うことができます。
ただし、SOAR の機能をテストするには、別の Unity プロジェクトが必要です。

- SOAR のリポジトリをローカルディレクトリにクローンします。
- SOAR をインポートするために、対象の Unity プロジェクトを開きます。
- `Window/Package Manager` を開きます。
- `+` アイコンをクリックします。
- `Install package from disk` オプションを選択します。
- クローンしたディレクトリから `package.json` ファイルを選択します。
- `Add` をクリックします。

パッケージはローカルパッケージ (`file://`) として manifest.json に追加されます。
ソースコードは、対象の Unity プロジェクトから変更できます。
変更は通常通り git で管理できます。
パッケージパスは、デフォルトの絶対パスの代わりに相対パスに変更することもできます。

```json
{
  "dependencies": {
    "com.ripandy.soar": "file:../path/to/your/local/SOAR"
  }
}
```

## Packages フォルダにクローン

このリポジトリを Unity プロジェクトの `Packages` ディレクトリにクローンします： `YourUnityProject/Packages/`。

Unity はプロジェクトをカスタムパッケージとして扱います。
ソースコードは、対象の Unity プロジェクトから変更できます。
変更は通常通り git で管理できます。
SOAR は、メインの git リポジトリのサブモジュールとしてクローンすることもできます。


[SOAR]: https://github.com/ripandy/SOAR
[R3]: https://github.com/Cysharp/R3
