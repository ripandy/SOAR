# 基本クラス

SOAR は、`Variable`、`GameEvent`、`Collection` などのコア機能に対して、事前定義された具体的な「基本クラス」のセットを提供します。
これらは、一般的なデータ型（例：`int`、`float`、`string`、`Vector3`）に対応した、すぐに使える `ScriptableObject` アセットであり、開発者は定型コードを記述することなく SOAR の機能をすぐに使用できます。
これらは別の `Soar.Base` アセンブリに配置されており、アセンブリ定義参照の追加が必要になる場合があります。

## 基本クラスの使用

事前定義された基本クラスを使用するには、プロジェクトのアセンブリ定義ファイル（`.asmdef`）に `Soar.Base` への参照があることを確認してください。

1.  **アセンブリ定義ファイルの特定**: 基本クラスを使用するスクリプトを管理するプロジェクト内の `.asmdef` ファイルを見つけます。存在しない場合は、プロジェクトウィンドウで右クリックし、`Create > Assembly Definition` を選択して作成します。

2.  **参照の追加**: `.asmdef` ファイルを選択してインスペクターで開きます。「Assembly Definition References」セクションで `+` ボタンをクリックし、`Soar.Base` を追加します。

    ![SOAR_Asmdef_BaseClass](../assets/images/SOAR_Asmdef_BaseClass.gif)
4
3.  **変更の適用**: 「Apply」をクリックして変更を保存します。

参照が追加されると、Unity エディターのコンテキストメニューからこれらの基本クラスのインスタンスを直接作成できます。

!!! Note "デフォルトの Assembly-CSharp"
    プロジェクトに指定された asmdef ファイルがない場合、SOAR の基本クラスはスクリプトから自動的に参照できます。これはプロトタイピングには非常に便利ですが、本番環境では推奨されません。

## 利用可能な基本クラス

基本クラスは、`Assets > Create > SOAR` メニューの各機能カテゴリの下に整理されています。

### GameEvents

一般的なデータ型のパラメータなしおよび型付きの `GameEvent` は、`Assets > Create > SOAR > Game Events` にあります。

-   **パラメータなし**:
    - `GameEvent` (単純なシグナル用)
-   **プリミティブ型**:
    - `BoolGameEvent`
    - `ByteGameEvent`
    - `DoubleGameEvent`
    - `FloatGameEvent`
    - `IntGameEvent`
    - `LongGameEvent`
    - `StringGameEvent`
-   **Unity 型**:
    - `AudioClipGameEvent`
    - `GameObjectGameEvent`
    - `PoseGameEvent`
    - `QuaternionGameEvent`
    - `Texture2DGameEvent`
    - `Vector2GameEvent`
    - `Vector3GameEvent`

これらは、「PlayerDied」（`GameEvent`）、「ItemCollected」（アイテム名付きの `StringGameEvent`）、「PlaySound」（`AudioClipGameEvent`）などのイベントをブロードキャストするのに役立ちます。

### Variables

一般的に使用される変数型は、`Assets > Create > SOAR > Variables` で利用できます。

-   **プリミティブ型**:
    - `BoolVariable`
    - `ByteVariable`
    - `DoubleVariable`
    - `FloatVariable`
    - `IntVariable`
    - `LongVariable`
    - `StringVariable`
-   **Unity 型**:
    - `CameraVariable`
    - `ColorVariable`
    - `GameObjectVariable`
    - `PoseVariable`
    - `QuaternionVariable`
    - `TransformVariable`
    - `Vector2Variable`
    - `Vector3Variable`

これらの変数は、プレイヤーの体力（`FloatVariable`）、スコア（`IntVariable`）、プレイヤー名（`StringVariable`）などの共有データの変更を保存および反応するために使用できます。

### Collections (Lists)

SOAR は、`Assets > Create > SOAR > Lists` で事前定義されたリストコレクションを提供します。

-   **プリミティブ型**:
    - `BoolList`
    - `ByteList`
    - `DoubleList`
    - `FloatList`
    - `IntList`
    - `LongList`
    - `StringList`
-   **Unity 型**:
    - `AudioClipList`
    - `ColorList`
    - `PoseList`
    - `QuaternionList`
    - `SpriteList`
    - `Vector2List`
    - `Vector3List`

これらのリストは、アクティブな敵のリスト（`GameObjectList`）、曲のプレイリスト（`AudioClipList`）、チュートリアル手順のシーケンス（`StringList`）など、動的なアイテムのコレクションを管理するのに役立ちます。

!!! Note "辞書はカスタムクラスのみ"
    現在、基本クラスとして提供されているのは `List` コレクションのみです。`Dictionary` コレクションの場合は、`SoarDictionary<TKey, TValue>` を継承するカスタムクラスを作成してください。

### Transactions

基本の `Transaction` 型は、`Assets > Create > SOAR > Transactions` で利用できます。

-   **パラメータなし**:
    - `Transaction` (単純なリクエスト/レスポンスシグナル用)
-   **対称型**:
    - `BoolTransaction`
    - `ByteTransaction`
    - `DoubleTransaction`
    - `FloatTransaction`
    - `IntTransaction`
    - `LongTransaction`
    - `PoseTransaction`
    - `QuaternionTransaction`
    - `StringTransaction`
    - `Vector2Transaction`
    - `Vector3Transaction`

これらのトランザクションは、リクエストとレスポンスの両方に同じ型を使用します（例：`IntTransaction` は `Transaction<int, int>` です）。これらは、値を受け取り、処理し、同じ型の変更された値を返す操作に役立ちます。非対称型の場合は、必要に応じて `Transaction<TRequest, TResponse>` を継承するカスタムクラスを作成してください。

## カスタムクラスの作成

SOAR は幅広い基本クラスを提供していますが、プロジェクト固有のカスタムデータ型がしばしば必要になります。このプロセスは簡単で、すべての SOAR 機能で同じパターンに従います。

たとえば、カスタムの `PlayerData` 構造体の `Variable` を作成するには：

1.  **データ型の定義**: カスタムデータ型が `[Serializable]` であることを確認します。

    ```csharp
    // File: PlayerData.cs
    using System;

    [Serializable]
    public struct PlayerData
    {
        public string name;
        public int level;
        public float health;
    }
    ```

2.  **Variable クラスの作成**: `Variable<T>` を継承する新しいクラスを作成します。

    ```csharp
    // File: PlayerDataVariable.cs
    using Soar;
    using Soar.Variables;
    using UnityEngine;

    [CreateAssetMenu(fileName = "PlayerDataVariable", menuName = MenuHelper.DefaultVariableMenu + "Player Data Variable")]
    public class PlayerDataVariable : Variable<PlayerData> { }
    ```

3.  **アセットの作成**: `Assets > Create > SOAR > Variables > Player Data Variable` メニューから `PlayerDataVariable` アセットを作成します。

この同じプロセスは、カスタムの `Command`、`GameEvent`、`Collection`、および `Transaction` の作成にも適用されます。
