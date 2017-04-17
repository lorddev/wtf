using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace wtf.asp.tests
{
    public class PersistenceProofOfConceptTest
    {
        public PersistenceProofOfConceptTest(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        [Fact]
        public void TestFileCreate()
        {
            var target = new PersistenceProofOfConcept<InMemoryDataStore>();
            target.CreateFile();
        }

        [Fact]
        public async Task TestFileDeserialize()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var target = await new PersistenceProofOfConcept<InMemoryDataStore>().Load();
            stopWatch.Stop();
            _output.WriteLine(stopWatch.ElapsedMilliseconds.ToString());
            Assert.NotNull(target);
            Assert.Null(target.Foo);
        }

        [Fact]
        public async Task TestFileDeserialize2()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var target = await new PersistenceProofOfConcept<InMemoryDataStore>().Load2();

            _output.WriteLine(stopWatch.ElapsedMilliseconds.ToString());
            Assert.NotNull(target);
            Assert.Null(target.Foo);
        }

        [Fact]
        public void TestFileSynchronize()
        {
            var target = new PersistenceProofOfConcept<InMemoryDataStore>();
            target.Save();
        }
    }
}