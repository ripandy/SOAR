# SOAR

Scriptable Object Architecture Reactive-extensible (SOAR) is an implementation of the Scriptable Object Architecture.
Scriptable Object Architecture intends to provide clean and decoupled code architecture.
SOAR's implementation is based on [Ryan Hipple's talk at Unite Austin 2017](https://youtu.be/raQ3iHhE_Kk).

SOAR is an event-based system that encourages the use of the Pub/Sub pattern.
Its fundamental principle involves treating the [ScriptableObject] instance (with its `name`/`guid` property) as a 'Channel' or 'Topic'.
Pairing between publisher and subscriber are established through references to each of SOAR's instance.

SOAR is designed to be extensible with the Reactive library [R3], a feature-rich and modern Reactive Extensions for C#.
SOAR wraps and utilizes R3's feature within the Scriptable Object Architecture.
SOAR can function independently, but its implementation provides only basic functionality.
It is highly recommended to use SOAR in conjunction with R3.

## Key Links

- [SOAR] - Access to the GitHub repository.
- [R3] - The new future of dotnet/reactive and UniRx. Developed by Cysharp, Inc.
- [Kassets] - SOAR's predecessor, developed by Kadinche Corp. An implementation of Scriptable Object Architecture extensible with UniRx and UniTask.
- [Unite 2017 Talk by Ryan Hipple](https://youtu.be/raQ3iHhE_Kk) - The original inspiration.

[SOAR]: https://github.com/ripandy/SOAR
[R3]: https://github.com/Cysharp/R3
[Kassets]: https://github.com/kadinche/Kassets
[ScriptableObject]: https://docs.unity3d.com/Manual/class-ScriptableObject.html
