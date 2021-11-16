﻿using Centaurus.Models;

namespace Centaurus
{
    public static class ByteArraySignatureExtensions
    {

        /// <summary>
        /// Signs data and returns signature object
        /// </summary>
        /// <param name="binaryData">Data to sign</param>
        /// <param name="keyPair">KeyPair to sign the data.</param>
        /// <returns></returns>
        public static TinySignature Sign(this byte[] binaryData, KeyPair keyPair)
        {
            var rawSignature = keyPair.Sign(binaryData);
            return new TinySignature
            {
                Data = rawSignature
            };
        }
    }
}
