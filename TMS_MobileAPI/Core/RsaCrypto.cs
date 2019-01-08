using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TMS_MobileAPI.Core
{
    public static class RsaCrypto
    {
        //公钥
        private const string PublicRsaKey = @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA1ZOB3VsmkN2FHj7YxZ0F
HEHV51wymLyyyqYtXjnlWDeMbE3jVyd4LPv4qOPPij6jaEZwQELGZqQmjFTN3A4w
GlOCoAT4hd2PU0IkvgCoq7ReBhdxLotbmQal/pzAB45FAcHFF9CJb1LwBGW0CoNL
bwvMyZyhN/UwToDw19p5YXeQhc4gpOHUh0aCSzvxsqV5e5/ibDr52/OUsetbE74y
mDc4NeTAvRQQcrk0/VG+hY9KM86SwXp9/WD6YeCQHgldAgvTsEHFRhQGdd44NZbO
f3iHFDADhvXpoSQYBQoBlaU2ou8jOiVMKxir0u2f9f4mBwkIa0sXxtms2o/a69M0
nQIDAQAB";
        //私钥
        private const string PrivateRsaKey = @"MIIEpQIBAAKCAQEA1ZOB3VsmkN2FHj7YxZ0FHEHV51wymLyyyqYtXjnlWDeMbE3j
Vyd4LPv4qOPPij6jaEZwQELGZqQmjFTN3A4wGlOCoAT4hd2PU0IkvgCoq7ReBhdx
LotbmQal/pzAB45FAcHFF9CJb1LwBGW0CoNLbwvMyZyhN/UwToDw19p5YXeQhc4g
pOHUh0aCSzvxsqV5e5/ibDr52/OUsetbE74ymDc4NeTAvRQQcrk0/VG+hY9KM86S
wXp9/WD6YeCQHgldAgvTsEHFRhQGdd44NZbOf3iHFDADhvXpoSQYBQoBlaU2ou8j
OiVMKxir0u2f9f4mBwkIa0sXxtms2o/a69M0nQIDAQABAoIBABqnfRaWEJsVuk5L
A5yt+vxKzxo/rGTZb+rQkGrpCNFYpWLC2bN/zoS2sZLKU5VqOdSCrfGnI8GdjN4I
m2RyNNikQXIdDLMQMpnucE1yIZrEONPDyzFG3Ric9sxu3ZxTyhLtDt7o/K3zCNdT
pgOF3T0vmNA0hv5H0E6L0wLtViaund446JiMu1RLTpNmnhc0nycFBDHqy/GmVMK9
53yak3W2dF8GjipTc2nyKHrgHvmw8w4imu+9Xu/r1vlCe3eHqvDL4Im/5ooZvuQy
pbJ87sXVrIgAsPHaxbwDYzXIrWjS73Ra7QeWkGIxs5wontQiGmXwY/ZKI0hoVn9u
Aaj+Y4ECgYEA9ldPUdjOYmPuyAyWWkx9TH1LN5zz7o7giJuLDH2dcl4MhfsqOLNf
vvHSvVkkWfcII4735GBlwfxdBpqz7vBwEwU58a/Ty8MIU66lC0UkEv4hKnr2IPGD
ifMOYhd3ctyfbj5NoGwqOFpyzRPzo9n8cO3RqtfOdO410LnLk6Xg/mECgYEA3fNQ
jXXdzVAs2d3UtpxAc/4LMz88mCq65P3pDE8jaMxWw3XjJwM8wwPXFfw8W9R8LDZs
iq/SQ5XCKeRc1vNYi+vkdt/lP6eIPXABbyJB7ZeMz2Jx2b185z7fjiQJlYfHBKrK
+JG11+o/lsLZtxnxajp2kAqMNJd8bN1s4w06x70CgYEAvx7SHXAV/2QE+BQ7+mbp
t5Rl7QnKf0U+kv4KjXBP3VF2aeSJx1/zYtN/awRkHaB5Ig1j6j2X9T70dZFFFJZa
fYdKg2ak7autJC+VQP8tk8ayuKCZoMmAAmEewZ/vPqBI2WFhHJuOzhXuh4l5N6CC
KKN6ThAMz8+Y1Q2xEiZaP0ECgYEArCgY6UDthF2WDH/GiMZ2MAJziZKZW6Z5RTm1
1LhlzNoCyqXcRdx6+wE1inYWE1yj3F9ynbh3Lbkx2/CoGoqyugWFNrfASDmYsZ75
XcyCutn8fLfte2lBQtU/7i8ByByDQJmByoCrPgkSvcvxt9bFrRIf+OZVjk2aU48E
8LDIUTkCgYEA7VAmSoS7oYJ9tCcvAeOGY1VXwyCvxikxzuXoRCz+E8aDz3WlBSfp
rurqnx0rVagGthpWRbDLEbqgIii5lvHwyl1plLtouIikIukp+dJyDbPMRumtLZpa
J2bG0mCtw3XwKgbCd69Se0e7Fh/84bMq/EH8CKgb9WfiwJCbON9zJ3U=";

        private readonly static RSA _privateKeyRsaProvider;
        private readonly static RSA _publicKeyRsaProvider;
        private readonly static HashAlgorithmName _hashAlgorithmName;
        private readonly static Encoding _encoding;



        static RsaCrypto()
        {

            _encoding = Encoding.UTF8;
            _privateKeyRsaProvider = CreateRsaProviderFromPrivateKey(PrivateRsaKey);

            _publicKeyRsaProvider = CreateRsaProviderFromPublicKey(PublicRsaKey);

            _hashAlgorithmName = HashAlgorithmName.SHA256;

        }

       

        /// <summary>
        /// 使用私钥签名
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <returns></returns>
        public static string Sign(string data)
        {
            byte[] dataBytes = _encoding.GetBytes(data);

            var signatureBytes = _privateKeyRsaProvider.SignData(dataBytes, _hashAlgorithmName, RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(signatureBytes);
        }

       

        /// <summary>
        /// 使用公钥验证签名
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="sign">签名</param>
        /// <returns></returns>
        public static bool Verify(string data, string sign)
        {
            byte[] dataBytes = _encoding.GetBytes(data);
            byte[] signBytes = Convert.FromBase64String(sign);

            var verify = _publicKeyRsaProvider.VerifyData(dataBytes, signBytes, _hashAlgorithmName, RSASignaturePadding.Pkcs1);

            return verify;
        }


        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="cipherText">字符串</param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            if (_privateKeyRsaProvider == null)
            {
                throw new Exception("_privateKeyRsaProvider is null");
            }
            return Encoding.UTF8.GetString(_privateKeyRsaProvider.Decrypt(Convert.FromBase64String(cipherText), RSAEncryptionPadding.Pkcs1));
        }

       
        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Encrypt(string text)
        {
            if (_publicKeyRsaProvider == null)
            {
                throw new Exception("_publicKeyRsaProvider is null");
            }
            return Convert.ToBase64String(_publicKeyRsaProvider.Encrypt(Encoding.UTF8.GetBytes(text), RSAEncryptionPadding.Pkcs1));
        }


        /// <summary>
        /// 使用私钥创建RSA实例
        /// </summary>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        private static RSA CreateRsaProviderFromPrivateKey(string privateKey)
        {
            var privateKeyBits = Convert.FromBase64String(privateKey);

            var rsa = RSA.Create();
            var rsaParameters = new RSAParameters();

            using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                rsaParameters.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.D = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.P = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.Q = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.DP = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.DQ = binr.ReadBytes(GetIntegerSize(binr));
                rsaParameters.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            rsa.ImportParameters(rsaParameters);
            return rsa;
        }

        /// <summary>
        /// 使用公钥创建RSA实例
        /// </summary>
        /// <param name="publicKeyString"></param>
        /// <returns></returns>
        private static RSA CreateRsaProviderFromPublicKey(string publicKeyString)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            var x509Key = Convert.FromBase64String(publicKeyString);

            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            using (MemoryStream mem = new MemoryStream(x509Key))
            {
                using (BinaryReader binr = new BinaryReader(mem))  //wrap Memory Stream with BinaryReader for easy reading
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    seq = binr.ReadBytes(15);       //read the Sequence OID
                    if (!CompareBytearrays(seq, seqOid))    //make sure Sequence for OID is correct
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    bt = binr.ReadByte();
                    if (bt != 0x00)     //expect null byte next
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                        lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte(); //advance 2 bytes
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                    int modsize = BitConverter.ToInt32(modint, 0);

                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {   //if first byte (highest order) of modulus is zero, don't include it
                        binr.ReadByte();    //skip this null byte
                        modsize -= 1;   //reduce modulus buffer size by 1
                    }

                    byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                    if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                        return null;
                    int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                    byte[] exponent = binr.ReadBytes(expbytes);

                    // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                    var rsa = RSA.Create();
                    RSAParameters rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);

                    return rsa;
                }

            }
        }

        /// <summary>
        /// 导入密钥算法
        /// </summary>
        /// <param name="binr"></param>
        /// <returns></returns>
        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
            if (bt == 0x82)
            {
                var highbyte = binr.ReadByte();
                var lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        private static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

       

    }

    /// <summary>
    /// RSA算法类型
    /// </summary>
    public enum RSAType
    {
        /// <summary>
        /// SHA1
        /// </summary>
        RSA = 0,
        /// <summary>
        /// RSA2 密钥长度至少为2048
        /// SHA256
        /// </summary>
        RSA2
    }
}
