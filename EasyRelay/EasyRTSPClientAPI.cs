using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace EasyRelay
{
    /// <summary>
    /// RTSP拉流客户端
    /// </summary>
    public class EasyRTSPClientAPI
    {
        #region 常量和枚举

        const string EasyRTSP_LIB_WIN32 = "libEasyRTSPClient.dll";

        const string EasyRTSP_LIB_Linux = "libeasyrtspclient.so";

        const string EasyRTSP_KEY_WIN32 = "6D75724D7A4969576B5A7541725370636F3956524575314659584E35556C525455454E73615756756443356C65475570567778576F5036532F69426C59584E35";

        const string EasyRTSP_KEY_Linux = "6D75724D7A4A4F576B597141725370636F3956524576466C59584E35636E527A63474E736157567564434E58444661672F704C2B4947566863336B3D";

        /// <summary>
        /// RTP 连接类型
        /// </summary>
        public enum EASY_RTP_CONNECT_TYPE
        {
            EASY_RTP_OVER_TCP	=	0x01,		/* RTP Over TCP */
            EASY_RTP_OVER_UDP,					/* RTP Over UDP */
            EASY_RTP_OVER_MULTICAST				/* RTP Over MULTICAST */
        }

        #endregion

        /// <summary>
        /// RTSP 回调
        /// typedef int (Easy_APICALL *RTSPSourceCallBack)( int _channelId, void *_channelPtr, int _frameType, char *pBuf, EASY_FRAME_INFO* _frameInfo);
        /// </summary>
        /// <param name="channelId"> 通道号 </param>
        /// <param name="channelPtr"> 通道对应对象 </param>
        /// <param name="frameType"> EASY_SDK_VIDEO_FRAME_FLAG/EASY_SDK_AUDIO_FRAME_FLAG/EASY_SDK_EVENT_FRAME_FLAG/... </param>
        /// <param name="pBuf"> 回调的数据部分 </param>
        /// <param name="frameInfo"> 帧结构数据 </param>
        /// <returns></returns>
        public delegate int RTSPSourceCallBack(int channelId, IntPtr channelPtr, int frameType, IntPtr pBuf, /* EasyTypes.EASY_FRAME_INFO */ IntPtr frameInfo);

        #region API

        /// <summary>
        /// 获取最后一次错误的错误码 win32
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport(EasyRTSP_LIB_WIN32, EntryPoint = "EasyRTSP_GetErrCode")]
        private extern static int EasyRTSP_GetErrCode_Win32(IntPtr handle);

        /// <summary>
        /// 获取最后一次错误的错误码 linux
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport(EasyRTSP_LIB_Linux, EntryPoint = "EasyRTSP_GetErrCode")]
        private extern static int EasyRTSP_GetErrCode_Linux(IntPtr handle);

        /// <summary>
        /// rtsp 激活 win32
        /// </summary>
        /// <param name="license"></param>
        /// <returns></returns>
        [DllImport(EasyRTSP_LIB_WIN32 ,EntryPoint = "EasyRTSP_Activate")]
        private extern static int EasyRTSP_Activate_Win32(string license);

        /// <summary>
        /// rtsp 激活 linux
        /// </summary>
        /// <param name="license"></param>
        /// <returns></returns>
        [DllImport(EasyRTSP_LIB_Linux ,EntryPoint = "EasyRTSP_Activate")]
        private extern static int EasyRTSP_Activate_Linux(string license);

        /// <summary>
        /// 创建RTSPClient句柄  win32
        /// </summary>
        /// <param name="handle"> 输出 RTSPClient句柄 </param>
        /// <returns> 返回0表示成功，返回非0表示失败 </returns>
        [DllImport(EasyRTSP_LIB_WIN32, EntryPoint = "EasyRTSP_Init")]
        private extern static int EasyRTSP_Init_Win32(out IntPtr handle);

        /// <summary>
        /// 创建RTSPClient句柄  linux
        /// </summary>
        /// <param name="handle"> 输出 RTSPClient句柄</param>
        /// <returns> 返回0表示成功，返回非0表示失败 </returns>
        [DllImport(EasyRTSP_LIB_Linux, EntryPoint = "EasyRTSP_Init")]
        private extern static int EasyRTSP_Init_Linux(out IntPtr handle);

        /// <summary>
        /// 释放RTSPClient win32
        /// </summary>
        /// <param name="handle"> RTSPClient句柄 </param>
        /// <returns></returns>
        [DllImport(EasyRTSP_LIB_WIN32, EntryPoint = "EasyRTSP_Deinit")]
        private extern static int EasyRTSP_Deinit_Win32(IntPtr handle);

        /// <summary>
        /// 释放RTSPClient linux
        /// </summary>
        /// <param name="handle"> RTSPClient句柄 </param>
        /// <returns></returns>
        [DllImport(EasyRTSP_LIB_Linux, EntryPoint = "EasyRTSP_Deinit")]
        private extern static int EasyRTSP_Deinit_Linux(IntPtr handle);

        /// <summary>
        /// 设置数据回调 win32
        /// </summary>
        /// <param name="handle"> RTSPClient句柄 </param>
        /// <param name="callBack"> 回调委托 </param>
        /// <returns></returns>
        [DllImport(EasyRTSP_LIB_WIN32, EntryPoint = "EasyRTSP_SetCallback")]
        private extern static int EasyRTSP_SetCallback_Win32(IntPtr handle, RTSPSourceCallBack callBack);

        /// <summary>
        /// 设置数据回调 linux
        /// </summary>
        /// <param name="handle"> RTSPClient句柄 </param>
        /// <param name="callBack"> 回调委托 </param>
        /// <returns></returns>
        [DllImport(EasyRTSP_LIB_Linux, EntryPoint = "EasyRTSP_SetCallback")]
        private extern static int EasyRTSP_SetCallback_Linux(IntPtr handle, RTSPSourceCallBack callBack);

        /// <summary>
        /// 打开网络流 Win32
        /// </summary>
        /// <param name="handle"> RTSPClient句柄 </param>
        /// <param name="channelid"> 通道id </param>
        /// <param name="url"> rtsp url </param>
        /// <param name="connType"> 连接方式 </param>
        /// <param name="mediaType"> 媒体类型 </param>
        /// <param name="username"> 用户名 </param>
        /// <param name="password"> 密码 </param>
        /// <param name="userPtr"> 用户信息 </param>
        /// <param name="reconn"> 1000表示长连接,即如果网络断开自动重连, 其它值为连接次数 </param>
        /// <param name="outRtpPacke"> 默认为0,即回调输出完整的帧, 如果为1,则输出RTP包 </param>
        /// <param name="heartbeatType"> 0x00:不发送心跳 0x01:OPTIONS 0x02:GET_PARAMETER </param>
        /// <param name="verbosity"> 日志打印输出等级，0表示不输出 </param>
        /// <returns></returns>
        [DllImport(EasyRTSP_LIB_WIN32, EntryPoint = "EasyRTSP_OpenStream", CallingConvention = CallingConvention.StdCall)]
        private extern static int EasyRTSP_OpenStream_Win32(IntPtr handle, int channelid, string url, EASY_RTP_CONNECT_TYPE connType, int mediaType, string username, string password, IntPtr userPtr, int reconn, int outRtpPacke, int heartbeatType, int verbosity);

        /// <summary>
        /// 打开网络流 Linux
        /// </summary>
        /// <param name="handle"> RTSPClient句柄 </param>
        /// <param name="channelid"> 通道id </param>
        /// <param name="url"> rtsp url </param>
        /// <param name="connType"> 连接方式 </param>
        /// <param name="mediaType"> 媒体类型 </param>
        /// <param name="username"> 用户名 </param>
        /// <param name="password"> 密码 </param>
        /// <param name="userPtr"> 用户信息 </param>
        /// <param name="reconn"> 1000表示长连接,即如果网络断开自动重连, 其它值为连接次数 </param>
        /// <param name="outRtpPacke"> 默认为0,即回调输出完整的帧, 如果为1,则输出RTP包 </param>
        /// <param name="heartbeatType"> 0x00:不发送心跳 0x01:OPTIONS 0x02:GET_PARAMETER </param>
        /// <param name="verbosity"> 日志打印输出等级，0表示不输出 </param>
        /// <returns></returns>
        [DllImport(EasyRTSP_LIB_Linux, EntryPoint = "EasyRTSP_OpenStream", CallingConvention = CallingConvention.StdCall)]
        private extern static int EasyRTSP_OpenStream_Linux(IntPtr handle, int channelid, string url, EASY_RTP_CONNECT_TYPE connType, int mediaType, string username, string password, IntPtr userPtr, int reconn, int outRtpPacke, int heartbeatType, int verbosity);

        /// <summary>
        /// 关闭网络流
        /// </summary>
        /// <param name="handle"> RTSPClient句柄 </param>
        /// <returns></returns>
        [DllImport(EasyRTSP_LIB_WIN32, EntryPoint = "EasyRTSP_CloseStream", CallingConvention = CallingConvention.StdCall)]
        private extern static int EasyRTSP_CloseStream_Win32(IntPtr handle);

        /// <summary>
        /// 关闭网络流
        /// </summary>
        /// <param name="handle"> RTSPClient句柄 </param>
        /// <returns></returns>
        [DllImport(EasyRTSP_LIB_Linux, EntryPoint = "EasyRTSP_CloseStream", CallingConvention = CallingConvention.StdCall)]
        private extern static int EasyRTSP_CloseStream_Linux(IntPtr handle);

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
        /// RTSP 激活
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
                int ret = IsWindows ? EasyRTSP_Activate_Win32(EasyRTSP_KEY_WIN32) : EasyRTSP_Activate_Linux(EasyRTSP_KEY_Linux);
                if(ret > 0)
                {
                    Activated = true;
                    Console.WriteLine("RTSP 激活成功!");
                    return new KeyValuePair<bool, string>(true, "RTSP 激活成功!");
                }
                else
                {
                    Activated = false;
                    Console.WriteLine(string.Format("RTSP 激活失败，错误代码：{0}", ret));
                    return new KeyValuePair<bool, string>(false, string.Format("RTSP 激活失败，错误代码：{0}", ret));
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 日志级别
        /// </summary>
        /// <value></value>
        public int Verbosity { get; set; } = 3;

        /// <summary>
        /// RTSP 句柄
        /// </summary>
        /// <value></value>
        public IntPtr RtspHandle { get; private set; } = IntPtr.Zero;

        /// <summary>
        /// 获取是否初始化
        /// </summary>
        /// <value></value>
        public bool IsInit
        {
            get{ return this.RtspHandle != IntPtr.Zero; }
        }

        /// <summary>
        /// 回调委托
        /// </summary>
        /// <value></value>
        public RTSPSourceCallBack RTSPCallBack { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> Init()
        {
            //rtsp 释放
            this.Deinit();
            try
            {
                IntPtr ptr;
                int ret = IsWindows ? EasyRTSP_Init_Win32(out ptr) : EasyRTSP_Init_Linux(out ptr);
                if(ptr != IntPtr.Zero)
                {
                    this.RtspHandle = ptr;
                    return new KeyValuePair<bool, string>(true, "RTSP 创建成功!");
                }
                else
                {
                    this.RtspHandle = IntPtr.Zero;
                    return new KeyValuePair<bool, string>(false, string.Format("RTSP 创建失败，错误代码：{0}", ret));
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> Deinit()
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTSP 未创建!");
            }
            try
            {
                int ret = IsWindows ? EasyRTSP_Deinit_Win32(this.RtspHandle) : EasyRTSP_Deinit_Linux(this.RtspHandle);
                if(ret == 0)
                {
                    this.RtspHandle = IntPtr.Zero;
                    return new KeyValuePair<bool, string>(true, "RTSP 释放成功!");
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, string.Format("RTSP 释放失败，错误代码：{0}", ret));
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 获取最后一次错误的错误码 
        /// </summary>
        /// <param name="errorCode"> 错误代码 </param>
        /// <returns></returns>
        public KeyValuePair<bool, string> GetErrCode(out int errorCode)
        {
            errorCode = 0;
            if(!IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTSP 未初始化!");
            }
            try
            {
                errorCode = IsWindows ? EasyRTSP_GetErrCode_Win32(this.RtspHandle) : EasyRTSP_GetErrCode_Linux(this.RtspHandle);
                return new KeyValuePair<bool, string>(true, "RTSP 获取错误代码成功 !");
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
        /// <returns></returns>
        public KeyValuePair<bool, string> SetCallback(RTSPSourceCallBack callBack)
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTSP 未创建!");
            }
            try
            {
                int ret = IsWindows ? EasyRTSP_SetCallback_Win32(this.RtspHandle, callBack) : EasyRTSP_SetCallback_Linux(this.RtspHandle, callBack);
                if(ret == 0)
                {
                    this.RTSPCallBack = callBack;
                    return new KeyValuePair<bool, string>(true, "RTSP 设置回调成功!");
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, string.Format("RTSP 设置回调失败，错误代码：{0}", ret));
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 打开网络流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userPtr"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public KeyValuePair<bool, string> OpenStream(string url, IntPtr userPtr, int option)
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTSP 未创建!");
            }
            try
            {
                int channelId = 0;
                int mediaType = EasyTypes.EASY_SDK_VIDEO_FRAME_FLAG | EasyTypes.EASY_SDK_AUDIO_FRAME_FLAG;
                var connType = EASY_RTP_CONNECT_TYPE.EASY_RTP_OVER_TCP;
                int ret = 0;
                if(IsWindows)
                {
                    ret = EasyRTSP_OpenStream_Win32(this.RtspHandle, channelId, url, connType, mediaType, "", "", userPtr, 1000, 0, option, this.Verbosity);
                }
                else
                {
                    ret = EasyRTSP_OpenStream_Linux(this.RtspHandle, channelId, url, connType, mediaType, "", "", userPtr, 1000, 0, option, 3);
                }
                if(ret == 0)
                {
                    return new KeyValuePair<bool, string>(true, "RTSP 打开网络流成功!");
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, string.Format("RTSP 打开网络流失败，错误代码：{0}", ret));
                }
            }
            catch (Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }

        /// <summary>
        /// 关闭网络流
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> CloseStream()
        {
            if(!this.IsInit)
            {
                return new KeyValuePair<bool, string>(false, "RTSP 未创建!");
            }
            try
            {
                int ret = IsWindows ? EasyRTSP_CloseStream_Win32(this.RtspHandle) : EasyRTSP_CloseStream_Linux(this.RtspHandle);
                if(ret == 0)
                {
                    return new KeyValuePair<bool, string>(true, "RTSP 关闭网络流成功!");
                }
                else
                {
                    return new KeyValuePair<bool, string>(false, string.Format("RTSP 关闭网络流失败，错误代码：{0}", ret));
                }
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<bool, string>(false, e.Message);
            }
        }
    }
 
}