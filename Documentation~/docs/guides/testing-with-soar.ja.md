# SOAR を使用したテスト

Scriptable Object Architecture の主な利点の1つは、コードのテストが大幅に容易になることです。コンポーネントを疎結合にすることで、複雑なシーンを構築したり、他のシステムを存在させる必要なく、ロジックを分離してテストできます。

このガイドでは、Unity の組み込みテストフレームワークを使用して、SOAR アセットを使用するコンポーネントの単体テストを作成する方法を示します。

## テスト対象のコンポーネント

ダメージを受けるために `GameEvent` をリッスンする単純な `Player` コンポーネントを考えてみましょう。目標は、完全なゲーム環境を必要とせずに、このコンポーネントのロジックをテストすることです。

テストするコンポーネントは次のとおりです：

```csharp
// File: Player.cs
using Soar.Events;
using Soar.Variables;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private FloatVariable playerHealth;
    [SerializeField] private FloatGameEvent onPlayerDamaged;

    private void Start()
    {
        // 開始時に体力を満タンにする
        playerHealth.Value = 100f;

        // ダメージイベントをサブスクライブする
        onPlayerDamaged.Subscribe(TakeDamage);
    }

    private void TakeDamage(float amount)
    {
        playerHealth.Value -= amount;
    }
}
```

## 単体テストの作成

`Player` コンポーネントをテストするには、「Editor」フォルダ（またはテスト用に構成されたアセンブリ）内にテストスクリプトを作成できます。

テストの主要なステップは次のとおりです：
1.  **準備（Arrange）：** 必要な SOAR アセット（`FloatVariable`、`FloatGameEvent`）と `Player` コンポーネント自体のインスタンスを作成します。
2.  **実行（Act）：** テストするイベントをシミュレートします（`onPlayerDamaged` イベントを発生させます）。
3.  **検証（Assert）：** コンポーネントが期待どおりに動作したことを確認します（`playerHealth` `Variable` が減少したこと）。

対応するテストスクリプトは次のとおりです：

```csharp
// File: PlayerTests.cs
using NUnit.Framework;
using Soar.Events;
using Soar.Variables;
using UnityEngine;

public class PlayerTests
{
    [Test]
    public void Player_TakesDamage_When_OnPlayerDamagedEventIsRaised()
    {
        // 1. 準備
        // コンポーネントをホストする GameObject を作成
        var playerGameObject = new GameObject();
        var player = playerGameObject.AddComponent<Player>();

        // 必要な SOAR アセットのインスタンスをメモリに作成
        var healthVariable = ScriptableObject.CreateInstance<FloatVariable>();
        var damageEvent = ScriptableObject.CreateInstance<FloatGameEvent>();

        // リフレクションまたはパブリックセッターを使用して、これらのアセットを Player コンポーネントに割り当てます
        // (これにより、テストのためにプレハブにシリアル化する必要がなくなります)
        SetPrivateField(player, "playerHealth", healthVariable);
        SetPrivateField(player, "onPlayerDamaged", damageEvent);

        // Start() を手動で呼び出して、サブスクリプションをトリガーし、初期体力を設定します
        player.SendMessage("Start");

        // 2. 実行
        // ダメージイベントが発生したことをシミュレートします
        damageEvent.Raise(25f);

        // 3. 検証
        // 体力変数が正しく更新されたかどうかを確認します
        Assert.AreEqual(75f, healthVariable.Value);

        // GameObject をクリーンアップします
        Object.DestroyImmediate(playerGameObject);
    }

    // テスト目的でプライベートフィールドを設定するためのヘルパーメソッド
    private void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }
}
```

## テストにおける重要な概念

*   **インメモリアセット：** `ScriptableObject.CreateInstance<T>()` を使用して、テストに必要な SOAR アセットの一時的なインメモリバージョンを作成します。これらはプロジェクトに保存されず、テストの実行後にガベージコレクションされます。
*   **手動初期化：** `Player` コンポーネントはシーン内にないため、その `Start()` メソッドを `SendMessage("Start")` を介して手動で呼び出して、そのロジック（初期体力の設定やイベントのサブスクライブなど）が実行されるようにする必要があります。
*   **イベントのシミュレート：** テストは SOAR アセットを直接制御します。`damageEvent.Raise(25f)` を呼び出すことで、テストは「パブリッシャー」の役割を果たし、`Player` コンポーネントのロジックをトリガーします。
*   **疎結合の実践：** テストは、プレイヤーが*どのように*ダメージを受けるかを知る必要はなく、`onPlayerDamaged` イベントをリッスンすると `playerHealth` 変数が変更されることだけを知る必要があることに注意してください。これは、疎結合コンポーネントのテストの力を示しています。

このパターンに従うことで、入力と出力に SOAR を利用するコンポーネントを徹底的かつ効率的にテストでき、より安定して保守しやすいコードベースにつながります。
