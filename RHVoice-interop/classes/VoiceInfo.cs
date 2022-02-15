namespace RHVoice_interop;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public class VoiceInfo
{
    public string Language;
    public string Name;
    public Gender Gender;
    public string Country;
}
