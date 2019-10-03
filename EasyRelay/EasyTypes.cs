using System;
using System.Runtime.InteropServices;

namespace EasyRelay
{
    public class EasyTypes
    {
        #region 常量/枚举

        /* 视频编码 */
        public const int EASY_SDK_VIDEO_CODEC_H264 =	0x1C;		/* H264  */
        public const int EASY_SDK_VIDEO_CODEC_H265 =	0xAE;	  /* H265 */
        public const int EASY_SDK_VIDEO_CODEC_MJPEG =	0x08;		/* MJPEG */
        public const int EASY_SDK_VIDEO_CODEC_MPEG4 =	0x0D;		/* MPEG4 */

        /* 音频编码 */
        public const int EASY_SDK_AUDIO_CODEC_AAC =	0x15002;		/* AAC */
        public const int EASY_SDK_AUDIO_CODEC_G711U =	0x10006;		/* G711 ulaw*/
        public const int EASY_SDK_AUDIO_CODEC_G711A =	0x10007;		/* G711 alaw*/
        public const int EASY_SDK_AUDIO_CODEC_G726 =	0x1100B;		/* G726 */

        public const int EASY_SDK_EVENT_CODEC_ERROR =	0x63657272;	/* ERROR */
        public const int EASY_SDK_EVENT_CODEC_EXIT =	0x65786974;	/* EXIT */
        public const int EASY_SDK_EVENT_CODEC_FAIL = 0x7265636F;/* Fail */

        /* 音视频帧标识 */
        public const int EASY_SDK_VIDEO_FRAME_FLAG =	0x00000001;		/* 视频帧标志 */
        public const int EASY_SDK_AUDIO_FRAME_FLAG =	0x00000002;		/* 音频帧标志 */
        public const int EASY_SDK_EVENT_FRAME_FLAG =	0x00000004;		/* 事件帧标志 */
        public const int EASY_SDK_RTP_FRAME_FLAG =		0x00000008;		/* RTP帧标志 */
        public const int EASY_SDK_SDP_FRAME_FLAG =		0x00000010;		/* SDP帧标志 */
        public const int EASY_SDK_MEDIA_INFO_FLAG =	0x00000020;		/* 媒体类型标志*/
        public const int EASY_SDK_SNAP_FRAME_FLAG =	0x00000040;		/* 图片标志*/

        /* 视频关键字标识 */
        public const int EASY_SDK_VIDEO_FRAME_I =		0x01;		/* I帧 */
        public const int EASY_SDK_VIDEO_FRAME_P =		0x02;		/* P帧 */
        public const int EASY_SDK_VIDEO_FRAME_B =		0x03;		/* B帧 */
        public const int EASY_SDK_VIDEO_FRAME_J =		0x04;		/* JPEG */

        public enum Easy_Error
        {
            Easy_NoErr						= 0,
            Easy_RequestFailed				= -1,
            Easy_Unimplemented				= -2,
            Easy_RequestArrived				= -3,
            Easy_OutOfState					= -4,
            Easy_NotAModule					= -5,
            Easy_WrongVersion				= -6,
            Easy_IllegalService				= -7,
            Easy_BadIndex					= -8,
            Easy_ValueNotFound				= -9,
            Easy_BadArgument				= -10,
            Easy_ReadOnly					= -11,
            Easy_NotPreemptiveSafe			= -12,
            Easy_NotEnoughSpace				= -13,
            Easy_WouldBlock					= -14,
            Easy_NotConnected				= -15,
            Easy_FileNotFound				= -16,
            Easy_NoMoreData					= -17,
            Easy_AttrDoesntExist			= -18,
            Easy_AttrNameExists				= -19,
            Easy_InstanceAttrsNotAllowed	= -20,
            Easy_InvalidSocket				= -21,
            Easy_MallocError				= -22,
            Easy_ConnectError				= -23,
            Easy_SendError					= -24
        }

        public enum EASY_ACTIVATE_ERR_CODE_ENUM
        {
            EASY_ACTIVATE_INVALID_KEY		=		-1,			/* 无效Key */
            EASY_ACTIVATE_TIME_ERR			=		-2,			/* 时间错误 */
            EASY_ACTIVATE_PROCESS_NAME_LEN_ERR	=	-3,			/* 进程名称长度不匹配 */
            EASY_ACTIVATE_PROCESS_NAME_ERR	=		-4,			/* 进程名称不匹配 */
            EASY_ACTIVATE_VALIDITY_PERIOD_ERR=		-5,			/* 有效期校验不一致 */
            EASY_ACTIVATE_PLATFORM_ERR		=		-6,			/* 平台不匹配 */
            EASY_ACTIVATE_COMPANY_ID_LEN_ERR=		-7,			/* 授权使用商不匹配 */
            EASY_ACTIVATE_SUCCESS			=		9999,		/* 激活成功 */
        }

        #endregion

