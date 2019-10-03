using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EasyRelay
{
    /// <summary>
    /// 直播客户端
    /// </summary>
    public class EasyRTSPLive
    {
        /// <summary>
        /// rtsp url
        /// </summary>
        /// <value></value>
        public string RtspUrl { get; set; }

        /// <summary>
        /// rtmp url
        /// </summary>
        /// <value></value>
        public string RtmpUrl { get; set; }

        /// <summary>
        /// 通道id
        /// </summary>
        /// <value></value>
        public int ChannelId { get; set; } = 0;

        public int Option { get; set; } = 0;

        /// <summary>
        /// 日志级别
        /// </summary>
        /// <value></value>
        public int Verbosity
        {
            get { return this.rtspClient.Verbosity; }
            set{ this.rtspClient.Verbosity = value; }
        }

        private IntPtr userPtr;

        /// <summary>
        /// rtsp 客户端
        /// </summary>
        private EasyRTSPClientAPI rtspClient;

        /// <summary>
        /// rtmp 客户端
        /// </summary>
        private EasyRTMPAPI rtmpClient;

        /// <summary>
        /// AAC 编码器
        /// </summary>
        private EasyAACEncoderAPI aacEncoder;

        /// <summary>
        /// 连接变更事件委托
        /// </summary>
        /// <param name="connected"></param>
        /// <param name="message"></param>
        public delegate void ConnectChangedEventHandler(bool connected, string message);

        /// <summary>
        /// 获取 RTSP 连接状态
        /// </summary>
        /// <value></value>
        public bool RtspConnected { get; private set; }

        /// <summary>
        /// 获取 RTSP 消息内容
        /// </summary>
        /// <value></value>
        public string RtspMessage { get; private set; }

        /// <summary>
        /// rtsp 连接变更事件
        /// </summary>
        public event ConnectChangedEventHandler RtspConnectChanged;

        public void OnRtspConnectChanged(bool connected, string message)
        {
            //判断RTSP结果是否变化
            if(this.RtspConnected == connected && this.RtspMessage == message)
            {
                return;
            }
            this.RtspConnected = connected;
            this.RtspMessage = message;
            if(this.RtspConnectChanged != null)
            {
                this.RtspConnectChanged(connected, message);
            }
            else
            {
                Console.WriteLine(string.Concat("RTSP Connected:", connected, " , ", "Message:", message));
            }
        }

        /// <summary>
        /// 获取 RTMP 连接状态
        /// </summary>
        /// <value></value>
        public bool RtmpConnected { get; private set; }

        /// <summary>
        /// 获取 RTMP 消息内容
        /// </summary>
        /// <value></value>
        public string RtmpMessage { get; private set; }

        /// <summary>
        /// rtmp 连接变更事件
        /// </summary>
        public event ConnectChangedEventHandler RtmpConnectChanged;

        public void OnRtmpConnectChanged(bool connected, string message)
        {
            //判断RTMP结果是否变化
            if(this.RtmpConnected == connected && this.RtmpMessage == message)
            {
                return;
            }
            this.RtmpConnected = connected;
            this.RtmpMessage = message;
            if(this.RtmpConnectChanged != null)
            {
                this.RtmpConnectChanged(connected, message);
            }
            else
            {
                Console.WriteLine(string.Concat("RTMP Connected:", connected, " , ", "Message:", message));
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public EasyRTSPLive()
        {
            this.rtspClient = new EasyRTSPClientAPI();
            this.rtmpClient = new EasyRTMPAPI();
            this.aacEncoder = new EasyAACEncoderAPI();
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> Start()
        {
            var ret = this.StartRtsp();
            if(!ret.Key)
            {
                return ret;
            }
            return new KeyValuePair<bool, string>(true, "live 启动成功!");
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            this.StopRtsp();
            this.StopRtmp();

            if(this.aacEncoder.IsInit)
            {
                this.aacEncoder.Release();
            }
        }

        /// <summary>
        /// 启动 rtsp
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> StartRtsp()
        {
            //rtsp 初始化
            var ret = this.rtspClient.Init();
            if(!ret.Key)
            {
                this.StopRtsp();
                return ret;
            }
            //rtsp 回调
            ret = this.rtspClient.SetCallback(this.RtspCallBack);
            if(!ret.Key)
            {
                this.StopRtsp();
                return ret;
            }
            
            //创建通道信息结构体
            var channelInfo = new _channel_info();
            channelInfo.fCfgInfo.channelId = this.ChannelId;
            channelInfo.fHavePrintKeyInfo = false;

            char[] charBuffer = new char[64];
            string stringBuffer = string.Format("channel{0}", this.ChannelId);
            Array.Copy(stringBuffer.ToCharArray(), charBuffer, stringBuffer.Length);
            channelInfo.fCfgInfo.channelName = charBuffer;

            charBuffer = new char[512];
            Array.Copy(this.RtspUrl.ToCharArray(), charBuffer, this.RtspUrl.Length);
            channelInfo.fCfgInfo.srcRtspAddr = charBuffer;

            charBuffer = new char[512];
            Array.Copy(this.RtmpUrl.ToCharArray(), charBuffer, this.RtmpUrl.Length);
            channelInfo.fCfgInfo.destRtmpAddr = charBuffer;

            channelInfo.fCfgInfo.option = this.Option;
            channelInfo.fNVSHandle = this.rtspClient.RtspHandle;
            //分配用户自定义数据指针缓冲区
            this.userPtr = Marshal.AllocHGlobal(Marshal.SizeOf(channelInfo));
            //拷贝结构体数据作为用户自定义数据
            Marshal.StructureToPtr(channelInfo, this.userPtr, true);
            //rtsp 连接
            ret = this.rtspClient.OpenStream(this.RtspUrl, this.userPtr, 0);
            if(!ret.Key)
            {
                this.StopRtsp();
                return ret;
            }
            return new KeyValuePair<bool, string>(true, "rtsp 已启动!"); 
        }

        /// <summary>
        /// 停止 rtsp
        /// </summary>
        public KeyValuePair<bool, string> StopRtsp()
        {
            //释放指针
            if(this.userPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.userPtr);
                this.userPtr = IntPtr.Zero;
            }
            if(this.rtspClient.IsInit)
            {
                this.rtspClient.Deinit();
            }
            return new KeyValuePair<bool, string>(true, "");
        }

        /// <summary>
        /// 启动 rtmp
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> StartRtmp()
        {
            //rtmp 初始化
            var ret = this.rtmpClient.Create();
            if(!ret.Key)
            {
                this.StopRtmp();
                return ret;
            }
            //rtmp 回调
            ret = this.rtmpClient.SetCallback(this.RtmpCallBack, this.userPtr);
            if(!ret.Key)
            {
                this.StopRtmp();
                return ret;
            }
            //rtmp 
            ret = this.rtmpClient.Connect(this.RtmpUrl);
            if(!ret.Key)
            {
                this.StopRtmp();
                return ret;
            }
            return new KeyValuePair<bool, string>(true, "rtmp 已启动!");
        }

        /// <summary>
        /// 停止 rtmp
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<bool, string> StopRtmp()
        {
            if(this.rtmpClient.IsInit)
            {
                this.rtmpClient.Release();
            }
            return new KeyValuePair<bool, string>(true, "");
        }

        /// <summary>
        /// rtsp 回调
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="channelPtr"></param>
        /// <param name="frameType"></param>
        /// <param name="pBuf"></param>
        /// <param name="frameInfo"></param>
        /// <returns></returns>
        private int RtspCallBack(int channelId, IntPtr channelPtr, int frameType, IntPtr pBuf, IntPtr frameInfoPtr)
        {
            //视频帧处理
            if(frameType == EasyTypes.EASY_SDK_VIDEO_FRAME_FLAG)
            {
                //是否存在帧
                if(frameInfoPtr != IntPtr.Zero)
                {
                    this.OnRtspConnectChanged(true, "RTSP 正在接收!");
                    //获取帧信息
                    EasyTypes.EASY_FRAME_INFO frameInfo = (EasyTypes.EASY_FRAME_INFO)Marshal.PtrToStructure(frameInfoPtr, typeof(EasyTypes.EASY_FRAME_INFO));
                    if(frameInfo.type == EasyTypes.EASY_SDK_VIDEO_FRAME_I)//处理I帧
                    {
                        this.ProcessVideoFrame_I(channelPtr, frameInfoPtr, pBuf);
                    }
                    else//处理其他帧
                    {
                        this.ProcessVideoFrame(channelPtr, frameInfoPtr, pBuf);
                    }
                }
            }

            //音频侦处理
            if(frameType == EasyTypes.EASY_SDK_AUDIO_FRAME_FLAG)
            {
                this.ProcessAudioFrame(channelPtr, frameInfoPtr, pBuf);
            }

            //媒体类型侦处理
            if(frameType == EasyTypes.EASY_SDK_MEDIA_INFO_FLAG)
            {
                this.ProcessMediaFrame(channelPtr, frameInfoPtr, pBuf);
            }

            //事件帧处理
            if(frameType == EasyTypes.EASY_SDK_EVENT_FRAME_FLAG)
            {
                this.ProcessEventFrame(channelPtr, frameInfoPtr, pBuf);
            }

            return 0;
        }

        /// <summary>
        /// 处理RTSP视频I帧
        /// </summary>
        /// <param name="channelPtr"></param>
        /// <param name="frameInfoPtr"></param>
        /// <param name="pBuf"></param>
        /// <returns></returns>
        private int ProcessVideoFrame_I(IntPtr channelPtr, IntPtr frameInfoPtr, IntPtr pBuf)
        {
            //获取通道信息
            _channel_info channelInfo = (_channel_info)Marshal.PtrToStructure(channelPtr, typeof(_channel_info));
            //获取帧信息
            EasyTypes.EASY_FRAME_INFO frameInfo = (EasyTypes.EASY_FRAME_INFO)Marshal.PtrToStructure(frameInfoPtr, typeof(EasyTypes.EASY_FRAME_INFO));
            //RTMP初始化
            if(!this.rtmpClient.IsInit)
            {
                //启动 rtmp
                var retRtmp = this.StartRtmp();
                if(!retRtmp.Key)
                {
                    return 0;
                }
                EasyTypes.EASY_MEDIA_INFO_T mediaInfo = EasyTypes.Create_EASY_MEDIA_INFO_T();
                mediaInfo.u32VideoFps = channelInfo.fMediainfo.u32VideoFps;
                mediaInfo.u32AudioSamplerate = channelInfo.fMediainfo.u32AudioSamplerate ;/* 音频采样率 */
                mediaInfo.u32AudioChannel = channelInfo.fMediainfo.u32AudioChannel;/* 音频通道数 */
                mediaInfo.u32AudioBitsPerSample = channelInfo.fMediainfo.u32AudioBitsPerSample;/* 音频采样精度 */
                //媒体信息
                retRtmp = this.rtmpClient.InitMetadata(mediaInfo);
                if(!retRtmp.Key)
                {
                    //return;
                }
            }
            EasyTypes.EASY_AV_Frame avFrame = new EasyTypes.EASY_AV_Frame();
            avFrame.u32AVFrameFlag = EasyTypes.EASY_SDK_VIDEO_FRAME_FLAG;
            avFrame.u32AVFrameLen = frameInfo.length;
            avFrame.pBuffer = pBuf;
            avFrame.u32VFrameType = EasyTypes.EASY_SDK_VIDEO_FRAME_I;
            //发送数据包
            var ret = this.rtmpClient.SendPacket(avFrame);
            if(!ret.Key)
            {
                //return;
            }
            return 0;
        }

        /// <summary>
        /// 处理RTSP视频帧
        /// </summary>
        /// <param name="channelPtr"></param>
        /// <param name="frameInfoPtr"></param>
        /// <param name="pBuf"></param>
        /// <returns></returns>
        private int ProcessVideoFrame(IntPtr channelPtr, IntPtr frameInfoPtr, IntPtr pBuf)
        {
            //判断RTMP是否初始化
            if(!this.rtmpClient.IsInit)
            {
                return 0;
            }
            //获取帧信息
            EasyTypes.EASY_FRAME_INFO frameInfo = (EasyTypes.EASY_FRAME_INFO)Marshal.PtrToStructure(frameInfoPtr, typeof(EasyTypes.EASY_FRAME_INFO));

            EasyTypes.EASY_AV_Frame avFrame = new EasyTypes.EASY_AV_Frame();
            avFrame.u32AVFrameFlag = EasyTypes.EASY_SDK_VIDEO_FRAME_FLAG;
            //avFrame.u32AVFrameLen = frameinfo->length-4;
            avFrame.u32AVFrameLen = frameInfo.length - 4;
            //avFrame.pBuffer = (unsigned char*)pbuf+4;
            avFrame.pBuffer = new IntPtr(pBuf.ToInt64() + 4);
            avFrame.u32VFrameType = EasyTypes.EASY_SDK_VIDEO_FRAME_P;
            //发送数据包
            var ret = this.rtmpClient.SendPacket(avFrame);
            if(!ret.Key)
            {
            //return;
            }

            return 0;
        }

        /// <summary>
        /// 处理RTSP音频帧
        /// </summary>
        /// <param name="channelPtr"></param>
        /// <param name="frameInfoPtr"></param>
        /// <param name="pBuf"></param>
        /// <returns></returns>
        private int ProcessAudioFrame(IntPtr channelPtr, IntPtr frameInfoPtr, IntPtr pBuf)
        {
            return 0;
        }

        /// <summary>
        /// 处理RTSP媒体信息帧
        /// </summary>
        /// <param name="channelPtr"></param>
        /// <param name="frameInfoPtr"></param>
        /// <param name="pBuf"></param>
        /// <returns></returns>
        private int ProcessMediaFrame(IntPtr channelPtr, IntPtr frameInfoPtr, IntPtr pBuf)
        {
            if(pBuf == IntPtr.Zero)
            {
                return 0;
            }
            this.OnRtspConnectChanged(true, "RTSP 已连接!");
            //获取通道信息
            _channel_info channelInfo = (_channel_info)Marshal.PtrToStructure(channelPtr, typeof(_channel_info));
            //从pBuf中获取媒体信息
            var mediainfo = (EasyTypes.EASY_MEDIA_INFO_T)Marshal.PtrToStructure(pBuf,typeof(EasyTypes.EASY_MEDIA_INFO_T));
            //更新通道媒体信息
            channelInfo.fMediainfo = mediainfo;
            //更新通道信息指针
            Marshal.StructureToPtr(channelInfo, channelPtr, true);

            return 0;
        }

        /// <summary>
        /// 处理RTSP事件帧
        /// </summary>
        /// <param name="channelInfo"></param>
        /// <param name="frameInfoPtr"></param>
        /// <param name="pBuf"></param>
        /// <returns></returns>
        private int ProcessEventFrame(IntPtr channelPtr, IntPtr frameInfoPtr, IntPtr pBuf)
        {
            //开始进行连接，建立EasyRTSPClient连接线程
            if(pBuf == IntPtr.Zero && frameInfoPtr == IntPtr.Zero)
            {
                this.OnRtspConnectChanged(false, "RTSP 正在连接!");
                return 0;
            }

            //是否存在帧
            bool hasFrame = frameInfoPtr != IntPtr.Zero;

            //获取帧信息
            EasyTypes.EASY_FRAME_INFO frameInfo = hasFrame ? (EasyTypes.EASY_FRAME_INFO)Marshal.PtrToStructure(frameInfoPtr, typeof(EasyTypes.EASY_FRAME_INFO)) : new EasyTypes.EASY_FRAME_INFO();
            
            //连接线程退出，此时上层应该停止相关调用，复位连接按钮等状态
            if(hasFrame && frameInfo.codec == EasyTypes.EASY_SDK_EVENT_CODEC_EXIT)
            {
                this.OnRtspConnectChanged(false, "RTSP 连接退出!");
                return 0;
            }

            //连接失败
            if(hasFrame && frameInfo.codec == EasyTypes.EASY_SDK_EVENT_CODEC_FAIL)
            {
                this.OnRtspConnectChanged(false, "RTSP 连接失败!");
                return 0;
            }

            //连接错误，错误码通过EasyRTSP_GetErrCode()接口获取，比如404
            if(hasFrame && frameInfo.codec == EasyTypes.EASY_SDK_EVENT_CODEC_ERROR)
            {
                int errorCode;
                this.rtspClient.GetErrCode(out errorCode);
                string message = errorCode.ToString();
                if(errorCode == 400)
                {
                    message = "Bad Request";
                }
                else if(errorCode == 401)
                {
                    message = "Unauthorized";
                }
                else if(errorCode == 403)
                {
                    message = "Forbidden";
                }
                else if(errorCode == 404)
                {
                    message = "Not Found";
                }
                else if(errorCode == 500)
                {
                    message = "Internal Server Error";
                }
                else 
                {
                    message = errorCode.ToString();
                }
                this.OnRtspConnectChanged(false, string.Concat("RTSP 连接错误:", message));
            }

            return 0;
        }

        /// <summary>
        /// rtmp 回调
        /// </summary>
        /// <param name="frameType"></param>
        /// <param name="pBuf"></param>
        /// <param name="state"></param>
        /// <param name="userPtr"></param>
        /// <returns></returns>
        private int RtmpCallBack(int frameType, IntPtr pBuf, EasyRTMPAPI.EASY_RTMP_STATE_T state, IntPtr userPtr)
        {
            //获取通道信息
            _channel_info channelInfo = (_channel_info)Marshal.PtrToStructure(userPtr, typeof(_channel_info));
            switch(state)
            {
                case EasyRTMPAPI.EASY_RTMP_STATE_T.EASY_RTMP_STATE_CONNECTING:
                    this.OnRtmpConnectChanged(false, "RTMP 正在连接!");
                    break;
                case EasyRTMPAPI.EASY_RTMP_STATE_T.EASY_RTMP_STATE_CONNECTED:
                    this.OnRtmpConnectChanged(false, "RTMP 已连接!");
                    break;
                case EasyRTMPAPI.EASY_RTMP_STATE_T.EASY_RTMP_STATE_CONNECT_FAILED:
                    this.OnRtmpConnectChanged(false, "RTMP 连接失败!");
                    break;
                case EasyRTMPAPI.EASY_RTMP_STATE_T.EASY_RTMP_STATE_CONNECT_ABORT:
                    this.OnRtmpConnectChanged(false, "RTMP 连接中断!");
                    break;
                case EasyRTMPAPI.EASY_RTMP_STATE_T.EASY_RTMP_STATE_DISCONNECTED:
                    this.OnRtmpConnectChanged(false, "RTMP 连接断开!");
                    break;
                default:
                    break;
            }
            return 0;
        }

        #region 结构体

        /// <summary>
        /// 通道配置信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct _channel_cfg
        {
            public System.Int32 channelId;

            public System.Int32 option;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public System.Char[] channelName;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public System.Char[] srcRtspAddr;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public System.Char[] destRtmpAddr;
        }

        /// <summary>
        /// rtmp 推流信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct _rtmp_pusher
        {
            public IntPtr aacEncHandle;

            public IntPtr rtmpHandle;

            public System.UInt32 u32AudioCodec;

            public System.UInt32 u32AudioSamplerate;

            public System.UInt32 u32AudioChannel;

            public IntPtr pAACCacheBuffer;
        }

        /// <summary>
        /// 通道信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct _channel_info
        {   
            /// <summary>
            /// 通道配置信息
            /// </summary>
            public _channel_cfg fCfgInfo;

            /// <summary>
            /// rtmp 推流信息
            /// </summary>
            public _rtmp_pusher fPusherInfo;

            /// <summary>
            /// rtsp 句柄
            /// </summary>
            public IntPtr fNVSHandle;

            public IntPtr fLogHandle;

            public bool fHavePrintKeyInfo;

            /// <summary>
            /// 媒体信息
            /// </summary>
            public EasyTypes.EASY_MEDIA_INFO_T fMediainfo;
        }

        #endregion
    }

}