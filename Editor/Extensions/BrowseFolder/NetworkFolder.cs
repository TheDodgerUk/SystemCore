
namespace EditorTools
{
    public class NetworkFolder : BrowseFolder
    {
        private static readonly string Key = (nameof(NetworkFolder) + "_main").ShaHash();

        public NetworkFolder() : base("Network", Key, "//UKSDATA/SHARED/InnoSoftAR/Imhotep", new[] { "data", "bundles"}) { }
    }
}
