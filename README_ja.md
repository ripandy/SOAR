# Scriptable Object Architecture Reactive-extensible (SOAR)

[English](./README.md)

<!-- [![openupm](https://img.shields.io/npm/v/com.ripandy.soar?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.ripandy.soar/) -->

SOARは、Scriptable Object Architectureの実装です。
Scriptable Object Architectureの目的は、クリーンで疎結合なコンポーネントアーキテクチャを作成することです。
SOARは、[Ryan Hipple氏のUnite Austin 2017での講演](https://youtu.be/raQ3iHhE_Kk)に基づいて実装されています。

SOARは、[Pub/Subパターン](#publishersubscriber-pattern)の使用を推奨するイベントベースのシステムです。
その基本原則は、[ScriptableObject]インスタンス（`name`/`guid`プロパティを持つ）を「チャネル」または「トピック」として扱うことです。
パブリッシャーとサブスクライバーのペアリングは、SOARの各インスタンスへの参照を通じて確立されます。

SOARは、C#向けの機能豊富でモダンなリアクティブエクステンションライブラリである[R3]で拡張可能に開発・設計されています。
SOARは、Scriptable Object Architecture内でR3の機能をラップして利用します。
SOARは単体でも機能しますが、その実装は基本的な機能のみを提供します。
R3と組み合わせてSOARを使用することを強くお勧めします。

- [R3] - dotnet/reactiveと[UniRx]の新しい未来。
- [Kassets] - SOARの前身。 [UniRx]と[UniTask]で拡張可能なScriptable Object Architecture。

__詳細については、[ドキュメント]を参照してください__

### Unityバージョン
- Unity 6.0以降

> [!NOTE]
> このリポジトリはUnityパッケージであり、スタンドアロンのUnityプロジェクトではありません。Unityエディタで直接開くことはできません。[インストール](#installation)の指示に従って、プロジェクトに追加してください。

# はじめに

## インストール

<details>

<summary>OpenUPMから追加 | <em>スコープ付きレジストリ経由でインポート</em></summary>

パッケージマネージャにOpenUPMを追加するには：

- `Edit/Project Settings/Package Manager`を開きます
- 新しいスコープ付きレジストリを追加します：
```
Name: OpenUPM
URL:  https://package.openupm.com/
Scope(s):
  - com.ripandy
  - com.cysharp.r3 (推奨)
```
- <kbd>Save</kbd>をクリックします
- `Window/Package Manager`を開きます
- 左上のドロップダウンで`My Registries`を選択します
- `SOAR`を選択し、`Install`をクリックします
- `R3`を選択し、`Install`をクリックします（推奨）（注を参照）

</details>

<details>

<summary>GitHubから追加 | <em>GitHubリンクを使用してインポート</em></summary>

GitHubから直接パッケージを追加します。

- `Window/Package Manager`を開きます
- `+`アイコンをクリックします
- `Install package from Git URL`オプションを選択します
- 次のURLを貼り付けます： `https://github.com/ripandy/SOAR.git`
- `Add`をクリックします

特定のバージョンをインストールするには、SOARのリリースバージョンタグをURLに追記します。例：`https://github.com/ripandy/SOAR.git#1.0.0`

</details>

<details>

<summary>ローカルフォルダにクローン | <em>変更を個別に管理</em></summary>

- このリポジトリをローカルディレクトリにクローンします。
- `Window/Package Manager`を開きます
- `+`アイコンをクリックします
- `Install package from disk`オプションを選択します
- クローンしたディレクトリから`package.json`ファイルを選択します。
- `Add`をクリックします

パッケージはローカルパッケージ（`file://`）として`manifest.json`に追加されます。
ソースコードは、含むUnityプロジェクトから変更できます。
変更は通常通りgitで管理できます。
パッケージパスは、デフォルトの絶対パスの代わりに相対パスに変更することもできます。

```json
{
  "dependencies": {
    "com.ripandy.soar": "file:../path/to/your/local/SOAR"
  }
}
```

</details>

<details>

<summary>Packagesフォルダにクローン | <em>開発または貢献目的</em></summary>

このリポジトリをUnityプロジェクトの`Packages`ディレクトリにクローンします：`YourUnityProject/Packages/`。

Unityはプロジェクトをカスタムパッケージとして扱います。
ソースコードは、含むUnityプロジェクトから変更できます。
変更は通常通りgitで管理できます。
SOARはgitリポジトリのサブモジュールとしてクローンすることもできます。

</details>

> [!NOTE]
> Unityに[R3]をインストールするには、そのNuGetバージョンをインストールする必要があります。詳細については、[R3 Unity Installation](https://github.com/Cysharp/R3?tab=readme-ov-file#unity)を参照してください。

## クイックスタート

### GameEventインスタンスの作成

SOARのインスタンスは、`Create`コンテキストメニューまたはメニューバーの`Assets/Create`から作成できます。
Projectウィンドウを右クリックし、作成するインスタンスを選択します。
GameEventインスタンスの場合、`Create/SOAR/Game Events/`メニューからいずれかのイベントタイプを選択します。

![SOAR_Create-GameEvent](https://github.com/user-attachments/assets/7fef75b6-995b-4195-a4c8-4d6548b017c6)

### UnityEventsからGameEventを発生させる

Unity UIのボタンからイベントを発生させるには、作成したGameEventインスタンスをボタンのOnClickイベントに割り当てます。
ボタンがクリックされるたびに、イベントが発行され、すべてのサブスクライバーに通知されます。

![Screen Recording 2025-05-17 at 10 44 03 mov](https://github.com/user-attachments/assets/7354506f-13d6-45f9-90f2-9f920bac9964)

### UnityEventBinderの使用法

Unity Event Binderは、`GameEvent`によって発生したイベントを`UnityEvent`に転送するカスタム実装のUnityコンポーネントです。
これは、Scriptable Object Architectureの用語で`EventListener`としても知られています。

使用するには、任意のGameObjectにコンポーネントを追加し、`Game Event To Listen`フィールドにGameEventインスタンスを割り当てます。

![SOAR_UnityEventBinder_AssignGameEvent](https://github.com/user-attachments/assets/5b0604ed-28a9-41e6-9045-92f2d38314a4)

イベントが発生すると、`On Game Event Raised`に割り当てられたアクションがUnityEventによって呼び出されます。

<img width="300" src="https://github.com/user-attachments/assets/d13742c0-a75d-4094-a1a6-f2596bea58ba" alt="SOAR_UnityEventBinder_AssignUnityEvent"/>

### スクリプトからGameEventを発生させる

スクリプトからイベントを発生させるには、`GameEvent`インスタンスの`Raise()`メソッドを使用します。
イベントが発生すると、すべてのサブスクライバーに通知されます。

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

### スクリプトからGameEventをサブスクライブする

スクリプトからイベントをサブスクライブするには、`GameEvent`インスタンスの`Subscribe()`メソッドを使用します。
サブスクライブすると、イベントが発生したときに提供されたコールバックが呼び出されます。

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

### インスペクターからGameEventを発生させる

インスペクターウィンドウからイベントを発生させるには、`GameEvent`インスタンスを選択し、インスペクターの`Raise`ボタンを押します。
これにより、イベントが呼び出され、すべてのサブスクライバーに通知されます。
SOARの疎結合な性質により、サブスクライバーはイベントのソースを意識する必要がありません。
これにより、特定のパブリッシャーを必要とせずにイベントの簡単なテストとデバッグが可能になります。

![SOAR_EditModeRaise mov](https://github.com/user-attachments/assets/0a0cca97-8452-4fe5-8285-5f69548e275a)

> [!NOTE]
> デフォルトでは、イベントリスナーは編集モードでは呼び出されません。サブスクライバーは、編集モードでの実行を手動で処理する必要があります。

### 編集モードで実行するようにサブスクライバーを設定する

編集モードで実行するようにサブスクライバーを設定するには、サブスクライバークラスで`ExecuteAlways`または`ExecuteInEditMode`属性を使用します。
これにより、サブスクライバーが編集モードで呼び出されるようになります。

```csharp
// File: GameEventSubscriberExample.cs
using System;
using Soar.Events;
using UnityEngine;

[ExecuteAlways]
public class GameEventSubscriberExample : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;

    private IDisposable subscription;

    private void OnEnable()
    {
        // 編集モードと再生モードの両方でサブスクライブ
        SubscribeToEvent();
    }

    private void OnDisable()
    {
        // 編集モードと再生モードの両方でアンサブスクライブ
        UnsubscribeFromEvent();
    }

    private void SubscribeToEvent()
    {
        if (gameEvent == null) return;
        
        // 既存のサブスクリプションがあれば破棄
        subscription?.Dispose();
        
        // 新しいサブスクリプションを作成
        subscription = gameEvent.Subscribe(OnGameEventRaised);
    }

    private void UnsubscribeFromEvent()
    {
        subscription?.Dispose();
        subscription = null;
    }

    private void OnGameEventRaised()
    {
        Debug.Log($"Game Event {gameEvent.name} Received on {(Application.isPlaying ? "Play" : "Edit")} mode.");
    }
}
```

# 機能

### Command

`Command`は、[コマンドパターン](https://gameprogrammingpatterns.com/command.html)の実装であり、`Interface`の代わりに`ScriptableObject`を利用します。
`Command`クラスは抽象クラスであり、具象実装が必要です。
このパターンは、ロギングなどの一方向の実行に役立ちます。


### GameEvent

イベントは、特定の応答を必要とするプログラム実行内の発生事を表します。
各イベントには、少なくとも1つのパブリッシャーと1つのサブスクライバーが必要です。


### Variable

Variableは、操作可能な`ScriptableObject`に保存されるデータです。
SOARでは、`Variable`は`GameEvent`から派生し、変更時に値変更イベントをトリガーします。


### Collection

Collectionは、複数のアイテムを保持できるデータ構造です。
`Collection`クラスは、`SoarList`および`SoarDictionary`のベースとして機能します。
`Collection`は、アイテムが追加、削除、更新されたとき、またはコレクションがクリアされたときにトリガーされる追加のイベントを提供します。
`Collection`は、LINQとの互換性を確保するために、`ICollection`、`IList`、`IDictionary`などの一般的なインターフェイスを実装しています。


### Transaction

Transactionは、リクエストとレスポンスを伴う双方向のイベントです。
リクエストが送信されると、登録されたレスポンスイベントがそれを処理し、結果をリクエスターに返します。
一度に登録できるレスポンスイベントは1つだけです。
これは、操作がイベントの完了を待つ必要がある場合に役立ちます。


### Base Classes

SOARは、すぐに使用できるデフォルトの基本クラスを提供します。
これらは、`Create > SOAR`コンテキストメニューまたは`Assets > Create > SOAR`メニューバー項目からアクセスできます。
Base Classesは異なるアセンブリ定義ファイル`(.asmdef)`を使用することに注意してください。
プロジェクトで`Soar.Base`の参照を追加するには、手動での`.asmdef`参照管理が必要になる場合があります。


### Unity Event Binder

Unity Event Binderは、`GameEvent`によって発生したイベントを`UnityEvent`に転送するカスタム実装のUnityコンポーネントです。
これは、Scriptable Object Architectureの用語で`EventListener`としても知られています。


### Json Extension

SOARは、SOAR VariableのデータをJSON文字列またはローカルJSONファイルに変換するエディタツールを実装しています。
この機能は、SOAR Variableのインスペクターウィンドウからアクセスできます。
`JsonableVariable<T>`から派生したクラスでのみ使用できます。


# Reactive ExtensionsライブラリR3との連携

SOARでReactive ExtensionsライブラリR3を使用するには、プロジェクトにR3をインポートするだけです。
インポートすると、SOARは`SOAR_R3`スクリプティング定義を使用して内部統合を調整します。これは、パッケージマネージャ経由でR3がインポートされると自動的に定義されるはずです。
`SOAR_R3`スクリプティング定義が何らかの理由で未定義の場合は、プロジェクト設定の`Scripting Define Symbols`に追加します。

R3をインポートすることにより、SOARには追加機能があります：

- __R3による内部イベント処理__

  SOARは、Reactive ExtensionsライブラリR3で拡張可能に開発・設計されています。
  `Observable`を単純にラップし、それらを内部イベントハンドラとして利用します。

- __ValueTaskによる非同期イベント処理__

  R3をインポートすると、SOARのイベントハンドラは`ValueTask`および`IAsyncObservable`で使用できます。

  ```csharp
  // File: GameEventAwaitExample.cs
  using System.Threading.Tasks;
  using Soar.Events;
  using UnityEngine;
  
  public class GameEventAwaitExample : MonoBehaviour
  {
      [SerializeField] private GameEvent gameEvent;
  
      private async void Start()
      {
          // Startでゲームイベントを待機
          await AwaitGameEvent();
      }
  
      private async ValueTask AwaitGameEvent()
      {
          Debug.Log("Waiting for game event...");
          await gameEvent.EventAsync();
          Debug.Log("Game event received!");
      }
  }
  ```

- 変換メソッド

  R3をインポートすると、SOARのインスタンスは[R3]の`Observable`、(Uni)Rxの`IObservable`、および(Uni)Taskの`IAsyncObservable`に変換可能になります。
  これにより、SOARのインスタンスはそれぞれのライブラリの機能を利用できます。
  変換後、詳細については各ライブラリのドキュメントを参照してください。

  ```csharp
  // SOARのGameEvent<T>に実装
  AsObservable()        // SOARのインスタンスをR3のObservable<T>に変換
  AsUnitObservable()    // SOARのインスタンスをR3のObservable<Unit>に変換
  AsSystemObservable()  // SOARのインスタンスをSystemのIObservable<T>に変換。UniRxでさらに拡張可能。
  ToAsyncEnumerable()   // SOARのインスタンスをSystemのIAsyncEnumerable<T>に変換
  ```

# Publisher/Subscriberパターン

The Publisher/Subscriber（Pub/Sub）パターンは、次のようなメッセージングパターンです：

- パブリッシャーは、どのコンポーネントがメッセージ/イベントを受信するかを直接知ることなく、メッセージ/イベントを発行します
- サブスクライバーは、どのコンポーネントがイベントを生成するかを知ることなく、特定のイベントに関心を登録します
- メッセージブローカーまたはイベントチャネルがそれらの間を仲介し、コンポーネントを疎結合にします

SOARの実装では：

- SOARのインスタンスがメッセージチャネルまたはトピックとして機能します
- パブリッシャーはGameEventで`Raise()`メソッドを呼び出してメッセージをブロードキャストします
- サブスクライバーは`Subscribe()`を使用して、イベントが発生したときに実行されるコールバックを登録します

明示的なメッセージブローカーの実装はありませんが、シーンを越えて永続するScriptableObjectインスタンスの性質により、互いに直接参照することのないコンポーネント間の通信が可能になります。
このパターンは、Unityで優れた疎結合を提供し、コンポーネントが直接的な依存関係なしに通信できるようにします。

# MVPパターンでのSOARの活用

SOARは、Model-View-Presenter（MVP）パターンによく適合します。
SOARの`GameEvent`または`Variable`インスタンスは、モデルをラップするために利用できます。
ラッパーとして、SOARはモデルに直接関与しません。
モデルは、異なる名前空間またはアセンブリにある可能性があります。
次に、MonoBehaviourを継承するクラスをプレゼンターまたはコントローラーとして使用します。
これらのクラスは`GameEvent`または`Variable`インスタンスをサブスクライブし、イベントから受け取ったモデルを処理します。
最後に、UnityのコンポーネントまたはUI要素をビューとして使用し、処理されたモデルデータを使用してプレゼンターによって更新されます。

![image](https://github.com/user-attachments/assets/99b63d4d-562d-43b8-9516-38e136772eda)

# 参考文献
- [Unite2017 Scriptable Object Architectureサンプルプロジェクト](https://github.com/roboryantron/Unite2017)
- [Unite2017 Game Architecture with Scriptable Objects on Slideshare](https://www.slideshare.net/RyanHipple/game-architecture-with-scriptable-objects)
- [R3 — A New Modern Reimplementation of Reactive Extensions for C#](https://neuecc.medium.com/r3-a-new-modern-reimplementation-of-reactive-extensions-for-c-cf29abcc5826)
- [Kassets (SOARの前身). Scriptable Object Architecture extensible with UniRx and UniTask](https://github.com/kadinche/Kassets).

# ライセンス

- SOARは[MITライセンス](LICENSE)の下でライセンスされています
- [R3]は[MITライセンス](https://github.com/Cysharp/R3/blob/main/LICENSE)の下でライセンスされています
- [Kassets]（SOARの前身）は[MITライセンス](https://github.com/kadinche/Kassets/blob/main/LICENSE)の下でライセンスされています

[R3]: https://github.com/Cysharp/R3
[Kassets]: https://github.com/kadinche/Kassets
[UniRx]: https://github.com/kadinche/UniRx
[UniTask]: https://github.com/Cysharp/UniTask
[ドキュメント]: https://ripandy.github.io/SOAR/
[ScriptableObject]: https://docs.unity3d.com/Manual/class-ScriptableObject.html
