using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plugins.ECNBits;
using Plugins.ECNBits.Android;
using UnityEngine;
using TMPro;

using static Plugins.ECNBits.ECNBits_Utils;
using static Plugins.ECNBits.Constants;

namespace Scripts
{
    public class AndroidEcnClientSample : MonoBehaviour
    {
        private bool running = false;
        public string host;
        public int port;
        public string message;

        public TextMeshProUGUI TextField;
        
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        public void OnStart()
        {
            if (running) return;
            if (string.IsNullOrWhiteSpace(host))
                return;

            TextField.text = "";
            var cancellationToken = tokenSource.Token;
            var augmentedMessage = $"{message}-from-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            Task.Run(() => SocketTask(host, port, augmentedMessage, cancellationToken));
        }

        private async Task SocketTask(string host, int port, string message, CancellationToken token)
        {
            const uint BUFFER_SIZE = 512;
            
            // Make sure the task will be run on the ThreadPool and not on the Unity UI thread
            await Task.Yield();

            var buffer = IntPtr.Zero;
            try
            {
                running = true;
                var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
                Api.PrepareSocket(socket);
                socket.Connect(host, port);
                Debug.Log(
                    $"Connected to server! LocalEndpoint<{socket.LocalEndPoint}>, RemoteEndpoint<{socket.RemoteEndPoint}>!");
                TextField.text = "Connected to server!\n";
                
                socket.ReceiveTimeout = 50;
                Debug.Log($"Modified socket timeout: {socket.ReceiveTimeout}!");
                TextField.text += $"Modified socket timeout: {socket.ReceiveTimeout}!\n";


                var messageAsBytes = Encoding.ASCII.GetBytes(message);

                /*
                var messagePointer = Marshal.AllocHGlobal(messageAsBytes.Length);
                Marshal.Copy(messageAsBytes, 0, messagePointer, messageAsBytes.Length);
                var preparedMessage =
                    (IntPtr) Api.MakeCMessageHeader(messagePointer, (uint)messageAsBytes.Length, AddressFamily.InterNetwork, Constants.ECNBITS_NON);
                Marshal.FreeHGlobal(messagePointer);
                */

                socket.Send(messageAsBytes, 0, messageAsBytes.Length, SocketFlags.None, out var errorCode);
                Debug.Log($"Sent message<{message}> to server...  ErrorCode:{errorCode}!");
                TextField.text += $"Sent message<{message}> to server...  ErrorCode:{errorCode}!\n";
                
                buffer = Marshal.AllocHGlobal((int) BUFFER_SIZE);
                ushort ecnResult = 0;
                
                while (true)
                {
                    // If cancellation was requested, break out of the while loop
                    token.ThrowIfCancellationRequested();

                    // Read data using the ECNBits Api
                    var readBits = Api.Receive(socket, buffer, BUFFER_SIZE, 0, ref ecnResult);
                    var isValid = IsValid(ecnResult);

                    Debug.Log($"Read {readBits} bits. EcnResult:{ConvertToBitString(ecnResult, 16)}!");
                    TextField.text += $"Read {readBits} bits. EcnResult:{ConvertToBitString(ecnResult, 16)}!\n";
                    Debug.Log($"EcnResult validity:{isValid}, shortname:{GetDescription(ecnResult)}");
                    TextField.text += $"EcnResult validity:{isValid}, shortname:{GetDescription(ecnResult)}\n";

                    // If the read ECNBits are valid, log their state
                    if (isValid)
                    {
                        var ecnBits = GetBits(ecnResult);
                        Debug.Log( $"ECNBits:<{ConvertToBitString(ecnBits, 2)}>, meaning:<{Constants.ECNBITS_MEANINGS[ecnBits]}>");
                        TextField.text +=
                            $"ECNBits:<{ConvertToBitString(ecnBits, 2)}>, meaning:<{Constants.ECNBITS_MEANINGS[ecnBits]}>\n";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                running = false;
                Marshal.FreeHGlobal(buffer);
            }
        }
        
        public static string ConvertToBitString(ushort bits, uint pad) => Convert.ToString(bits, 2).PadLeft((int)pad, '0');
        
        private void OnDestroy()
        {
            tokenSource.Cancel();
            tokenSource.Dispose();
        }
    }
}