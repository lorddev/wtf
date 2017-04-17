using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace wtf.asp
{
    /// <summary>
    /// Writes an in-memory object to a file.
    /// </summary>
    public class PersistenceProofOfConcept<T> where T : Dson, new()
    {
        private static readonly string _fileName = AppContext.BaseDirectory + "\\persist.json";

        private static readonly object _lock = new object();

        private readonly T _dataStore;

        public PersistenceProofOfConcept()
        {
            _dataStore = new T { Id = Guid.NewGuid() };
        }

        public PersistenceProofOfConcept(T seed)
        {
            _dataStore = seed;
        }

        public void CreateFile()
        {
            var fi = new FileInfo(_fileName);
            if (fi.Exists)
            {
                return;
            }

            using (var file = new FileStream(_fileName, FileMode.Create))
            {
                WriteCompletely(file);
            }
        }

        private void WriteCompletely(FileStream file)
        {
            //var orig = file.Length;
            var info = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_dataStore,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            file.SetLength(info.Length);
            file.Write(info, 0, info.Length);
        }

        public void Save()
        {
            lock (_lock)
            {
                using (var file = new FileStream(_fileName, FileMode.Open, FileAccess.Write))
                {
                    file.Position = 0;
                    WriteCompletely(file);
                }
            }
        }


        public async Task<T> Load()
        {
            byte[] info;
            using (var file = new FileStream(_fileName, FileMode.Open))
            {
                info =
                    new byte[file
                        .Length]; // file.ReadAsync() Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_dataStore));
                await file.ReadAsync(info, 0, info.Length);
            }

            var result = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(info));
            if (result == null)
            {
                Save();
                return _dataStore;
            }

            return result;
        }

        public async Task<T> Load2()
        {
            byte[] info;
            var sb = new StringWriter();

            using (var file = new FileStream(_fileName, FileMode.Open))
            {
                info =
                    new byte[1024]; // file.ReadAsync() Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_dataStore));
                var offset = 0;
                while (offset < file.Length)
                {
                    var remainder = file.Length - offset;
                    //  sb.WriteAsync()
                    var count = (int) Math.Min(remainder, 1024);
                    await file.ReadAsync(info, 0, count);
                    await sb.WriteAsync(Encoding.UTF8.GetChars(info, 0, count));
                    offset += count;
                }
            }

            var result = JsonConvert.DeserializeObject<T>(sb.ToString());
            if (result == null)
            {
                Save();
                return _dataStore;
            }

            return result;
        }
    }

    public abstract class Dson
    {
        public Guid Id { get; set; }
    }

    public class InMemoryDataStore : Dson
    {
        public InMemoryDataStore()
        {
            for (var i = 0; i < 1024; i++)
            {
                Collection.Add(i);
            }
        }

        public string Foo { get; set; }

        public List<int> Collection { get; } = new List<int>(1024);
    }
}