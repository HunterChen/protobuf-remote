using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ProtoBuf;

namespace ProtoBufRemote
{
    /// <summary>
    /// An RpcChannel that communicates over a Stream. Does not work with any Stream, it must be a bidrectional channel
    /// like NetworkStream.
    /// </summary>
    public class StreamRpcChannel : RpcChannel
    {
        private readonly Queue<RpcMessage> queuedMessages = new Queue<RpcMessage>();
        private readonly Mutex queueMutex = new Mutex();
        private readonly AutoResetEvent queueEvent = new AutoResetEvent(false);
        private readonly ManualResetEvent closeEvent = new ManualResetEvent(false);
        private readonly Thread readThread;
        private readonly Thread writeThread;
        private readonly Stream readStream;
        private readonly Stream writeStream;


        public StreamRpcChannel(RpcController controller, Stream readStream, Stream writeStream)
            : base(controller)
        {
            this.readStream = readStream;
            this.writeStream = writeStream;
            readThread = new Thread(ReadRun);
            writeThread = new Thread(WriteRun);
        }

        public override void Start()
        {
            readThread.Start();
            writeThread.Start();
        }

        /// <summary>
        /// Closes all the streams and requests that the channel shutdown, then joins the channel threads until they
        /// have terminated.
        /// </summary>
        public void CloseAndJoin(bool isCloseWriteStream = true)
        {
            //readStream must be closed, it's the only way we can terminate the readThread cleanly. Even async reads
            //don't support cancellation
            readStream.Close();
            if (isCloseWriteStream)
                writeStream.Close();
            closeEvent.Set();
            readThread.Join();
            writeThread.Join();
        }

        internal override void Send(RpcMessage message)
        {
            queueMutex.WaitOne();

            queuedMessages.Enqueue(message);
            queueEvent.Set();

            queueMutex.ReleaseMutex();
        }

        private void ReadRun()
        {
            byte[] buffer = new byte[1024];
            int bytesRemaining = sizeof(int);
            int bufferPos = 0;
            bool isReadingSize = true;

            while (true)
            {
                int bytesRead;
                try
                {
                    bytesRead = readStream.Read(buffer, bufferPos, bytesRemaining);
                    if (bytesRead == 0)
                        break;
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                catch (IOException)
                {
                    break;
                }

                bufferPos += bytesRead;
                bytesRemaining -= bytesRead;

                if (bytesRemaining == 0)
                {
                    if (isReadingSize)
                    {
                        bytesRemaining = BitConverter.ToInt32(buffer, 0);
                        bufferPos = 0;
                        isReadingSize = false;

                        if (bytesRemaining > buffer.Length)
                            buffer = new byte[bytesRemaining];
                    }
                    else
                    {
                        var memStream = new MemoryStream(buffer, 0, bufferPos);
                        RpcMessage message = Serializer.Deserialize<RpcMessage>(memStream);
                        Receive(message);

                        isReadingSize = true;
                        bytesRemaining = sizeof(int);
                        bufferPos = 0;
                    }
                }
            }
        }

        private void WriteRun()
        {
            var waitHandles = new WaitHandle[] { queueEvent, closeEvent };
            bool isTerminated = false;
            while (!isTerminated)
            {
                int waitIndex = WaitHandle.WaitAny(waitHandles);

                queueMutex.WaitOne();
                while (queuedMessages.Count > 0)
                {
                    RpcMessage message = queuedMessages.Dequeue();
                    try
                    {
                        Serializer.SerializeWithLengthPrefix(writeStream, message, PrefixStyle.Fixed32);
                    }
                    catch (InvalidOperationException)
                    {
                        isTerminated = true;
                        break;
                    }
                    catch (IOException)
                    {
                        isTerminated = true;
                        break;
                    }
                }
                queueMutex.ReleaseMutex();

                if (waitIndex == 1)
                    break;
            }

            writeStream.Flush();
        }
    }
}
