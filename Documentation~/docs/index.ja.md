# SOAR

Scriptable Object Architecture Reactive-extensible (SOAR) は、クリーンで疎結合なイベント駆動型アーキテクチャを構築するために ScriptableObject を活用する、Unity 向けのモジュール式フレームワークです。これは、[Ryan Hipple 氏の Unite Austin 2017 での講演](https://youtu.be/raQ3iHhE_Kk)に基づいています。

SOAR は、`ScriptableObject` アセットがチャネルとして機能する、出版/購読パターンを推奨します。パブリッシャーとサブスクライバーは、これらの共有アセットを介して相互作用し、直接的な依存関係を排除します。

SOAR は独立して機能することもできますが、C# 向けのモダンなリアクティブエクステンションライブラリである [R3](https://github.com/Cysharp/R3) で拡張できるように設計されています。SOAR のポテンシャルを最大限に引き出すために、R3 と組み合わせて使用することを強くお勧めします。

## 主要リンク

- [SOAR] - GitHub リポジトリへのアクセス。
- [R3] - Cysharp, Inc. によって開発された、dotnet/reactive と UniRx の新しい未来。
- [Kassets] - SOAR の前身であり、Kadinche Corp. によって開発されました。UniRx と UniTask で拡張可能な Scriptable Object Architecture の実装。
- [Unite 2017 で Ryan Hipple 氏による講演](https://youtu.be/raQ3iHhE_Kk) - 元々のインスピレーション。

[SOAR]: https://github.com/ripandy/SOAR
[R3]: https://github.com/Cysharp/R3
[Kassets]: https://github.com/kadinche/Kassets
[ScriptableObject]: https://docs.unity3d.com/Manual/class-ScriptableObject.html
