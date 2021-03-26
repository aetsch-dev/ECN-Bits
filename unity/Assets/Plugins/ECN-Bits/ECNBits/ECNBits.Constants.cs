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
    /// <summary>
    /// Constants defined in the ws2/inc/ecn-bitw.h header file
    /// </summary>
    public static class Constants
    {
        public const ushort ECNBITS_INVALID_BIT = (ushort)0x0100U;
        public const ushort ECNBITS_ISVALID_BIT = (ushort)0x0200U;

        public const byte ECNBITS_NON = 0;
        public const byte ECNBITS_ECT0 = 2;
        public const byte ECNBITS_ECT1 = 1;
        public const byte ECNBITS_CE = 3;

        public const string UNKNOWN = "??ECN?";

        public static readonly string[] ECNBITS_MEANINGS =
        {
            "nōn-ECN-capable transport",
            "ECN-capable; L4S: L4S-aware transport",
            "ECN-capable; L4S: legacy transport",
            "congestion experienced"
        };

        public static readonly string[] ECNBITS_SHORTNAMES =
        {
            "no ECN",
            "ECT(1)",
            "ECT(0)",
            "ECN CE"
        };
    }
}