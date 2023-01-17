using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace CN.SMSSDK.Sharp
{
    public static class RSAHelper
    {
        public static readonly BigInteger e = BigInteger.Parse("10076229452305170363284523002287788362891942094499259986077446739262864815703612932916776862551174475816402617797292206010609940169458144629629006028036947", System.Globalization.NumberStyles.Integer);
        public static readonly BigInteger p = BigInteger.Parse("2812270971283895641890725534922161033522202251012339746034216824624545467471847509824679174226189943922265612877553119939746705022611677490928638430168609", System.Globalization.NumberStyles.Integer);

        public static byte[] Encrypt(byte[] bArr)
        {
            try
            {
                int i2 = 16;
                int i3 = 5;
                int i4 = 0;
                MemoryStream ms = new MemoryStream(bArr);
                MemoryStream msout = new MemoryStream();

                while (ms.Length > i4)
                {
                    int min = Math.Min(bArr.Length - i4, i3);
                    EncryptPart(bArr, i4, min, i2, msout);
                    i4 += min;
                }
                ms.Close();
                return msout.ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static void EncryptPart(byte[] bArr, int i1, int i2, int i3, MemoryStream dataOutputStream)
        {
            byte[] bArr2 = bArr;
            if (bArr.Length != i2 || i1 != 0)
            {
                bArr2 = new byte[i2];
            }
            ArrayCopy(bArr, i1, bArr2, 0, i2);
            BigInteger bigInteger3 = new BigInteger(EncryptPartInternal(bArr2, i3));
            byte[] byteArray = BigInteger.ModPow(bigInteger3, e, p).ToByteArray();
            //if (byteArray.Last() == 0)
            //{
            //    dataOutputStream.WriteInt(byteArray.Length - 1);
            //    dataOutputStream.Write(byteArray.Reverse().ToArray(), 1);
            //}
            //else
            {
                dataOutputStream.WriteInt(byteArray.Length);
                dataOutputStream.Write(byteArray.Reverse().ToArray());
            }
        }

        private static byte[] EncryptPartInternal(byte[] bArr, int i)
        {
            if (bArr.Length > i - 1)
            {
                throw new Exception("Message too large");
            }
            byte[] bArr2 = new byte[i];
            bArr2[0] = 1;
            int length = bArr.Length;
            bArr2[1] = (byte)(length >> 24);
            bArr2[2] = (byte)(length >> 16);
            bArr2[3] = (byte)(length >> 8);
            bArr2[4] = (byte)length;
            ArrayCopy(bArr, 0, bArr2, i - length, length);
            return bArr2.Reverse().ToArray<byte>();
        }

        public static byte[] Decrypt(byte[] bArr)
        {
            try
            {
                MemoryStream ms = new MemoryStream(bArr);
                MemoryStream msout = new MemoryStream();
                while (ms.Position < ms.Length)
                {
                    DecryptPart(ms.ReadInt(), ms, msout);
                }
                ms.Close();
                msout.Close();
                return msout.ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static void DecryptPart(int i, Stream dataInputStream, Stream byteArrayOutputStream)
        {
            byte[] bArr = new byte[i];
            dataInputStream.Read(bArr, 0, i);
            byteArrayOutputStream.Write(DecryptPartInternal(BigInteger.ModPow(new BigInteger(bArr.Reverse().ToArray()), e, p).ToByteArray().Reverse().ToArray()));
        }
        
        private static byte[] DecryptPartInternal(byte[] bArr)
        {
            if (bArr[0] != 1)
            {
                throw new Exception("Not RSA Block");
            }
            int i = ((bArr[1] & 255) << 24) + ((bArr[2] & 255) << 16) + ((bArr[3] & 255) << 8) + (bArr[4] & 255);
            byte[] bArr2 = new byte[i];
            ArrayCopy(bArr, bArr.Length - i, bArr2, 0, i);
            return bArr2;
        }

        ////Help
        public static void ArrayCopy(byte[] src, int srcPos, byte[] dest, int destPos, int length)
        {
            for (int i = 0; i < length; i++)
                dest[destPos + i] = src[srcPos + i];
        }
    }
}
