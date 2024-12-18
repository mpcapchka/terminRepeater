using System.IO.Abstractions;

namespace TerminRepeater.Services
{
    public interface IBuildInfo
    {
        IFileSystem FileSystem { get; }
        Version AppVersion { get; }
        string LocalDataStorageDirectory { get; }
    }
}
