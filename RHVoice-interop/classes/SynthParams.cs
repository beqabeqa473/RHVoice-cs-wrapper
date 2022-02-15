namespace RHVoice_interop;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public class SynthParams
{
    public string VoiceProfile;
    public double AbsoluteRate { get; set; }
    public double AbsolutePitch { get; set; }
    public double AbsoluteVolume { get; set; }
    public double RelativeRate { get; set; } = 1;
    public double RelativePitch { get; set; } = 1;
    public double RelativeVolume { get; set; } = 1;
    public PunctuationMode PunctuationMode { get; set; } = PunctuationMode.Default;
    public string PunctuationList;
    public CapitalsMode CapitalsMode { get; set; } = CapitalsMode.Default;
}
