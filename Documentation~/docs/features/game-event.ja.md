# GameEvent

`GameEvent` は、SOAR のコア機能であり、`ScriptableObject` アセットを使用してパブリッシュ/サブスクライブパターンを実装します。
イベントは、特定の応答を必要とするプログラム実行内の発生事を表します。
これにより、アプリケーションの異なる部分間で疎結合な通信が可能になり、あるシステムによってイベントが発行（raise）され、他の複数のシステムが直接の参照を必要とせずにそれをリッスンできます。

## `GameEvent` (パラメータなし)

ベースの `GameEvent` は、データを運ばないイベントを表します。

### GameEvent の作成

`Assets > Create > SOAR > Game Events > GameEvent` メニューから `GameEvent` アセットを作成します。

![SOAR_Create-GameEvent](../assets/images/SOAR_Create-GameEvent.gif)

### スクリプトからの GameEvent の発行

スクリプトからイベントを発行するには、`GameEvent` インスタンスの `Raise()` メソッドを呼び出します。
すべてのアクティブなサブスクライバーに通知されます。

```csharp
// File: GameEventPublisherExample.cs
using Soar.Events;
using UnityEngine;

public class GameEventPublisherExample : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameEvent.Raise();
            Debug.Log($"Game Event {gameEvent.name} Raised.");
        }
    }
}
```

### スクリプトからの GameEvent のサブスクライブ

イベントをリッスンするには、イベントが発行されたときに実行される `Action` を指定して `Subscribe()` メソッドを呼び出します。
`Subscribe` メソッドは `IDisposable` を返します。これは、リスナーがイベントの受信を不要になったとき（例：`OnDestroy` または `OnDisable` 内）に破棄（dispose）する必要があります。

```csharp
// File: GameEventSubscriberExample.cs
using System;
using Soar.Events;
using UnityEngine;

public class GameEventSubscriberExample : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;
    
    private IDisposable subscription;
    
    private void Start()
    {
        subscription = gameEvent.Subscribe(OnGameEventRaised);
    }

    private void OnGameEventRaised()
    {
        Debug.Log($"Game Event {gameEvent.name} Received.");
    }
    
    private void OnDestroy()
    {
        subscription?.Dispose();
    }
}
```

## `GameEvent<T>` (データ付き)

ジェネリッククラス `GameEvent<T>` を使用すると、特定の型 `T` のデータペイロードを運ぶイベントを作成できます。

### 型付き GameEvent の作成

SOAR は、一般的な型付き `GameEvent`（例：`IntGameEvent`、`StringGameEvent`）をいくつか提供しており、これらはメニューから作成できます。
カスタムの型付き `GameEvent` は、`GameEvent<T>` を継承して作成できます：

```csharp
// File: MyCustomDataGameEvent.cs
using System;
using Soar;
using Soar.Events;
using UnityEngine;

// カスタムデータ構造を定義
[Serializable]
public struct MyCustomData
{
    public int id;
    public string message;
}

// MyCustomData 用の新しい GameEvent アセットタイプを作成
[CreateAssetMenu(fileName = "MyCustomDataGameEvent", menuName = MenuHelper.DefaultGameEventMenu + "My Custom Data GameEvent")]
public class MyCustomDataGameEvent : GameEvent<MyCustomData> { }
```

その後、`Create > SOAR > Game Events > My Custom Data GameEvent` からこのタイプのアセットを作成できます。

### 型付きイベントの発行

データペイロードを指定して `Raise(T value)` を呼び出します。

```csharp
// File: MyCustomDataGameEvent.cs
using UnityEngine;

public class MyTypedPublisher : MonoBehaviour
{
    [SerializeField] private MyCustomDataGameEvent onDataPublished;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var data = new MyCustomData { id = 1, message = "Hello World" };
            onDataPublished.Raise(data);
        }
    }
}
```

### 型付きイベントのサブスクライブ

データを受信するために `Action<T>` でサブスクライブします。

```csharp
// File: MyTypedSubscriber.cs
using System;
using UnityEngine;

public class MyTypedSubscriber : MonoBehaviour
{
    [SerializeField] private MyCustomDataGameEvent gameEvent;
    
    private IDisposable subscription;
    
    private void Start()
    {
        subscription = gameEvent.Subscribe(HandlePublishedData);
    }

    private void HandlePublishedData(MyCustomData data)
    {
        Debug.Log($"Data received: ID={data.id}, Message='{data.message}'");
    }
    
    private void OnDestroy()
    {
        subscription?.Dispose();
    }
}
```

