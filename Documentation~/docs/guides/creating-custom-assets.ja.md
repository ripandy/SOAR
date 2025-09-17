# カスタム SOAR アセットの作成

SOAR には、プリミティブ型（`IntVariable`、`StringList` など）に対応した豊富な「基本」アセットが付属していますが、フレームワークの真のポテンシャルは、プロジェクト固有のデータ構造に合わせたアセットを作成することで引き出されます。このガイドでは、一般的なユースケースであるプレイヤーデータの管理のために、カスタム SOAR アセットの完全なセットを作成するプロセスを示します。

このプロセスはシンプルで一貫性があり、定型コードはほとんど必要ありません。作成されると、カスタムアセットは、組み込み型と同様に、Unity の `Assets > Create` メニューにシームレスに統合されます。

## 例：`PlayerData`

このガイドでは、次の `PlayerData` 構造体で動作する一連の SOAR アセットを作成します。この構造体は、プレイヤーに関するすべての重要な情報を保持します。

### ステップ1：データ構造の定義

まず、データ用の新しい C# スクリプトを作成する必要があります。最も重要なステップは、`struct` または `class` に `[System.Serializable]` 属性をマークすることです。これにより、Unity はそれをシリアル化し、インスペクターに表示できます。

```csharp
// File: PlayerData.cs
using System;

[Serializable]
public struct PlayerData
{
    public string playerName;
    public int level;
    public float health;
}
```

この `PlayerData` 構造体を使用して、`Variable`、`GameEvent`、`Collection`、および `Transaction` を作成できるようになりました。

## ステップ2：カスタム `Variable` の作成

`Variable` は、単一のデータの状態を保持します。`PlayerDataVariable` を作成して、現在のプレイヤーの情報を保存できます。

`PlayerDataVariable.cs` という名前の新しいスクリプトを作成する必要があります：

```csharp
// File: PlayerDataVariable.cs
using Soar;
using Soar.Variables;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerDataVariable", menuName = MenuHelper.DefaultVariableMenu + "Player Data")]
public class PlayerDataVariable : Variable<PlayerData> { }
```

`[CreateAssetMenu]` 属性により、エディターで利用可能になります。`MenuHelper.DefaultVariableMenu` は、他のすべての変数と同じサブメニュー（`SOAR > Variables`）に表示されるようにするために使用されます。

**結果：** `Assets > Create > SOAR > Variables > Player Data` から新しい `PlayerDataVariable` アセットを作成できるようになりました。

## ステップ3：カスタム `GameEvent` の作成

`GameEvent` は、何かが起こったことをブロードキャストするために使用されます。`PlayerDataGameEvent` を使用して、プレイヤーが接続したとき、またはデータが更新されたときに通知できます。

`PlayerDataGameEvent.cs` という名前の新しいスクリプトを作成する必要があります：

```csharp
// File: PlayerDataGameEvent.cs
using Soar;
using Soar.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerDataEvent", menuName = MenuHelper.DefaultGameEventMenu + "Player Data")]
public class PlayerDataGameEvent : GameEvent<PlayerData> { }
```

**結果：** このイベントは `Assets > Create > SOAR > Game Events > Player Data` から作成できるようになりました。

## ステップ4：カスタム `Collection` (List) の作成

`Collection` は、アイテムのリストまたは辞書を管理します。`PlayerDataList` を作成して、マルチプレイヤーロビーのすべてのプレイヤーのリストを保存できます。

`PlayerDataList.cs` という名前の新しいスクリプトを作成する必要があります：

```csharp
// File: PlayerDataList.cs
using Soar;
using Soar.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerDataList", menuName = MenuHelper.DefaultListMenu + "Player Data")]
public class PlayerDataList : SoarList<PlayerData> { }
```

**結果：** このリストは `Assets > Create > SOAR > Lists > Player Data` から作成できるようになりました。

## ステップ5：カスタム `Transaction` の作成

`Transaction` は、リクエスト/レスポンスサイクルを処理します。`FetchPlayerDataTransaction` を作成して、リクエストとして `string`（プレイヤーの ID）を受け取り、レスポンスとして対応する `PlayerData` を返すことができます。

`FetchPlayerDataTransaction.cs` という名前の新しいスクリプトを作成する必要があります：

```csharp
// File: FetchPlayerDataTransaction.cs
using Soar;
using Soar.Transactions;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFetchPlayerDataTransaction", menuName = MenuHelper.DefaultTransactionMenu + "Fetch Player Data")]
public class FetchPlayerDataTransaction : Transaction<string, PlayerData> { }
```

**結果：** このトランザクションは `Assets > Create > SOAR > Transactions > Fetch Player Data` から作成できるようになりました。

## ステップ6：カスタム `UnityEventBinder` の作成

新しい `PlayerDataGameEvent` をエディターで簡単に接続するために、カスタム `UnityEventBinder` を作成できます。

`PlayerDataUnityEventBinder.cs` という名前の新しいスクリプトを作成する必要があります：

```csharp
// File: PlayerDataUnityEventBinder.cs
using Soar.Events;

public class PlayerDataUnityEventBinder : UnityEventBinder<PlayerData> { }
```

**結果：** `PlayerDataUnityEventBinder` コンポーネントを任意の `GameObject` に追加できるようになりました。これにより、`PlayerDataGameEvent` をリッスンし、`UnityEvent<PlayerData>` を呼び出して、イベントデータをインスペクターの他のコンポーネントに直接渡すことができます。

## 結論

このシンプルで一貫性のあるパターンに従うことで、プロジェクト内のあらゆるデータ型に対して、堅牢でタイプセーフ、かつエディターフレンドリーなアーキテクチャを作成できます。これにより、コードベースがよりモジュール化され、テストが容易になり、チーム全体が追加のコードを記述することなく、Unity エディターで直接複雑なロジックを接続できるようになります。
