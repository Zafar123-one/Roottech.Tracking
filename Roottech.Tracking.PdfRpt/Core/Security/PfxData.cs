﻿using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;

namespace Roottech.Tracking.PdfRpt.Core.Security
{
    /// <summary>
    /// A Personal Information Exchange File Info
    /// </summary>
    public class PfxData
    {
        /// <summary>
        /// Represents an X509 certificate
        /// </summary>
        public X509Certificate[] X509PrivateKeys { set; get; }

        /// <summary>
        /// Certificate's public key
        /// </summary>
        public ICipherParameters PublicKey { set; get; }
    }
}
