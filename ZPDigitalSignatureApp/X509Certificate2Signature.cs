using iTextSharp.text.pdf.security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ZPDigitalSignatureApp
{
    public class X509Certificate2Signature : IExternalSignature
    {
        private String hashAlgorithm;
        private String encryptionAlgorithm;
        private X509Certificate2 certificate;

        public X509Certificate2Signature(X509Certificate2 certificate, String hashAlgorithm)
        {
            if (!certificate.HasPrivateKey)
                throw new ArgumentException("No private key.");
            this.certificate = certificate;
            this.hashAlgorithm = DigestAlgorithms.GetDigest(DigestAlgorithms.GetAllowedDigests(hashAlgorithm));
            if (certificate.PrivateKey is RSACryptoServiceProvider)
                encryptionAlgorithm = "RSA";
            else if (certificate.PrivateKey is DSACryptoServiceProvider)
                encryptionAlgorithm = "DSA";

            else if (certificate.PrivateKey is System.Security.Cryptography.RSACng)
                encryptionAlgorithm = "RSA";
        }

        public virtual byte[] Sign(byte[] message)
        {
            if (certificate.PrivateKey is RSACryptoServiceProvider)
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificate.PrivateKey;
                return rsa.SignData(message, hashAlgorithm);
            }
            else if (certificate.PrivateKey is System.Security.Cryptography.RSACng)
            {
                System.Security.Cryptography.RSACng rSACng = (System.Security.Cryptography.RSACng)certificate.PrivateKey;
                return rSACng.SignData(message, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }

            else
            {
                DSACryptoServiceProvider dsa = (DSACryptoServiceProvider)certificate.PrivateKey;
                return dsa.SignData(message);
            }
        }

        public virtual String GetHashAlgorithm()
        {
            return hashAlgorithm;
        }

        public virtual String GetEncryptionAlgorithm()
        {
            return encryptionAlgorithm;
        }
    }
}