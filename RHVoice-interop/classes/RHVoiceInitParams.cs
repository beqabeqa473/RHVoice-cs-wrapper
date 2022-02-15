namespace RHVoice_interop;

[StructLayout(LayoutKind.Sequential)]
public class RHVoiceInitParams
{
    public string DataPath;
    public string ConfigPath;
    public IntPtr ResourcePaths;
    public RHVoiceCallbacks Callbacks;
    public RHVoiceEngineOptions Options;
}