        /// <summary>
        /// 帧信息
        /// </summary>
        public struct EASY_FRAME_INFO
        {
            /// <summary>
            /// 音视频格式
            /// </summary>
            public System.UInt32 codec;
            /// <summary>
            /// 视频帧类型
            /// </summary>
            public System.UInt32 type;
            /// <summary>
            /// 视频帧率
            /// </summary>
            public System.Byte fps;
            /// <summary>
            /// 视频宽
            /// </summary>
            public System.UInt16 width;
            /// <summary>
            /// 视频高
            /// </summary>
            public System.UInt16 height;
            /// <summary>
            /// 保留参数1
            /// </summary>
            public System.UInt32 reserved1;
            /// <summary>
            /// 保留参数2
            /// </summary>
            public System.UInt32 reserved2;
            /// <summary>
            /// 音频采样率
            /// </summary>
            public System.UInt32 sample_rate;
            /// <summary>
            /// 音频声道数
            /// </summary>
            public System.UInt32 channels;
            /// <summary>
            /// 音频采样精度
            /// </summary>
            public System.UInt32 bits_per_sample;
            /// <summary>
            /// 音视频帧大小
            /// </summary>
            public System.UInt32 length;
            /// <summary>
            /// 时间戳,微妙
            /// </summary>
            public System.UInt32 timestamp_usec;
            /// <summary>
            /// 时间戳 秒
            /// </summary>
            public System.UInt32 timestamp_sec;
            /// <summary>
            /// 比特率
            /// </summary>
            public System.Single bitrate;
            /// <summary>
            /// 丢包率
            /// </summary>
            public System.Single losspacket;
        }

        /*
        [StructLayout(LayoutKind.Explicit)]
        public struct EASY_FRAME_INFO
        {
            /// <summary>
            /// 音视频格式
            /// </summary>
            [FieldOffset(0)]
            public System.UInt32 codec;
            /// <summary>
            /// 视频帧类型
            /// </summary>
            [FieldOffset(0)]
            public System.UInt32 type;
            /// <summary>
            /// 视频帧率
            /// </summary>
            [FieldOffset(0)]
            public System.Byte fps;
            /// <summary>
            /// 视频宽
            /// </summary>
            [FieldOffset(0)]
            public System.UInt16 width;
            /// <summary>
            /// 视频高
            /// </summary>
            [FieldOffset(0)]
            public System.UInt16 height;
            /// <summary>
            /// 保留参数1
            /// </summary>
            [FieldOffset(0)]
            public System.UInt32 reserved1;
            /// <summary>
            /// 保留参数2
            /// </summary>
            [FieldOffset(0)]
            public System.UInt32 reserved2;
            /// <summary>
            /// 音频采样率
            /// </summary>
            [FieldOffset(0)]
            public System.UInt32 sample_rate;
            /// <summary>
            /// 音频声道数
            /// </summary>
            [FieldOffset(0)]
            public System.UInt32 channels;
            /// <summary>
            /// 音频采样精度
            /// </summary>
            [FieldOffset(0)]
            public System.UInt32 bits_per_sample;
            /// <summary>
            /// 音视频帧大小
            /// </summary>
            [FieldOffset(0)]
            public System.UInt32 length;
            /// <summary>
            /// 时间戳,微妙
            /// </summary>
            [FieldOffset(0)]
            public System.UInt32 timestamp_usec;
            /// <summary>
            /// 时间戳 秒
            /// </summary>
            [FieldOffset(0)]
            public System.UInt32 timestamp_sec;
            /// <summary>
            /// 比特率
            /// </summary>
            [FieldOffset(0)]
            public System.Single bitrate;
            /// <summary>
            /// 丢包率
            /// </summary>
            [FieldOffset(0)]
            public System.Single losspacket;
        }
        */

        /// <summary>
        /// 媒体信息
        /// </summary>
        public struct EASY_MEDIA_INFO_T
        {
            public System.UInt32 u32VideoCodec;				/* 视频编码类型 */
            public System.UInt32 u32VideoFps;				/* 视频帧率 */
            public System.UInt32 u32AudioCodec;				/* 音频编码类型 */
            public System.UInt32 u32AudioSamplerate;		/* 音频采样率 */
            public System.UInt32 u32AudioChannel;			/* 音频通道数 */
            public System.UInt32 u32AudioBitsPerSample;		/* 音频采样精度 */
            public System.UInt32 u32VpsLength;
            public System.UInt32 u32SpsLength;
            public System.UInt32 u32PpsLength;
            public System.UInt32 u32SeiLength;
            //char	 u8Vps[256];
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public System.Byte[] u8Vps;
            //char	 u8Sps[256];
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public System.Byte[] u8Sps;
            //char	 u8Pps[128];
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public System.Byte[] u8Pps;
            //char	 u8Sei[128];
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public System.Byte[] u8Sei;
        }

        public static EASY_MEDIA_INFO_T Create_EASY_MEDIA_INFO_T()
        {
            EASY_MEDIA_INFO_T mediainfo = new EASY_MEDIA_INFO_T();
            mediainfo.u8Vps = new System.Byte[256];
            mediainfo.u8Sps = new System.Byte[256];
            mediainfo.u8Pps = new System.Byte[128];
            mediainfo.u8Sei = new System.Byte[128];
            return mediainfo;
        }

        /// <summary>
        /// 视频/音频侦信息
        /// </summary>
        public struct EASY_AV_Frame
        {
            /// <summary>
            /// 帧标志  视频 or 音频
            /// </summary>
            public System.UInt32 u32AVFrameFlag;

            /// <summary>
            /// 帧的长度
            /// </summary>
            public System.UInt32 u32AVFrameLen;

            /// <summary>
            /// 视频的类型，I帧或P帧
            /// </summary>
            public System.UInt32 u32VFrameType;

            /// <summary>
            /// 数据
            /// </summary>
            public IntPtr pBuffer;

            /// <summary>
            /// 时间戳(秒)
            /// </summary>
            public System.UInt32 u32TimestampSec;

            /// <summary>
            /// 时间戳(微秒) 
            /// </summary>
            public System.UInt32 u32TimestampUsec;
        }
    }
 
}