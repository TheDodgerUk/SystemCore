namespace ConsoleExtraEnum
{
    public enum EDebugTypeBase
    {
        All,
        Audio,

        Animation,
        LoadScene,
        TaskAction,
        Todo,
        StartUp,
        Json, 
        NetworkAsset,
        Photon_Sent,
        Photon_Receive,
        VR, 
        Generic,
        DebugLogging
    }


    public enum EDebugType
    {
        Animation = EDebugTypeBase.Animation,
        LoadScene = EDebugTypeBase.LoadScene,
        TaskAction = EDebugTypeBase.TaskAction,
        Todo = EDebugTypeBase.Todo,
        StartUp = EDebugTypeBase.StartUp,
        Json = EDebugTypeBase.Json,
        NetworkAsset = EDebugTypeBase.NetworkAsset,
        Photon_Sent = EDebugTypeBase.Photon_Sent,
        Photon_Receive = EDebugTypeBase.Photon_Receive,
        VR = EDebugTypeBase.VR,
        Generic = EDebugTypeBase.Generic,
        DebugLogging = EDebugTypeBase.DebugLogging,

    }


    public enum LogError
    {
        /*
        NeedsRefactoring  = EDebugTypeBase.NeedsRefactoring,
        Init              = EDebugTypeBase.Init,
        InteractWith      = EDebugTypeBase.InteractWith,
        ICCInteractObject = EDebugTypeBase.ICCInteractObject,
        MenuCanvas        = EDebugTypeBase.MenuCanvas,
        Menu              = EDebugTypeBase.Menu,
        State             = EDebugTypeBase.State,
        Network           = EDebugTypeBase.Network,
        Event             = EDebugTypeBase.Event,
        ICCAnimation      = EDebugTypeBase.ICCAnimation,
        DisableScripts    = EDebugTypeBase.DisableScripts,
        */
    }


    public enum EDebugSpecialList
    {
        All   = EDebugTypeBase.All,
        Audio = EDebugTypeBase.Audio,
    }
}
