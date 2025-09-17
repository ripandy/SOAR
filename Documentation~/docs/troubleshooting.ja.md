# トラブルシューティングと FAQ

このページには、一般的な問題の解決策とよくある質問への回答が含まれています。

## 一般的な問題

### `UnityEventBinder` が起動しない

1.  **`GameEvent` の参照を確認する：** `UnityEventBinder` のインスペクターの `Game Event To Listen` フィールドに正しい `GameEvent` アセットをドラッグしたことを確認してください。
2.  **パブリッシャーを確認する：** 別のスクリプトが*まったく同じ* `GameEvent` アセットで `Raise()` を実際に呼び出していますか？ これをテストするには、プロジェクトウィンドウで `GameEvent` アセットを選択し、再生モード中にインスペクターの「Raise」ボタンをクリックします。バインダーが起動する場合、問題はパブリッシングスクリプトにあります。起動しない場合、問題はバインダーの設定にあります。
3.  **`[ExecuteInEditMode]` を確認する：** 編集モードでバインダーを動作させようとしている場合は、リスニングコンポーネントに `[ExecuteAlways]` または `[ExecuteInEditMode]` 属性があることを確認してください。

## よくある質問

### SOAR を使用することのパフォーマンスへの影響は何ですか？

ほとんどのユースケースでは、SOAR のパフォーマンスオーバーヘッドはごくわずかです。`GameEvent` を発生させることは、本質的にメソッド呼び出しとサブスクライバーのリストをループ処理することです。これは直接のメソッド呼び出しよりもわずかに遅いですが、疎結合のアーキテクチャ上の利点は、ほとんどの場合、直接参照のマイクロ最適化を上回ります。

フレームごとに数百回または数千回イベントを発生させている場合、パフォーマンスが考慮事項になる可能性があります。このような高頻度のシナリオでは、その特定のシステムに対してより直接的なアプローチを検討するかもしれませんが、これらのケースはまれです。

### R3 (Reactive Extensions) ライブラリなしで SOAR を使用できますか？

はい。SOAR は独立して機能するように設計されています。コア機能（`GameEvent`、`Variable`、`Collection` など）には、標準の C# イベント（`Action`）を使用する基本実装があります。

ただし、R3 との連携は、非同期プログラミングや複雑なイベント処理（イベントストリームのフィルタリング、マージ、スロットリングなど）に大きな利点をもたらします。単純なイベントを超えるものには強くお勧めします。

### `GameEvent` で複数のパラメータを渡すにはどうすればよいですか？

`GameEvent<T>` は1つのパラメータ（`T`）しか運ぶことができません。
複数の値を渡す必要がある場合は、カスタムの `struct` または `class` にカプセル化する必要があります。
このカプセル化の概念は、[データ転送オブジェクト (DTO)](https://en.wikipedia.org/wiki/Data_transfer_object) としても知られています。

**例：**
```csharp
// データを保持する構造体を定義する
[System.Serializable]
public struct PlayerDamagedData
{
    public float DamageAmount;
    public DamageType Type;
    public GameObject Instigator;
}

// その構造体の GameEvent を作成する
[CreateAssetMenu(menuName = "SOAR/Game Events/Player Damaged Event")]
public class PlayerDamagedEvent : GameEvent<PlayerDamagedData> { }
```
