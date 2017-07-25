using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security;

namespace Wtf.Queue
{
    public class CharacterQueue : IEnumerable<TempPassword>
    {
        public static CharacterQueue Instance { get; } = new CharacterQueue();

        private ConcurrentQueue<TempPassword> _queue1 = new ConcurrentQueue<TempPassword>();

        private CharacterQueue()
        {
            
        }

        public void Enqueue(TempPassword item)
        {
            _queue1.Enqueue(item);
        }

        public TempPassword? Dequeue()
        {
            if (_queue1.TryDequeue(out TempPassword item))
            {
                return item;
            }

            return null;
        }

        public SecureString DequeueSecureString(string userKey)
        {
            // This is an O(n) operation
            var secureString = new SecureString();
            int counter = _queue1.Count;
            while (_queue1.TryDequeue(out TempPassword item) && counter-- > 0)
            {
                if (item.UserKey == userKey)
                {
                    secureString.AppendChar(item.Character);
                }
                else
                {
                    // Send it back into the queue.
                    _queue1.Enqueue(item);
                }
            }

            return secureString.Length > 0 ? secureString : null;
        }

        public IEnumerator<TempPassword> GetEnumerator()
        {
            foreach (var item in _queue1)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct TempPassword
    {
        public string UserKey { get; set; }
        public char Character { get; set; }}
}
