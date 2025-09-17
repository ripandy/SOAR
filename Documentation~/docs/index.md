# SOAR

Scriptable Object Architecture Reactive-extensible (SOAR) is a modular Unity framework that uses ScriptableObjects to create a clean, decoupled, event-driven architecture. It is based on [Ryan Hipple's talk at Unite Austin 2017](https://youtu.be/raQ3iHhE_Kk).

SOAR encourages a publish/subscribe pattern where `ScriptableObject` assets act as channels. Publishers and subscribers interact through these shared assets, eliminating direct dependencies.

While SOAR can function independently, it is designed to be extended with [R3](https://github.com/Cysharp/R3), a modern Reactive Extensions library for C#. Using R3 with SOAR is highly recommended to unlock its full potential.

## Key Links

- [SOAR] - Access to the GitHub repository.
- [R3] - The new future of dotnet/reactive and UniRx. Developed by Cysharp, Inc.
- [Kassets] - SOAR's predecessor, developed by Kadinche Corp. An implementation of Scriptable Object Architecture extensible with UniRx and UniTask.
- [Unite 2017 Talk by Ryan Hipple](https://youtu.be/raQ3iHhE_Kk) - The original inspiration.

[SOAR]: https://github.com/ripandy/SOAR
[R3]: https://github.com/Cysharp/R3
[Kassets]: https://github.com/kadinche/Kassets
[ScriptableObject]: https://docs.unity3d.com/Manual/class-ScriptableObject.html
