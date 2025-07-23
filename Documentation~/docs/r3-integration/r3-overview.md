# R3 Integration: Overview

SOAR is designed to be highly extensible with [R3 (Reactive Extensions for C#)](https://github.com/Cysharp/R3), a modern and feature-rich reactive programming library. When R3 is present in the project, SOAR automatically integrates with it, unlocking a powerful set of features for asynchronous programming, event handling, and data stream manipulation.

The integration with R3 enhances SOAR's core features by:

-   **Exposing `Observable` Streams**: SOAR objects like `GameEvent`s, `Variable`s, `Collection`s, and `Transaction`s expose their events as R3 `Observable` streams, allowing for complex event processing with LINQ-style operators.
-   **Providing `async/await` Support**: Awaiting events and transactions becomes seamless with `async/await`.
-   **Advanced Concurrency Control**: For `Transaction`s, R3 provides fine-grained control over how concurrent requests are handled.

## Enabling the Integration

To enable the R3 integration, simply install R3 into the Unity project. SOAR will automatically detect its presence via the `SOAR_R3` scripting define symbol, which is typically added by the R3 package installation.

If the `SOAR_R3` symbol is not automatically defined, it can be added manually in `Project Settings > Player > Other Settings > Scripting Define Symbols`.
