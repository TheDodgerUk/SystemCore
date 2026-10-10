
using UnityEngine;

public class DeltaComputer<T>
{
    public delegate T DeltaHandler(T from, T to);

    public T Delta => m_ComputeDelta(Previous, Current);
    public T Total => m_ComputeDelta(Start, Current);

    public T Previous { get; private set; }
    public T Current { get; private set; }
    public T Start { get; private set; }

    private DeltaHandler m_ComputeDelta;

    public DeltaComputer(DeltaHandler computeDelta)
    {
        m_ComputeDelta = computeDelta;
    }

    public void SetCurrent(T current)
    {
        Previous = Current;
        Current = current;
    }

    public void SetStart(T start)
    {
        Start = Previous = Current = start;
    }

    public override string ToString() => Delta.ToString();
}

public class DeltaQuaternion: DeltaComputer<Quaternion>
{
    public DeltaQuaternion() : base((from, to) => from.Delta(to)) { }
}

public class DeltaVector3 : DeltaComputer<Vector3>
{
    public DeltaVector3() : base((from, to) => to - from) { }
}

public class DeltaFloat : DeltaComputer<float>
{
    public DeltaFloat() : base((from, to) => to - from) { }
}