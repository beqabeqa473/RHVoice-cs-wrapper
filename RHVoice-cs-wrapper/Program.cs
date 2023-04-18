using RHVoice_interop;

namespace RHVoice_cs_wrapper;

class Program
{

    static void Main(string[] args)
    {
        using var rhvoice = new RHVoice();
        Console.WriteLine($"RHVoice version: {rhvoice.GetVersion()}");
        Console.WriteLine($"Voices available: {rhvoice.GetVoiceCount()}");
        var voices = rhvoice.GetVoices();
        foreach (var voice in voices)
        {
            Console.WriteLine(voice.Language);
            Console.WriteLine(voice.Name);
            Console.WriteLine(voice.Gender);
            Console.WriteLine(voice.Country);
        }
        Console.WriteLine($"Profiles available: {rhvoice.GetVoiceProfilesCount()}");
        var profiles = rhvoice.GetVoiceProfiles();
        foreach (var profile in profiles)
        {
        Console.WriteLine(profile);
        }
        var p = new SynthParams
        {
            VoiceProfile = "Anna",
            RelativeRate = 3.0
        };
        string msg;
        while ((msg = Console.ReadLine()) != "exit")
        {
            rhvoice.Speak(msg, p, false);
        }
    }

}
