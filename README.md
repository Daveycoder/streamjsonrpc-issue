# streamjsonrpc-issue

Working (or non-working) example I could think of just to show the [Issue #658](https://github.com/microsoft/vs-streamjsonrpc/issues/658) I raised on the repo.

The Version seems to be ignored - setting it to (1. 0) to use JSON-RPC seems to cause issues/crashes when communication is between both Client and Server running StreamJsonRPC.

The issue I currently have is that my client is talking to a JSON-RPC 1.0 service; simply listening for notifications. However, as per [JSON-RPC 1.0](https://www.jsonrpc.org/specification_v1), the `ID` must be present, but null. This is slightly different to [notifications in JSON-RPC 2.0](https://www.jsonrpc.org/specification#notification) which state that the `ID` must not be present.

So...I think when Version is set to 1.0 the following issues occur:
1. When sending a message (either a notifcation from the server, or a request to the server), the message can be sent but contains a `json-rpc` string of `"2.0"`.
2. When the server receives a request, this causes an exception and terminates the stream.
3. When the client recieves a notification, it sends a response (this is the string issue I mentioned in the initial issue comment).

As I've mentioned in the issue, the only reason I noticed this is because I've got to talk to a 1.0 service which isn't expecting a notification to return a response. Interestingly (and totally outside the scope of this) it seems to be okay with the `json-rpc` string - provided it has method and ID it doesn't really care what else is there...but it does care about getting a notification response it shouldn't.

