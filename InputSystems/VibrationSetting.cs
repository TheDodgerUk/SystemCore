using System.Collections.Generic;

public enum VibrateType
{
    Tick,
    Pulse,
    Buzz,
    Rumble,
}

public class VibrationSetting
{
    private static readonly Dictionary<VibrateType, VibrationSetting> VibrationSettings = new Dictionary<VibrateType, VibrationSetting>
    {
        { VibrateType.Tick, new VibrationSetting(0.5f, 0.2f) },
        { VibrateType.Pulse, new VibrationSetting(0.2f, 0.02f) },
        { VibrateType.Buzz, new VibrationSetting(0.5f, 0.02f) },
        { VibrateType.Rumble, new VibrationSetting(0.5f, 0.5f) },
    };
    public static VibrationSetting Get(VibrateType type) => VibrationSettings[type];

    public float Intensity { get; private set; }
    public float Duration { get; private set; }

    private VibrationSetting(float intensity, float duration)
    {
        Intensity = intensity;
        Duration = duration;
    }

}
