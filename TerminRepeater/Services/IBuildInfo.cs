using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminRepeater.Services
{
    public interface IBuildInfo
    {
        IFileSystem FileSystem { get; }
        string LocalDataStorageDirectory { get; }
    }
}
