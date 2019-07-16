using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Symbols;
using Microsoft.Diagnostics.Tools.RuntimeClient;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Etlx;
using Microsoft.Diagnostics.Tracing.Stacks;

namespace Repro
{
    class Program
    {
        static string[] _helpFlags = { "-h", "-?", "/h", "/?" };
        static int Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 1 || (args.Length == 1 && _helpFlags.Contains(args[0])))
            {
                PrintUsage();
                return -1;
            }

            var pid = int.Parse(args[0]);
            ulong eventPipeSessionId = 0;

            var tempNetTraceFilename = Path.GetRandomFileName() + ".nettrace";
            var tempEtlxFilename = "";

            try
            {
                var providers = new List<Provider>()
                {
                    new Provider("Microsoft-DotNETCore-SampleProfiler")
                };

                var configuration = new SessionConfiguration(
                    circularBufferSizeMB: 10,
                    format: EventPipeSerializationFormat.NetTrace,
                    providers: providers
                );

                var eventPipeStream = EventPipeClient.CollectTracing(pid, configuration, out eventPipeSessionId);
                if (eventPipeSessionId == 0)
                {
                    throw new Exception("Failed to create session...");
                }

                var sawStacks = new ManualResetEvent(false);

                var readerTask = new Task(() =>
                {
                    using (var file = File.OpenWrite(tempNetTraceFilename))
                    {
                        int nBytes = 0;
                        int b = 0;
                        while ((b = eventPipeStream.ReadByte()) != -1)
                        {
                            nBytes++;
                            file.WriteByte((byte)b);
                            if (nBytes > 10)
                                sawStacks.Set();
                        }
                    }
                });

                readerTask.Start();
                sawStacks.WaitOne();
                EventPipeClient.StopTracing(pid, eventPipeSessionId);
                readerTask.Wait();

                tempEtlxFilename = TraceLog.CreateFromEventPipeDataFile(tempNetTraceFilename);
                using (var symbolReader = new SymbolReader(System.IO.TextWriter.Null) { SymbolPath = SymbolPath.MicrosoftSymbolServerPath })
                using (var eventLog = new TraceLog(tempEtlxFilename))
                {
                    var stackSource = new MutableTraceEventStackSource(eventLog)
                    {
                        OnlyManagedCodeStacks = true
                    };

                    var computer = new SampleProfilerThreadTimeComputer(eventLog, symbolReader);
                    computer.GenerateThreadTimeStacks(stackSource);

                    var samplesForThread = new Dictionary<string, List<StackSourceSample>>();

                    stackSource.ForEach((sample) =>
                    {
                        var stackIndex = sample.StackIndex;
                        while (!stackSource.GetFrameName(stackSource.GetFrameIndex(stackIndex), false).StartsWith("Thread ("))
                            stackIndex = stackSource.GetCallerIndex(stackIndex);
                        
                        var threadName = stackSource.GetFrameName(stackSource.GetFrameIndex(stackIndex), false);
                        if (samplesForThread.TryGetValue(threadName, out var samples))
                        {
                            samples.Add(sample);
                        }
                        else
                        {
                            samplesForThread[threadName] = new List<StackSourceSample>() { sample };
                        }
                    });

                    foreach (var (threadName, samples) in samplesForThread)
                    {
                        PrintStack(threadName, samples[0], stackSource);
                    }
                }
                return 0;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
            finally
            {
                if (File.Exists(tempNetTraceFilename))
                    File.Delete(tempNetTraceFilename);
                if (File.Exists(tempEtlxFilename))
                    File.Delete(tempEtlxFilename);
            }
        }

        private static void PrintStack(string threadName, StackSourceSample stackSourceSample, StackSource stackSource)
        {
            Console.WriteLine($"Stack for {threadName}:");
            var stackIndex = stackSourceSample.StackIndex;
            while (!stackSource.GetFrameName(stackSource.GetFrameIndex(stackIndex), false).StartsWith("Thread ("))
            {
                Console.WriteLine($"  {stackSource.GetFrameName(stackSource.GetFrameIndex(stackIndex), false)}");
                stackIndex = stackSource.GetCallerIndex(stackIndex);
            }
            Console.WriteLine();
        }

        static void PrintUsage()
        {
            Console.WriteLine("DotStack Usage:");
            Console.WriteLine("\tdotstack <pid>");
        }
    }
}