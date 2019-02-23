using NAudio.Utils;
using NAudio.Wave;
using RHVoice_interop;
using System;
using System.IO;
using System.Media;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace RHVoice_cs_wrapper
{
    class Program
    {

        static void Main(string[] args)
        {
            var rhvoice = new RHVoice();
            Console.WriteLine($"RHVoice version: {rhvoice.GetVersion()}");
            Console.WriteLine($"Voices available: {rhvoice.GetVoiceCount()}");
            var voices = rhvoice.GetVoices();
            foreach (var voice in voices)
            {
                Console.WriteLine(voice.Language);
                Console.WriteLine(voice.Name);
                Console.WriteLine(voice.Gender);
            }
                Console.WriteLine($"Profiles available: {rhvoice.GetVoiceProfilesCount()}");
            var profiles = rhvoice.GetVoiceProfiles();
            foreach (var profile in profiles)
            {
                Console.WriteLine(profile);
            }
            var p = new SynthParams();
            p.VoiceProfile = "Aleksandr";
            p.RelativeRate = 3.0;
            using (var waveFile = File.Create("test.wav"))
            using (var spokenAudio = rhvoice.Speak(p))
                spokenAudio.CopyTo(waveFile);
        }


    }
}
