using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EasyRelay
{
    /// <summary>
    /// 音频AAC编码器
    /// </summary>
    public class EasyAACEncoderAPI
    {
        #region 常量和枚举

        const string EasyAACEncoder_LIB_WIN32 = "libEasyAACEncoder.dll";

        const string EasyAACEncoder_LIB_Linux = "libeasyaacencoder.so";

        /// <summary>
        /// Audio Codec
        /// </summary>
        public enum Law
        {
            Law_ULaw =	0, 		/**< U law */
            Law_ALaw =	1, 		/**< A law */
            Law_PCM16 =	2, 		/**< 16 bit uniform PCM values. original pcm data */  
            Law_G726 =	3		/**< G726 */
        }

        /// <summary>
        /// Rate Bits
        /// </summary>
        public enum Rate
        {
            Rate16kBits = 2,	/**< 16k bits per second (2 bits per ADPCM sample) */
            Rate24kBits = 3,	/**< 24k bits per second (3 bits per ADPCM sample) */
            Rate32kBits = 4,	/**< 32k bits per second (4 bits per ADPCM sample) */
            Rate40kBits = 5	/**< 40k bits per second (5 bits per ADPCM sample) */
        }

        #endregion

        #region  结构体

        public struct G711Param
        {

        }

        public struct G726Param
        {
            /// <summary>
            /// //Rate16kBits Rate24kBits Rate32kBits Rate40kBits
            /// </summary>
            public System.Byte ucRateBits;
        }

        public struct InitParam
        {
            public System.Byte ucAudioCodec;// Law_uLaw  Law_ALaw Law_PCM16 Law_G726

            public System.Byte ucAudioChannel;//1

            public System.UInt32 u32AudioSamplerate;//8000

            public System.UInt32 u32PCMBitSize;//16

            public G711Param g711param;

            public G726Param g726param;
        }

        #endregion

        #region API

        /// <summary>
        /// Create AAC Encoder Handle Win32
        /// </summary>
        /// <param name="initPar"></param>
        /// <returns></returns>
        [DllImport(EasyAACEncoder_LIB_WIN32, EntryPoint = "Easy_AACEncoder_Init")]
        private extern static IntPtr Easy_AACEncoder_Init_Win32(InitParam initPar);

        /// <summary>
        /// Create AAC Encoder Handle Linux
        /// </summary>
        /// <param name="initPar"></param>
        /// <returns></returns>
        [DllImport(EasyAACEncoder_LIB_Linux, EntryPoint = "Easy_AACEncoder_Init")]
        private extern static IntPtr Easy_AACEncoder_Init_Linux(InitParam initPar);

        /// <summary>
        /// Input original data, output the encoede AAC data Win32
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="inbuf"></param>
        /// <param name="inlen"></param>
        /// <param name="outbuf"></param>
        /// <param name="outlen"></param>
        /// <returns></returns>
        [DllImport(EasyAACEncoder_LIB_WIN32, EntryPoint = "Easy_AACEncoder_Encode")]
        private extern static int Easy_AACEncoder_Encode_Win32(IntPtr handle, IntPtr inbuf, System.UInt32 inlen, IntPtr outbuf, System.UInt32 outlen);

        /// <summary>
        /// Input original data, output the encoede AAC data Linux
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="inbuf"></param>
        /// <param name="inlen"></param>
        /// <param name="outbuf"></param>
        /// <param name="outlen"></param>
        /// <returns></returns>
        [DllImport(EasyAACEncoder_LIB_Linux, EntryPoint = "Easy_AACEncoder_Encode")]
        private extern static int Easy_AACEncoder_Encode_Linux(IntPtr handle, IntPtr inbuf, System.UInt32 inlen, IntPtr outbuf, System.UInt32 outlen);


        /// <summary>
        /// Close Encoder Handle Win32
        /// </summary>
        /// <param name="handle"></param>
        [DllImport(EasyAACEncoder_LIB_WIN32, EntryPoint = "Easy_AACEncoder_Release")]
        private extern static void Easy_AACEncoder_Release_Win32(IntPtr handle);

        /// <summary>
        /// Close Encoder Handle Linux
        /// </summary>
        /// <param name="handle"></param>
        [DllImport(EasyAACEncoder_LIB_Linux, EntryPoint = "Easy_AACEncoder_Release")]
        private extern static void Easy_AACEncoder_Release_Linux(IntPtr handle);

        #endregion

            /// <summary>
        /// 获取是否为 Windows 系统
        /// </summary>
        /// <value></value>
        public static bool IsWindows
        {
            get
            {
                var platform = Environment.OSVersion.Platform;
                return platform == PlatformID.Win32NT || platform == PlatformID.Win32Windows;
            }
        }

        /// <summary>
        /// 获取是否初始化
        /// </summary>
        /// <value></value>
        public bool IsInit
        {
            get{ return this.AACEncoderHandle != IntPtr.Zero; }
        }

        public IntPtr AACEncoderHandle { get; private set; }

        /// <summary>
        /// 初始化 AACEncoder
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> Init()
        {
            //释放
            Release();
            try
            {
                InitParam initParam = new InitParam();
                IntPtr ret = IsWindows ? Easy_AACEncoder_Init_Win32(initParam) : Easy_AACEncoder_Init_Linux(initParam);
                if(ret == IntPtr.Zero)
                {
                    return new KeyValuePair<bool, string>(false, "AACEncoder 创建失败!");
                }
                else
                {
                    this.AACEncoderHandle = ret;
                    return new KeyValuePair<bool, string>(true , "AACEncoder 创建成功!");
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
                throw;
            }
        }

        /// <summary>
        /// AACEncoder 编码
        /// </summary>
        /// <param name="inbuf"></param>
        /// <param name="inlen"></param>
        /// <param name="outbuf"></param>
        /// <param name="outlen"></param>
        /// <returns></returns>
        public KeyValuePair<bool, string> Encode(IntPtr inbuf, System.UInt32 inlen, IntPtr outbuf, System.UInt32 outlen)
        {
            try
            {
                int ret = IsWindows ? Easy_AACEncoder_Encode_Win32(this.AACEncoderHandle, inbuf, inlen, outbuf, outlen) : Easy_AACEncoder_Encode_Linux(this.AACEncoderHandle, inbuf, inlen, outbuf, outlen);
                if(ret == 0)
                {
                    return new KeyValuePair<bool, string>(true, "AACEncoder 编码成功!");
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, "AACEncoder 编码失败!");
                }
            }
            catch (System.Exception e)
            {
                return new  KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 释放 AACEncoder
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> Release()
        {
            if(this.AACEncoderHandle == IntPtr.Zero)
            {
                return new KeyValuePair<bool, string>(false, "AACEncoder 未初始化!");
            }
            try
            {
                if(IsWindows)
                {
                    Easy_AACEncoder_Release_Win32(this.AACEncoderHandle);
                }
                else
                {
                    Easy_AACEncoder_Release_Linux(this.AACEncoderHandle);
                }
                this.AACEncoderHandle = IntPtr.Zero;
                return new KeyValuePair<bool, string>(true, "AACEncoder 释放成功!");
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }
    }

}