# Transaction Sample

This sample demonstrates the use of the `Transaction<TRequest, TResponse>` feature.

## Scene

The `TransactionSample` scene contains a button to initiate a request and several text labels to display the status of the transaction.

## How it Works

This sample demonstrates the full request-response-subscribe lifecycle:

1.  **`FloatTransaction` Asset:** A `Transaction<float, float>` asset is used. It takes a `float` as a request and is expected to return a `float` as a response.
2.  **`ResponseSample.cs`:** This component acts as the **responder**. It registers a response handler to the `FloatTransaction`. When a request is received, it performs a long-running (blocking) calculation to simulate work and returns the time elapsed as the response value.
3.  **`RequestSample.cs`:** This component acts as the **requester**. When the UI button is clicked, it calls `Request()` on the `FloatTransaction`, passing a value and a callback method. The UI is disabled until the callback (`OnResponse`) is invoked, at which point the UI is re-enabled and the response value is displayed.
4.  **`SubscribeToRequestSample.cs` & `SubscribeToResponseSample.cs`:** These components act as **observers**. They are not part of the request-response flow but listen to the transaction to update their own UI labels when a request is sent and when a response is received.

This sample showcases how to decouple a requester from a responder, and how other systems can observe the communication without participating in it.
