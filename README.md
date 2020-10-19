# DotStack
A sample tool to quickly print out managed stacks for dotnet applications
```
usage: DotStack <pid>
```

Sample usage:
```
$ ./DotStack 93400

Stack for Thread (2207382):
  UNMANAGED_CODE_TIME
  System.Private.CoreLib!System.Threading.ManualResetEventSlim.Wait(int32,value class System.Threading.CancellationToken)
  System.Private.CoreLib!System.Threading.Tasks.Task.SpinThenBlockingWait(int32,value class System.Threading.CancellationToken)
  System.Private.CoreLib!System.Threading.Tasks.Task.InternalWaitCore(int32,value class System.Threading.CancellationToken)
  System.Private.CoreLib!System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(class System.Threading.Tasks.Task)
  System.Private.CoreLib!System.Runtime.CompilerServices.TaskAwaiter.GetResult()
  Microsoft.Extensions.Hosting.Abstractions!Microsoft.Extensions.Hosting.HostingAbstractionsHostExtensions.Run(class Microsoft.Extensions.Hosting.IHost)
  testtesttest!testtesttest.Program.Main(class System.String[])

Stack for Thread (2207415):
  UNMANAGED_CODE_TIME
  System.IO.FileSystem.Watcher!System.IO.FileSystemWatcher+RunningInstance+StaticWatcherRunLoopManager.WatchForFileSystemEventsThreadStart(class System.Threading.ManualResetEventSlim,class Microsoft.Win32.SafeHandles.SafeEventStreamHandle)
  System.IO.FileSystem.Watcher!System.IO.FileSystemWatcher+RunningInstance+StaticWatcherRunLoopManager+<>c.<ScheduleEventStream>b__3_0(class System.Object)
  System.Private.CoreLib!System.Threading.ThreadHelper.ThreadStart_Context(class System.Object)
  System.Private.CoreLib!System.Threading.ExecutionContext.RunInternal(class System.Threading.ExecutionContext,class System.Threading.ContextCallback,class System.Object)
  System.Private.CoreLib!System.Threading.ThreadHelper.ThreadStart(class System.Object)

Stack for Thread (2207469):
  UNMANAGED_CODE_TIME
  System.Private.CoreLib!System.Threading.SemaphoreSlim.WaitUntilCountOrTimeout(int32,unsigned int32,value class System.Threading.CancellationToken)
  System.Private.CoreLib!System.Threading.SemaphoreSlim.Wait(int32,value class System.Threading.CancellationToken)
  System.Collections.Concurrent!System.Collections.Concurrent.BlockingCollection`1[Microsoft.Extensions.Logging.Console.LogMessageEntry].TryTakeWithNoTimeValidation(!0&,int32,value class System.Threading.CancellationToken,class System.Threading.CancellationTokenSource)
  System.Collections.Concurrent!System.Collections.Concurrent.BlockingCollection`1+<GetConsumingEnumerable>d__68[Microsoft.Extensions.Logging.Console.LogMessageEntry].MoveNext()
  Microsoft.Extensions.Logging.Console!Microsoft.Extensions.Logging.Console.ConsoleLoggerProcessor.ProcessLogQueue()
  System.Private.CoreLib!System.Threading.ThreadHelper.ThreadStart_Context(class System.Object)
  System.Private.CoreLib!System.Threading.ExecutionContext.RunInternal(class System.Threading.ExecutionContext,class System.Threading.ContextCallback,class System.Object)
  System.Private.CoreLib!System.Threading.ThreadHelper.ThreadStart()

Stack for Thread (2207474):
  UNMANAGED_CODE_TIME
  System.Private.CoreLib!System.Threading.Thread.Sleep(value class System.TimeSpan)
  Microsoft.AspNetCore.Server.Kestrel.Core!Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure.Heartbeat.TimerLoop()
  Microsoft.AspNetCore.Server.Kestrel.Core!Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure.Heartbeat+<>c.<.ctor>b__8_0(class System.Object)
  System.Private.CoreLib!System.Threading.ThreadHelper.ThreadStart_Context(class System.Object)
  System.Private.CoreLib!System.Threading.ExecutionContext.RunInternal(class System.Threading.ExecutionContext,class System.Threading.ContextCallback,class System.Object)
  System.Private.CoreLib!System.Threading.ThreadHelper.ThreadStart(class System.Object)

Stack for Thread (2207860):
  UNMANAGED_CODE_TIME
  System.Net.Sockets!System.Net.Sockets.SocketAsyncEngine.EventLoop()
  System.Net.Sockets!System.Net.Sockets.SocketAsyncEngine+<>c.<.ctor>b__14_0(class System.Object)
  System.Private.CoreLib!System.Threading.ThreadHelper.ThreadStart(class System.Object)
```

## Publish a single file
The project is setup to use CoreRT to publish a single file that has been AOT compiled.  The final binary is roughly 21 MB.
```
$ dotnet publish -c release -r <RID> -o ./native   # uses CoreRT to AOT compile the app
$ strip ./native/DotStack                          # strips the debug information from the binary, reducing the file size
```