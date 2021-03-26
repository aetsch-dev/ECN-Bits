// Copyright © 2021
//      Mihail Luchian <m.luchian@tarent.de>
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

namespace Plugins.ECNBits
{
    public static class ECNBits_Utils
    {
        public static bool IsValid(ushort ecnResult) => (ecnResult >> 8) == 0x02U;
        public static ushort GetBits(ushort ecnResult) => (ushort)(ecnResult & 0x03U);
        public static ushort GetDscp(ushort ecnResult) => (ushort)(ecnResult & 0xFCU);
        public static string GetDescription(ushort ecnResult) =>
            IsValid(ecnResult)
                ? Constants.ECNBITS_SHORTNAMES[GetBits(ecnResult)]
                : Constants.UNKNOWN;

        public static bool PrepFatal(int rv) => rv >= 2;
    }
}