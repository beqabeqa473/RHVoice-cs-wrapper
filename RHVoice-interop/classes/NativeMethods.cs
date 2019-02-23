using System;
using System.IO;
using System.Runtime.InteropServices;

namespace RHVoice_interop
{
    public static class NativeMethods
    {

        static NativeMethods()
        {
            var myPath = new Uri(typeof(NativeMethods).Assembly.CodeBase).LocalPath;
            var myFolder = Path.GetDirectoryName(myPath);
            var is64 = IntPtr.Size == 8;
            var subfolder = is64 ? "\\lib\\x64\\" : "\\lib\\x86\\";

            LoadLibrary(myFolder + subfolder + "RHVoice.dll");
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);
        [DllImport("RHVoice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RHVoice_get_version();
        [DllImport("RHVoice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RHVoice_new_tts_engine(RHVoiceInitParams initParams);
        [DllImport("RHVoice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RHVoice_delete_tts_engine(IntPtr engine);
        [DllImport("RHVoice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint RHVoice_get_number_of_voices(IntPtr engine);
        [DllImport("RHVoice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RHVoice_get_voices(IntPtr engine);
        [DllImport("RHVoice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint RHVoice_get_number_of_voice_profiles(IntPtr engine);
        [DllImport("RHVoice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RHVoice_get_voice_profiles(IntPtr engine);
        [DllImport("RHVoice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr RHVoice_new_message(IntPtr engine, byte[] text, uint length, MessageType messageType, SynthParams synthParams, IntPtr user_data);
        [DllImport("RHVoice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RHVoice_delete_message(IntPtr message);
        [DllImport("RHVoice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RHVoice_speak(IntPtr message);
    }
}
