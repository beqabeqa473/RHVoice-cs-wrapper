using System;
using System.IO;
using System.Media;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace RHVoice_interop
{
    public class RHVoice: IDisposable
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
        public void Speak(string msg, SynthParams p, bool toFile = false)
        {
            const int headerSize = 44;
            const int formatChunkSize = 16;
            const short waveAudioFormat = 1;
            const short numChannels = 1;
            const int sampleRate = 16000;
            const short bitsPerSample = 16;
            const int byteRate = (numChannels * bitsPerSample * sampleRate) / 8;
            const short blockAlign = numChannels * bitsPerSample / 8;
            var text = Encoding.UTF8.GetBytes(msg);
            var message = NativeMethods.RHVoice_new_message(engine, text, (uint)text.Length, MessageType.Text, p, IntPtr.Zero);
            using (stream = new MemoryStream())
                {
                NativeMethods.RHVoice_speak(message);
                NativeMethods.RHVoice_delete_message(message);
                var sizeInBytes = (int)stream.Length;
                using (var writer = new MemoryStream())
                {
                    writer.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);
                    writer.Write(BitConverter.GetBytes(sizeInBytes + headerSize - 8), 0, 4);
                    writer.Write(Encoding.ASCII.GetBytes("WAVEfmt "), 0, 8);
                    writer.Write(BitConverter.GetBytes(formatChunkSize), 0, 4);
                    writer.Write(BitConverter.GetBytes(waveAudioFormat), 0, 2);
                    writer.Write(BitConverter.GetBytes(numChannels), 0, 2);
                    writer.Write(BitConverter.GetBytes(sampleRate), 0, 4);
                    writer.Write(BitConverter.GetBytes(byteRate), 0, 4);
                    writer.Write(BitConverter.GetBytes(blockAlign), 0, 2);
                    writer.Write(BitConverter.GetBytes(bitsPerSample), 0, 2);
                    writer.Write(Encoding.ASCII.GetBytes("data"), 0, 4);
                    writer.Write(BitConverter.GetBytes(sizeInBytes), 0, 4);
                    stream.Position = 0;
                   stream.CopyTo(writer);
                    var player = new SoundPlayer(writer);
                    writer.Position = 0;
                    player.Play();
                    if (toFile)
                    {
                        using (var file = File.Create("file.wav"))
                        {
                            writer.Position = 0;
                            writer.CopyTo(file);
                        }
                    }
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