`GameEvent<T>` には、最後に発行された値を格納する `value` フィールドもあります。この値は、アプリケーションが終了するとき、またはドメインリロードが無効にされて再生モードが終了したときに `default(T)` にリセットされます。

## R3 との連携

R3 ライブラリが存在する場合、`GameEvent` は強力なリアクティブおよび非同期機能で強化されます。

-   **`AsObservable()`**: イベントを R3 の `Observable` ストリームとして返します。
    -   パラメータなしの `GameEvent` の場合、`Observable<Unit>` を返します。
    -   `GameEvent<T>` の場合、`Observable<T>` を返します。
-   **`AsUnitObservable()`**: `GameEvent<T>` に固有で、イベントが発行されたことだけを知りたい場合に便利な `Observable<Unit>` を返します。
-   **`EventAsync()`**: 次にイベントが発行されたときに完了する `ValueTask`（または `ValueTask<T>`）を返します。
-   **`ToAsyncEnumerable()`**: イベントストリームを `IAsyncEnumerable<T>` に変換し、`await foreach` ループで消費できるようにします。

### 例

**`GameEvent<T>` で `AsObservable` を使用する:**
```csharp
using R3;
using Soar.Events;
using UnityEngine;

public class EventLogger : MonoBehaviour
{
    [SerializeField] private StringGameEvent onImportantEvent;

    void Start()
    {
        onImportantEvent.AsObservable()
            .Subscribe(message => Debug.Log($"Event received: {message}"))
            .AddTo(this);
    }
}
```

**GameEvent を待機する:**
```csharp
using Soar.Events;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameEvent onPlayerReady;

    private async void Start()
    {
        Debug.Log("Waiting for player to be ready...");
        await onPlayerReady.EventAsync();
        Debug.Log("Player is ready! Starting level.");
    }
}
```

## エディタ統合

`GameEvent` アセットには、カスタムエディタインスペクタがあります。

- **Raise ボタン**: すべての `GameEvent` のインスペクタで「Raise」ボタンが利用できます。このボタンをクリックすると、イベントの `Raise()` メソッドが呼び出され、編集モードと再生モードの両方でイベント応答のデバッグとテストに役立ちます。

- **値の表示 (`GameEvent<T>` の場合)**: 型付きの `GameEvent<T>` の場合、現在の `value`（または複雑な型の場合はそのプロパティ）がインスペクターに表示されます。これにより、新しい値を指定せずに「Raise」ボタンを押した場合に発行されるデータを表示したり、場合によっては変更したりできます。

## ライフサイクルと破棄

`GameEvent` は `ScriptableObject` であり、`IDisposable` を実装しています。

- サブスクリプションは内部で管理されます。`GameEvent` が破棄されると（通常はアプリケーション終了時に `SoarCore` ライフサイクルの一部として行われます）、そのサブスクリプションはクリアされます。
- 独立した実装（`SOAR_R3` が定義されていない場合）では、サブスクリプションはリストに格納され、破棄されます。
- `SOAR_R3` が定義されている場合、`GameEvent` は内部で R3 の `Subject` を使用し、`Subject` を破棄することで破棄が処理されます。
- `SoarCore` ベースクラスは、初期化とクリーンアップを処理し、`OnQuit` 中に `Dispose()` を呼び出すことも含みます。また、Unity の「Enter Play Mode Options」（ドメインリロードの無効化など）に関連する動作も管理します。

これにより、リソースが正しくクリーンアップされ、イベントリスナーが破棄されたオブジェクトを操作しようとしないことが保証されます。

## サンプル使用法

この機能をテストするには、パッケージマネージャウィンドウから関連するサンプルパッケージをインポートできます。

![SOAR_ImportSamples_GameEvent](../assets/images/SOAR_ImportSamples_GameEvent.png)

The **GameEvent サンプル**は、UI ボタンが `GameEvent` を発行して、直接の参照なしにリスナーコンポーネントと通信する方法を示します。リスナーは、応答として UI テキスト要素を更新します。

詳細なセットアップと使用手順については、インポート後に `GameEventSamples` フォルダ内の `README.md` ファイルを参照してください。
