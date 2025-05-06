# SOAR

Scriptable Object Architecture Reactive-extensible (SOAR) is an implementation of the [Scriptable Object Architecture](https://docs.unity3d.com/Manual/class-ScriptableObject.html).
Scriptable Object Architecture aims to provide a clean and decoupled architecture.
SOAR is based on [Ryan Hipple's talk at Unite Austin 2017](https://youtu.be/raQ3iHhE_Kk).

SOAR is an event-based system that encourages the use of the Pub/Sub pattern.
Its fundamental principle involves treating the combination of a [ScriptableObject] instance and its `name` property as a 'Key' or 'Topic'.
This 'Key' is then used to establish a pair between a publisher and a subscriber.


SOAR is designed to be extensible with the Reactive library [R3], a feature-rich and modern Reactive Extensions for C#.
SOAR seeks to wrap and utilize R3's features within the Scriptable Object Architecture.
While SOAR can function independently, its standalone implementation offers minimal functionality.
Therefore, using SOAR in conjunction with R3 is highly recommended.

## Key Links

- [SOAR] - Access to the GitHub repository.
- [R3] - The new future of dotnet/reactive and UniRx. Developed by Cysharp, Inc.
- [Kassets] - SOAR's predecessor, developed by Kadinche Corp. An implementation of Scriptable Object Architecture extensible with UniRx and UniTask.
- [Unite 2017 Talk by Ryan Hipple](https://youtu.be/raQ3iHhE_Kk) - The original inspiration.

[SOAR]: https://github.com/ripandy/SOAR
[R3]: https://github.com/Cysharp/R3
[Kassets]: https://github.com/kadinche/Kassets
[ScriptableObject]: https://docs.unity3d.com/Manual/class-ScriptableObject.html
