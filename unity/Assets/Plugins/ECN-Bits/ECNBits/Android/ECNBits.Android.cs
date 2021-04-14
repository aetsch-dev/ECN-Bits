// Copyright © 2021
//      Jan Wollner <jan.wollner@telekom.de>
// Licensor: Deutsche Telekom
//
// Provided that these terms and disclaimer and all copyright notices
// are retained or reproduced in an accompanying document, permission
// is granted to deal in this work without restriction, including un‐
// limited rights to use, publicly perform, distribute, sell, modify,
// merge, give away, or sublicence.
//
// This work is provided “AS IS” and WITHOUT WARRANTY of any kind, to
// the utmost extent permitted by applicable law, neither express nor
// implied; without malicious intent or gross negligence. In no event
// may a licensor, author or contributor be held liable for indirect,
// direct, other damage, loss, or other issues arising in any way out
// of dealing in the work, even if advised of the possibility of such
// damage or existence of a defect, except proven that it results out
// of said person’s immediate fault when using the work as intended.

using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Plugins.ECNBits.Android
{
    public static class Api
    {
        private const string LIB_Name = "ecn-bitw";

        public static int PrepareSocket(Socket socket)
        {
            var addressFamily = socket.AddressFamily switch
            {
                AddressFamily.InterNetwork => 4,
                AddressFamily.InterNetworkV6 => 6,
                _ => 0
            };

            return ecnbits_prep(socket.Handle, addressFamily);
        }

        public static int PrepareSocketWithTos(Socket socket, byte iptos)
        {
            var addressFamily = socket.AddressFamily switch
            {
                AddressFamily.InterNetwork => 4,
                AddressFamily.InterNetworkV6 => 6,
                _ => 0
            };

            return ecnbits_tc(socket.Handle, addressFamily, iptos);
        }

        public static long MakeCMessageHeader(IntPtr buffer, uint bufferLength, AddressFamily addressFamily, byte tc)
        {
            var af = addressFamily switch
            {
                AddressFamily.InterNetwork => 4,
                AddressFamily.InterNetworkV6 => 6,
                _ => 0
            };

            return ecnbits_mkcmsg(buffer, new IntPtr(bufferLength), af, tc).ToInt64();
        }
        
        #region FriendlyWrapper

        public static int SocketToAddressFamily(Socket socket) =>
            ecnbits_stoaf(socket.Handle);
        
        public static long ReadMessage(Socket socket, ref CMessageHeader message, int flags, ref ushort ecnResult) =>
            ecnbits_rdmsg(socket.Handle, ref message, flags, ref ecnResult).ToInt64();

        public static long ReceiveMessage(Socket socket, ref CMessageHeader message, int flags, ref ushort ecnResult) =>
            ecnbits_recvmsg(socket.Handle, ref message, flags, ref ecnResult).ToInt64();
        
        public static long ReceiveFrom(
            Socket socket, IntPtr buffer, uint buflen, int flags,
            ref SockAddr srcAddr, ref int addrLen, ref ushort ecnResult) =>
            ecnbits_recvfrom(socket.Handle, buffer, new UIntPtr(buflen), flags, ref srcAddr, ref addrLen, ref ecnResult).ToInt64();
        
        public static long Receive(Socket socket, IntPtr buffer, uint bufLen, int flags, ref ushort ecnResult) =>
            ecnbits_recv(socket.Handle, buffer, new UIntPtr(bufLen), flags, ref ecnResult).ToInt64();
        
        #endregion
        
        #region native

        #region SocketOperations

        [DllImport(LIB_Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ecnbits_prep(IntPtr socketfd, int af);
        
        [DllImport(LIB_Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ecnbits_tc(IntPtr socketfd, int af, byte iptos);
        
        [DllImport(LIB_Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ecnbits_rdmsg(IntPtr socketfd, ref CMessageHeader msg, int flags, ref ushort ecnresult);

        #endregion

        #region Utilities

        [DllImport(LIB_Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ecnbits_mkcmsg(IntPtr buf, IntPtr size_t, int af, byte tc);

        [DllImport(LIB_Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ecnbits_stoaf(IntPtr socketfd);
        
        #endregion

        #region WrapperCalls

        [DllImport(LIB_Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ecnbits_recvmsg(IntPtr socketfd, ref CMessageHeader msg, int flags,
            ref ushort ecnresult);

        [DllImport(LIB_Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ecnbits_recvfrom(IntPtr socketfd, IntPtr buf,
            UIntPtr buflen, int flags,
            ref SockAddr src_adr, ref int addrlen,
            ref ushort ecnresult);
        
        [DllImport(LIB_Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ecnbits_recv(IntPtr socketfd, IntPtr buf,
            UIntPtr buflen, int flags,
            ref ushort ecnresult);
        
        #endregion
        
        #endregion
        
        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct CMessageHeader
        {
            public UIntPtr length;
            public int level;
            public int type;
        }
        
        /// <summary>
        /// Represents the C# version of the sockaddr struct
        /// <see cref="https://docs.microsoft.com/en-us/windows/win32/winsock/sockaddr-2"/>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SockAddr
        {
            public const int DATA_SIZE = 14;

            public ushort family;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = DATA_SIZE)]
            public byte[] data;

            public SockAddr(ushort family)
            {
                this.family = family;
                data = new byte[DATA_SIZE];
            }
        }

        #endregion
    }   
}