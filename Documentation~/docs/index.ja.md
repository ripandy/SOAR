# SOAR

Scriptable Object Architecture Reactive-extensible (SOAR) は、Unity 向けのモジュール式フレームワークであり、イベント駆動型アーキテクチャに ScriptableObject を活用します。
これは、クリーンで疎結合なコードアーキテクチャを提供することを目的とした、Scriptable Object Architecture の実装です。
SOAR の実装は、[Ryan Hipple 氏の Unite Austin 2017 での講演](https://youtu.be/raQ3iHhE_Kk)に基づいています。

SOAR は、Pub/Sub パターンの使用を推奨するイベントベースのシステムです。
その基本原則は、[ScriptableObject] インスタンス（およびその `name`/`guid` プロパティ）を「Channel」または「Topic」として扱うことです。
パブリッシャーとサブスクライバー間のペアリングは、各 SOAR インスタンスへの参照を通じて確立されます。

SOAR は、機能豊富でモダンな C# 向けリアクティブエクステンションであるリアクティブライブラリ [R3] で拡張できるように設計されています。
SOAR は、Scriptable Object Architecture 内で R3 の機能をラップして利用します。
SOAR は独立して機能することもできますが、その実装は基本的な機能のみを提供します。
SOAR を R3 と組み合わせて使用することを強くお勧めします。

## 主要リンク

- [SOAR] - GitHub リポジトリへのアクセス。
- [R3] - Cysharp, Inc. によって開発された、dotnet/reactive と UniRx の新しい未来。
- [Kassets] - SOAR の前身であり、Kadinche Corp. によって開発されました。UniRx と UniTask で拡張可能な Scriptable Object Architecture の実装。
- [Unite 2017 で Ryan Hipple 氏による講演](https://youtu.be/raQ3iHhE_Kk) - 元々のインスピレーション。

[SOAR]: https://github.com/ripandy/SOAR
[R3]: https://github.com/Cysharp/R3
[Kassets]: https://github.com/kadinche/Kassets
[ScriptableObject]: https://docs.unity3d.com/Manual/class-ScriptableObject.html
