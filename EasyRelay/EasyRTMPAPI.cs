using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EasyRelay
{
    /// <summary>
    /// RTMP推流客户端
    /// </summary>
    public class EasyRTMPAPI
    {
        #region 常量和枚举

        const string EasyRTMP_LIB_WIN32 = "libeasyrtmp.dll";

        const string EasyRTMP_LIB_Linux = "libeasyrtmp.so";

        const string EasyRTMP_KEY_WIN32 = "79736C36655969576B5A734154526C646F756179532B394659584E35556C524E55463949535573755A58686C4B56634D5671442F532F34675A57467A65513D3D";

        const string EasyRTMP_KEY_LINUX = "79736C36655A4F576B597141725370636F39565245764E6C59584E35636E52746346396F6157736A567778576F5036532F69426C59584E35";


        /// <summary>
        /// 推送事件类型定义
        /// </summary>
        public enum EASY_RTMP_STATE_T
        {
            EASY_RTMP_STATE_CONNECTING   =   1,     /* 连接中 */
            EASY_RTMP_STATE_CONNECTED,              /* 连接成功 */
            EASY_RTMP_STATE_CONNECT_FAILED,         /* 连接失败 */
            EASY_RTMP_STATE_CONNECT_ABORT,          /* 连接异常中断 */
            EASY_RTMP_STATE_PUSHING,                /* 推流中 */
            EASY_RTMP_STATE_DISCONNECTED,           /* 断开连接 */
            EASY_RTMP_STATE_ERROR
        }

        #endregion

        /// <summary>
        /// RTMP 回调
        /// typedef int (*EasyRTMPCallBack)(int _frameType, char *pBuf, EASY_RTMP_STATE_T _state, void *_userPtr);
        /// </summary>
        /// <param name="frameType"> EASY_SDK_VIDEO_FRAME_FLAG/EASY_SDK_AUDIO_FRAME_FLAG/EASY_SDK_EVENT_FRAME_FLAG/... </param>
        /// <param name="pBuf"> 回调的数据部分 </param>
        /// <param name="state"></param>
        /// <param name="userPtr"> 用户自定义数据 </param>
        /// <returns></returns>
        public delegate int EasyRTMPCallBack(int frameType, IntPtr pBuf, EASY_RTMP_STATE_T state, IntPtr userPtr);

        #region API

        /// <summary>
        /// rtmp 激活 win32
        /// </summary>
        /// <param name="license"></param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_WIN32, EntryPoint = "EasyRTMP_Activate")]
        private extern static int EasyRTMP_Activate_Win32(string license);

        /// <summary>
        /// rtmp 激活 linux
        /// </summary>
        /// <param name="license"></param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_Linux, EntryPoint = "EasyRTMP_Activate")]
        private extern static int EasyRTMP_Activate_Linux(string license);

        /// <summary>
        /// 创建RTMP推送Session 返回推送句柄 win32
        /// </summary>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_WIN32, EntryPoint = "EasyRTMP_Create")]
        private extern static IntPtr EasyRTMP_Create_Win32();

        /// <summary>
        /// 创建RTMP推送Session 返回推送句柄 linux
        /// </summary>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_Linux, EntryPoint = "EasyRTMP_Create")]
        private extern static IntPtr EasyRTMP_Create_Linux();

        /// <summary>
        /// 设置数据回调 win32
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        /// <param name="callBack"> 回调委托 </param>
        /// <param name="userptr"> 用户自定义数据 </param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_WIN32, EntryPoint = "EasyRTMP_SetCallback")]
        private extern static int EasyRTMP_SetCallback_Win32(IntPtr handle, EasyRTMPCallBack callBack, IntPtr userptr);

        /// <summary>
        /// 设置数据回调 linux
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        /// <param name="callBack"> 回调委托 </param>
        /// <param name="userptr"> 用户自定义数据 </param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_Linux, EntryPoint = "EasyRTMP_SetCallback")]
        private extern static int EasyRTMP_SetCallback_Linux(IntPtr handle, EasyRTMPCallBack callBack, IntPtr userptr);

        /// <summary>
        /// 创建RTMP推送的参数信息 win32
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        /// <param name="pstruStreamInfo"> 流媒体信息 </param>
        /// <param name="bufferKSize"> 缓冲区大小 </param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_WIN32, EntryPoint = "EasyRTMP_InitMetadata")]
        private extern static int EasyRTMP_InitMetadata_Win32(IntPtr handle,EasyTypes.EASY_MEDIA_INFO_T pstruStreamInfo, int bufferKSize);

        /// <summary>
        /// 创建RTMP推送的参数信息 linux
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        /// <param name="pstruStreamInfo"> 流媒体信息 </param>
        /// <param name="bufferKSize"> 缓冲区大小 </param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_Linux, EntryPoint = "EasyRTMP_InitMetadata")]
        private extern static int EasyRTMP_InitMetadata_Linux(IntPtr handle,EasyTypes.EASY_MEDIA_INFO_T pstruStreamInfo, int bufferKSize);

        /// <summary>
        /// 连接RTMP服务器 win32
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        /// <param name="url"> rtmp url </param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_WIN32, EntryPoint = "EasyRTMP_Connect")]
        private extern static bool EasyRTMP_Connect_Win32(IntPtr handle, string url);

        /// <summary>
        /// 连接RTMP服务器 linux
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        /// <param name="url"> rtmp url </param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_Linux, EntryPoint = "EasyRTMP_Connect")]
        private extern static bool EasyRTMP_Connect_Linux(IntPtr handle, string url);

        /// <summary>
        /// 推送H264或AAC流 linux
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        /// <param name="frame"> 侦信息 </param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_WIN32, EntryPoint = "EasyRTMP_SendPacket")]
        private extern static int EasyRTMP_SendPacket_Win32(IntPtr handle, EasyTypes.EASY_AV_Frame frame);

        /// <summary>
        /// 推送H264或AAC流 linux
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        /// <param name="frame"> 侦信息 </param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_Linux, EntryPoint = "EasyRTMP_SendPacket")]
        private extern static int EasyRTMP_SendPacket_Linux(IntPtr handle, EasyTypes.EASY_AV_Frame frame);

        /// <summary>
        /// 获取缓冲区大小 win32
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="usedSize"></param>
        /// <param name="totalSize"></param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_WIN32, EntryPoint = "EasyRTMP_GetBufInfo")]
        private extern static int EasyRTMP_GetBufInfo_Win32(IntPtr handle, out int usedSize, out int totalSize);

        /// <summary>
        /// 获取缓冲区大小 linux
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="usedSize"></param>
        /// <param name="totalSize"></param>
        /// <returns></returns>
        [DllImport(EasyRTMP_LIB_Linux, EntryPoint = "EasyRTMP_GetBufInfo")]
        private extern static int EasyRTMP_GetBufInfo_Linux(IntPtr handle, out int usedSize, out int totalSize);

        /// <summary>
        /// 停止RTMP推送，释放句柄 win32
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        [DllImport(EasyRTMP_LIB_WIN32, EntryPoint = "EasyRTMP_Release")]
        private extern static void EasyRTMP_Release_Win32(IntPtr handle);

        /// <summary>
        /// 停止RTMP推送，释放句柄 linux
        /// </summary>
        /// <param name="handle"> rtmp 句柄 </param>
        [DllImport(EasyRTMP_LIB_Linux, EntryPoint = "EasyRTMP_Release")]
        private extern static void EasyRTMP_Release_Linux(IntPtr handle);

        #endregion

        /// <summary>
        /// 激活状态
        /// </summary>
        /// <value></value>
        public static bool Activated { get; private set; }

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
        /// RTMP 激活
        /// </summary>
        /// <returns></returns>
        public static KeyValuePair<bool, string> Activate()
        {
            if(Activated)
            {
                return new KeyValuePair<bool, string>(true, "RTSP 已激活!");
            }
            try
            {
                int ret = IsWindows ? EasyRTMP_Activate_Win32(EasyRTMP_KEY_WIN32) : EasyRTMP_Activate_Linux(EasyRTMP_KEY_LINUX);
                if(ret > 0)
                {
                    Activated = true;
                    Console.WriteLine("RTMP 激活成功!");
                    return new KeyValuePair<bool, string>(true, "RTMP 激活成功!");
                }
                else
                {
                    Activated = false;
                    Console.WriteLine(string.Format("RTMP 激活失败，错误代码：{0}", ret));
                    return new KeyValuePair<bool, string>(false, string.Format("RTMP 激活失败，错误代码：{0}", ret));
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// RTMP 句柄
        /// </summary>
        /// <value></value>
        public IntPtr RtmpHandle { get; private set; } = IntPtr.Zero;

        /// <summary>
        /// 获取是否初始化
        /// </summary>
        /// <value></value>
        public bool IsInit
        {
            get{ return this.RtmpHandle != IntPtr.Zero; }
        }

        /// <summary>
        /// 回调委托
        /// </summary>
        /// <value></value>
        public EasyRTMPCallBack RTMPCallBack { get; private set; }

        /// <summary>
        /// 创建RTMP推送Session
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> Create()
        {
            //rtmp 释放
            this.Release();
            try
            {
                IntPtr ret = IsWindows ? EasyRTMP_Create_Win32() : EasyRTMP_Create_Linux();
                if(ret == IntPtr.Zero)
                {
                    this.RtmpHandle = IntPtr.Zero;
                    return new KeyValuePair<bool, string>(false, "RTMP 创建失败!");
                }
                else
                {
                    this.RtmpHandle = ret;
                    return new KeyValuePair<bool, string>(true, "RTMP 创建成功!");
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 设置回调委托
        /// </summary>
        /// <param name="callBack"> 回调委托 </param>
        /// <param name="userptr"> 用户自定义数据 </param>
        /// <returns></returns>
        public KeyValuePair<bool, string> SetCallback(EasyRTMPCallBack callBack, IntPtr userptr)
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTMP 未创建!");
            }
            try
            {
                int ret = IsWindows ? EasyRTMP_SetCallback_Win32(this.RtmpHandle, callBack, userptr) : EasyRTMP_SetCallback_Linux(this.RtmpHandle, callBack, userptr);
                if(ret == 0)
                {
                    this.RTMPCallBack = callBack;
                    return new KeyValuePair<bool, string>(true, "RTMP 设置回调成功!");
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, string.Format("RTMP 设置回调失败，错误代码：{0}", ret));
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 创建RTMP推送的参数信息
        /// </summary>
        /// <param name="mediaInfo"> 流媒体信息 </param>
        /// <param name="bufferKSize"> 缓冲区大小 </param>
        /// <returns></returns>
        public KeyValuePair<bool, string> InitMetadata(EasyTypes.EASY_MEDIA_INFO_T mediaInfo, int bufferKSize = 1024)
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTMP 未创建!");
            }
            try
            {
                int ret = IsWindows ? EasyRTMP_InitMetadata_Win32(this.RtmpHandle, mediaInfo, bufferKSize) : EasyRTMP_InitMetadata_Linux(this.RtmpHandle, mediaInfo, bufferKSize);
                if(ret == 0)
                {
                    return new KeyValuePair<bool, string>(true, "RTMP 创建媒体成功!");
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, string.Format("RTMP 创建媒体失败，错误代码：{0}", ret));
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 连接RTMP服务器
        /// </summary>
        /// <param name="url"> rtmp 地址 </param>
        /// <returns></returns>
        public KeyValuePair<bool, string> Connect(string url)
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTMP 未创建!");
            }
            try
            {
                var ret = IsWindows ? EasyRTMP_Connect_Win32(this.RtmpHandle, url) : EasyRTMP_Connect_Linux(this.RtmpHandle, url);
                if(ret)
                {
                    return new KeyValuePair<bool, string>(true, "RTMP 创建连接成功!");
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, string.Format("RTMP 创建连接失败，错误代码：{0}", ret));
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 获取缓冲区大小
        /// </summary>
        /// <param name="usedSize"> 已使用缓冲区大小 </param>
        /// <param name="totalSize"> 总共缓冲区大小 </param>
        /// <returns></returns>
        public KeyValuePair<bool, string> GetBufInfo(out int usedSize, out int totalSize)
        {
            if(!this.IsInit)
            {
                usedSize = 0;
                totalSize = 0;
                return new KeyValuePair<bool, string>(false, "RTMP 未创建!");
            }
            try
            {
                int ret = IsWindows ? EasyRTMP_GetBufInfo_Win32(this.RtmpHandle, out usedSize, out totalSize) : EasyRTMP_GetBufInfo_Linux(this.RtmpHandle, out usedSize, out totalSize);
                if(ret == 0)
                {
                    return new KeyValuePair<bool, string>(true, "");
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, "");
                }
            }
            catch (System.Exception e)
            {
                usedSize = 0;
                totalSize = 0;
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 停止RTMP推送，释放句柄
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> Release()
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTMP 未创建!");
            }
            try
            {
                if(IsWindows)
                {
                    EasyRTMP_Release_Win32(this.RtmpHandle);
                }
                else
                {
                    EasyRTMP_Release_Linux(this.RtmpHandle);
                }
                this.RtmpHandle = IntPtr.Zero;
                return new KeyValuePair<bool, string>(true, "RTMP 释放成功!");
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 推送H264或AAC流
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public KeyValuePair<bool, string> SendPacket(EasyTypes.EASY_AV_Frame frame)
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTMP 未创建!");
            }
            try
            {
                int ret = IsWindows ? EasyRTMP_SendPacket_Win32(this.RtmpHandle, frame) : EasyRTMP_SendPacket_Linux(this.RtmpHandle, frame);
                if(ret == 0)
                {
                    return new KeyValuePair<bool, string>(true, "发送流媒体数据包成功!");
                }
                else 
                {
                    return new KeyValuePair<bool, string>(true, "发送流媒体数据包失败!");
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }
    }
}