# Transaction Sample

## Overview

This sample demonstrates how to use a `Transaction<TRequest, TResponse>` to manage an asynchronous request-response cycle between two objects.

## Scene Setup

The `TransactionSample` scene contains a button to initiate a request and several text labels to display the status of the transaction.

## How it Works

This sample demonstrates the full request-response-subscribe lifecycle:

1.  **`FloatTransaction` Asset:** A `Transaction<float, float>` asset is used. It takes a `float` as a request and is expected to return a `float` as a response.
2.  **`ResponseSample.cs`:** This component acts as the **responder**. It registers an asynchronous response handler to the `FloatTransaction`. When a request is received, it simulates a long-running, non-blocking operation (using `Task.Delay`) and returns the time elapsed as the response value.
3.  **`RequestSample.cs`:** This component acts as the **requester**. When the UI button is clicked, it calls `RequestAsync()` on the `FloatTransaction`, passing a value. The UI is disabled until the `Task` completes, at which point the UI is re-enabled and the response value is displayed.
4.  **`SubscribeToRequestSample.cs` & `SubscribeToResponseSample.cs`:** These components act as **observers**. They are not part of the request-response flow but listen to the transaction to update their own UI labels when a request is sent and when a response is received.

## Key Concepts

*   **Asynchronous Operations:** Transactions are designed for handling asynchronous operations, where a requester can send a request and get a response later without blocking the main thread.
*   **Decoupling:** The requester (`RequestSample`) and the responder (`ResponseSample`) do not have direct references to each other. They communicate solely through the `FloatTransaction` asset.
*   **Observation:** Other systems can observe the request-response cycle without participating in it, allowing for flexible and scalable architectures.
