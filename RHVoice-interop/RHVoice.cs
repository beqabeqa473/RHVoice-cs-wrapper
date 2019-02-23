using NAudio.Utils;
using NAudio.Wave;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace RHVoice_interop
{
    public class RHVoice
    {
        MemoryStream stream;
        private IntPtr engine;
        private RHVoiceCallbacks.PlaySpeech playSpeechCallbackDelegate;

        public RHVoice()
        {
            var config = new RHVoiceInitParams();
            config.Callbacks = new RHVoiceCallbacks();
            playSpeechCallbackDelegate = playSpeech;
            config.Callbacks.OnPlaySpeech = playSpeechCallbackDelegate;
            engine = NativeMethods.RHVoice_new_tts_engine(config);
            if (engine == IntPtr.Zero)
                throw new Exception("RHVoice initialization error");
		}

        private   int playSpeech(IntPtr dataPtr, int count, IntPtr user_data)
        {
            byte[] samples = new byte[count * 2];
            Marshal.Copy(dataPtr, samples, 0, samples.Length);
            stream.Write(samples, 0, samples.Length);
            return 1;
        }

        public  string GetVersion()
        {
            var version = NativeMethods.RHVoice_get_version();
            return Marshal.PtrToStringAnsi(version);
        }

        public uint GetVoiceCount()
        {
            return NativeMethods.RHVoice_get_number_of_voices(engine);
        }

        public uint GetVoiceProfilesCount()
        {
            return NativeMethods.RHVoice_get_number_of_voice_profiles(engine);
        }

        public VoiceInfo[] GetVoices()
        {
                uint count = GetVoiceCount();
                if (count == 0)
                    return new VoiceInfo[0];

                var voices = new VoiceInfo[count];
                int itemSize = Marshal.SizeOf(typeof(VoiceInfo));

                var nativeVoices = NativeMethods.RHVoice_get_voices(engine);
                if (nativeVoices == IntPtr.Zero)
                    throw new Exception("Cannot get voices");

                for (int i = 0; i < count; i++)
                {
                    var voice = (VoiceInfo)Marshal.PtrToStructure(nativeVoices, typeof(VoiceInfo));
                    voices[i] = voice;
                    nativeVoices += itemSize;
                }
                return voices;
        }

        public string[] GetVoiceProfiles()
        {
                var count = NativeMethods.RHVoice_get_number_of_voice_profiles(engine);
                if (count == 0)
                    return new string[0];
                var nativeVoiceProfiles = NativeMethods.RHVoice_get_voice_profiles(engine);
                if (nativeVoiceProfiles == IntPtr.Zero)
                    throw new Exception("Cannot get voice profiles");

                var profiles = new string[count];
                for (int i = 0; i < count; i++, nativeVoiceProfiles += sizeof(int) * i)
                {
                    var profile = Marshal.PtrToStringAnsi((IntPtr)Marshal.PtrToStructure(nativeVoiceProfiles, typeof(IntPtr)));
                    profiles[i] = profile;
                }
                return profiles;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Stream Speak(SynthParams p)
        {
            var text = Encoding.UTF8.GetBytes("Конвенция о правах инвалидов (ратифицирована федеральным законом РФ от 03.05.2012 N 46-ФЗ).");
            var message = NativeMethods.RHVoice_new_message(engine, text, (uint)text.Length, MessageType.Text, p, IntPtr.Zero);
            using (stream = new MemoryStream())
            {
                NativeMethods.RHVoice_speak(message);
                NativeMethods.RHVoice_delete_message(message);
                stream.Position = 0;
                var sampleRate = 16000;
                var waveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, sampleRate, 1, sampleRate * 2, 2, 16);
                using (var reader = new RawSourceWaveStream(stream, waveFormat))
                {
                    var waveMemoryStream = new MemoryStream(stream.Capacity);
                    using (var writer = new WaveFileWriter(new IgnoreDisposeStream(waveMemoryStream), waveFormat))
                    {
                        reader.CopyTo(writer);
                    }
                    waveMemoryStream.Position = 0;
                    return waveMemoryStream;
                }
            }
        }

        public void Dispose()
        {
            if (engine == IntPtr.Zero)
                return;
            NativeMethods.RHVoice_delete_tts_engine(engine);
            engine = IntPtr.Zero;
        }
    }
}
