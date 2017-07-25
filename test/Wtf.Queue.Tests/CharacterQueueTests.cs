using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using Xunit;
using Xunit.Abstractions;

namespace Wtf.Queue.Tests
{
    public class CharacterQueueTests
    {
        private ITestOutputHelper _output;

        public CharacterQueueTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory(Skip = "Tests don't run in a particular order, so we can't expect a singleton to be empty.")]
        public void ConstructorTest()
        {
            // Arrange/Act
            var target = CharacterQueue.Instance;

            // Assert
            Assert.NotNull(target);
            Assert.Empty(target);
        }

        [Fact]
        public void EnqueueTest()
        {
            // Arrange
            var target = CharacterQueue.Instance;

            // Act
            target.Enqueue(new TempPassword());

            // Assert
            Assert.NotNull(target);
            Assert.NotEmpty(target);
        }

        [Fact]
        public void DequeueTest()
        {
            // Arrange
            var target = CharacterQueue.Instance;
            const string fooKey = "Foo";
            target.Enqueue(new TempPassword { Character = 'b', UserKey = fooKey });

            // Act
            var item = target.Dequeue();

            // Assert
            Assert.NotNull(item);
            Assert.Equal('b', item.Value.Character);
        }

        [Fact]
        public void DequeueSecureStringTest()
        {
            // Arrange
            var target = CharacterQueue.Instance;
            const string fooKey = "Foo";
            var password = new[] { 'p', 'a', 's', 's', 'w', 'o', 'r', 'd' };
            foreach (var c in password)
            {
                target.Enqueue(new TempPassword { Character = c, UserKey = fooKey });
            }

            var length = target.Count();

            // Act
            var actual = target.DequeueSecureString(fooKey);
            
            Assert.Equal(length - 8, target.Count());

            var bval = DeAlloc(actual);

            for (int i = 0; i < password.Length; i++)
            {
                var cha = Convert.ToChar(bval[i]);
                _output.WriteLine($"char[{i}] : {cha}");
                Assert.Equal(password[i], cha);
            }
        }

        /// <summary>
        /// See https://stackoverflow.com/questions/818704/how-to-convert-securestring-to-system-string
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        private static byte[] DeAlloc(SecureString ss)
        {
            byte[] bValue = new byte[ss.Length];
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = SecureStringMarshal.SecureStringToGlobalAllocUnicode(ss);
                for (int i = 0; i < ss.Length; i++)
                {
                    short unicodeChar = Marshal.ReadByte(valuePtr, i * 2);
                    // handle unicodeChar
                    // is this a char or a byte???
                    bValue[i] = (byte) unicodeChar;
                }
            }
            finally
            {
                // This will completely remove the data from memory
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }

            return bValue;
        }

    }
}
