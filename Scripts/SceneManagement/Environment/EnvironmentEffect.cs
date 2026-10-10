using System.Collections.Generic;
using UnityEngine;

public abstract class EnvironmentEffect : MonoBehaviour
{
    public virtual bool Initialise() { return true; }

    public virtual void OnEnvironmentEffectsLoaded() { }

    public abstract void OnSplashReset();

    public abstract void OnSplashComplete();

    public virtual void SetLayer(int layer) { }

    protected string GetObjectName()
    {
        string name = GetObjectName(this.gameObject);
        if(null == name)
        {
            Debug.LogError($"Could not parse video name from {this.gameObject}\n");
        }
        return name;
    }

    public static string GetObjectName(GameObject gameObjectRef)
    {
        var split = gameObjectRef.name.Split('_');
        if (split.Length > 1)
        {
            return split[1];
        }
        else
        {
            return gameObjectRef.name;
        }
    }


    public List<string> GetObjectNameExtention()
    {
        return GetObjectNameExtentionArguments(this.gameObject);
    }

    public static List<string> GetObjectNameExtentionArguments(GameObject gameObjectRef)
    {
        var split = gameObjectRef.name.Split('_');
        List<string> argumnets = new List<string>(split);
        try
        {
            argumnets.RemoveAt(0);
            argumnets.RemoveAt(0);
        }
        catch
        {
            Debug.LogError($"There was an error with GetObjectNameExtention:  {gameObjectRef.GetGameObjectPath()}");
        }
        return argumnets;
    }

    public List<string> GetObjectNameExtentionArguments()
    {
        return GetObjectNameExtentionArguments(this.gameObject);
    }

    public static string GetObjectNameExtensionArguments(GameObject gameObjectRef, int argumentIndex)
    {
        List<string> arguments = GetObjectNameExtentionArguments(gameObjectRef);
        if(arguments.Count > argumentIndex)
        {
            return arguments[argumentIndex];
        }
        return null;
    }
}