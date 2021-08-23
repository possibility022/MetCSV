using METCSV.WPF.ProductProvider;
using System.Threading;

namespace METCSV.UnitTests.ProductProvider
{
    class BaseProviderImplementation : ProductProviderBase
    {
        protected override string ArchiveFileNamePrefix => "TestFilePrefix";
        public string FilePrefix => ArchiveFileNamePrefix;
        public string Archive => ArchiveFolder;

        public BaseProviderImplementation(CancellationToken token) : base(token)
        {

        }
    }
}
