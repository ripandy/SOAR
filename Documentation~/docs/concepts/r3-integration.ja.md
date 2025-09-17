# R3 との連携

SOAR は、モダンで機能豊富なリアクティブプログラミングライブラリである [R3 (Reactive Extensions for C#)](https://github.com/Cysharp/R3) と高度に拡張可能に設計されています。R3 がプロジェクトに存在する場合、SOAR は自動的にそれと連携し、非同期プログラミング、イベント処理、およびデータストリーム操作のための強力な機能セットを解放します。

この連携により、SOAR のコア機能が次のように強化されます：

-   **`Observable` ストリームの公開**: `GameEvent`、`Variable`、`Collection`、`Transaction` などの SOAR オブジェクトは、イベントを R3 `Observable` ストリームとして公開し、LINQ スタイルの演算子を使用した複雑なイベント処理を可能にします。
-   **`async/await` サポートの提供**: `async/await` を使用して、イベントやトランザクションの待機がシームレスになります。
-   **高度な同時実行制御**: `Transaction` の場合、R3 は同時リクエストの処理方法をきめ細かく制御します。

## 連携の有効化

R3 との連携を有効にするには、R3 を Unity プロジェクトにインストールするだけです。SOAR は、通常 R3 パッケージのインストールによって追加される `SOAR_R3` スクリプティング定義シンボルを介して、その存在を自動的に検出します。

`SOAR_R3` シンボルが自動的に定義されない場合は、`Project Settings > Player > Other Settings > Scripting Define Symbols` で手動で追加できます。

## コアコンセプト

### Observable ストリーム

R3 を使用すると、SOAR のほとんどのイベントを `Observable` ストリームとして扱うことができます。つまり、`Where`、`Select`、`Merge`、`CombineLatest`、`Throttle` などの強力な R3 演算子を使用して、宣言的な方法で洗練されたイベント駆動型ロジックを作成できます。

### Async/Await サポート

この連携により、多くの操作に対して `...Async()` メソッドが提供され、`ValueTask` または `ValueTask<T>` が返されます。これにより、手動のコールバック管理なしで、クリーンで効率的な非同期コードを記述できます。

### Disposables とライフタイム管理

SOAR イベントへのすべてのサブスクリプション（`Subscribe()` または R3 の `Observable` を介して）は `IDisposable` を返します。メモリリークを防ぐために、これらのサブスクリプションのライフタイムを管理することが重要です。R3 は、`GameObject` が破棄されたときにサブスクリプションを自動的に破棄するための `AddTo(Component)` のような便利な拡張メソッドを提供します。

```csharp
using R3;
using Soar.Events;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    [SerializeField] private GameEvent myGameEvent;

    void Start()
    {
        myGameEvent.AsObservable()
            .Subscribe(_ => Debug.Log("Event raised!"))
            .AddTo(this); // この GameObject が破棄されるときに自動的に破棄されます
    }
}
```
