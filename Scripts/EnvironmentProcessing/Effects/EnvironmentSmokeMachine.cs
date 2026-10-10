
public class EnvironmentSmokeMachine : EnvironmentParticleEffect
{
    protected override string[] AudioNames => new[] { "SmokeMachine", "SmokeFan" };
    protected override string PrefabName => "SmokeFx";

    private RotateObject m_Fan;

    protected override void Awake()
    {
        base.Awake();

        m_FireDuration = 25f;

        // find fan and add rotation script
        var fan = transform.parent.Search("Fan");
        m_Fan = fan.AddComponent<RotateObject>();
        m_Fan.SetSpeed(0, 0, 500f);
        m_Fan.enabled = false;

        GetSfx(AudioNames[1], (item) =>
        {
            item.transform.OrientTo(fan, false);
        });
    }

    protected override void OnToggled(bool state)
    {
        base.OnToggled(state);

        m_Fan.enabled = state;
    }
}