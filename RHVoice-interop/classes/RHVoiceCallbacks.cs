using System;
using System.Runtime.InteropServices;

namespace RHVoice_interop
{
    [StructLayout(LayoutKind.Sequential)]
    public class RHVoiceCallbacks
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int PlaySpeech(IntPtr samples, int count, IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int ProcessMark([MarshalAs(UnmanagedType.LPStr)] string name, IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int WordStarts(UIntPtr position, UIntPtr length, IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int WordEnds(UIntPtr position, UIntPtr length, IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SentenceStarts(UIntPtr position, UIntPtr length, IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SentenceEnds(UIntPtr position, UIntPtr length, IntPtr user_data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int PlayAudio([MarshalAs(UnmanagedType.LPStr)] string src, IntPtr user_data);

        public PlaySpeech OnPlaySpeech;
        public ProcessMark OnProcessMark;
        public WordStarts OnWordStarts;
        public WordEnds OnWordEnds;
        public SentenceStarts OnSentenceStarts;
        public SentenceEnds OnSentenceEnds;
        public PlayAudio OnPlayAudio;
    }

}
