using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace seanfoy.mvcutils {
    [TestFixture]
    public class IEStream {
        [Test]
        public void Empty() {
            using (var ies = new IEnumerableStream(emptySrc(), System.Text.Encoding.UTF8)) {
                using (var ms = new MemoryStream(0)) {
                    byte [] iesb = new byte[1];
                    byte [] msb = new byte[1];
                    Assert.AreEqual(
                        ms.Read(msb, 0, 0),
                        ies.Read(iesb, 0, 0));
                    Assert.AreEqual(msb, iesb);
                    Assert.AreEqual(
                        ms.Read(msb, 0, 1),
                        ies.Read(iesb, 0, 1));
                }
            }
        }

        public IEnumerable emptySrc() {
            yield break;
        }

        public IEnumerable singleton() {
            yield return 0;
        }

        [Test]
        public void pastTheEnd() {
            using (var ies = new IEnumerableStream(singleton(), System.Text.Encoding.UTF8)) {
                using (var ms = new MemoryStream(new Byte [] {(Byte)'0'})) {
                    Byte [] iesb = new Byte[4];
                    Byte [] msb = new Byte[4];
                    Assert.AreEqual(
                        ms.Read(msb, 0, 4),
                        ies.Read(iesb, 0, 4));
                    Assert.AreEqual(msb, iesb);
                }
            }
        }

        public IEnumerable monotonic() {
            for (int i = 0; i < int.MaxValue; ++i) {
                yield return i;
            }
        }

        [Test]
        public void stateful() {
            var d = new HybridDictionary();
            using (var ies = new IEnumerableStream(monotonic(), System.Text.Encoding.UTF8)) {
                Byte [] buff = new Byte[1];
                int byteCount;
                for (int i = 0; i < 3; ++i) {
                    byteCount = ies.Read(buff, 0, buff.Length);
                    Assert.IsFalse(d.Contains(buff[0]), "A stream with no repeated bytes should not yield duplicate values");
                    d[buff[0]] = true;
                }
            }
        }

        [Test]
        public void TwoAtATime() {
            using (var ies = new IEnumerableStream(monotonic(), System.Text.Encoding.UTF8)) {
                Byte [] buff = new Byte[2];
                int byteCount;
                const int limit = 3;
                const int atATime = 2;
                for (int i = 0; i < limit; ++i) {
                    byteCount = ies.Read(buff, 0, atATime);
                    Assert.AreEqual(atATime, byteCount);
                }
                Byte expectedLastByte =
                    // assuming we start at zero
                    (Byte)((limit * atATime - 1).ToString()[0]);
                Assert.AreEqual(expectedLastByte, buff[1]);
            }
        }

        public IEnumerable mixedLengthChars() {
            yield return "a";
            yield return "Î»";
            yield return "q";
        }

        [Test]
        public void wchar() {
            Byte [] buff = new Byte[1];
            Assert.IsTrue(
                Enumerable.Any(
                    Enumerable.OfType<string>(mixedLengthChars()),
                    i => System.Text.Encoding.UTF8.GetByteCount((string)i) > buff.Length),
                "some element of the stream should be too big for the buffer");
            var expected = new StringBuilder();
            foreach (var i in mixedLengthChars()) {
                expected.Append(i);
            }
            var actual = new StringBuilder();
            using (var ies = new IEnumerableStream(mixedLengthChars(), System.Text.Encoding.UTF8)) {
                var d = System.Text.Encoding.UTF8.GetDecoder();
                var cbuff = new char[System.Text.Encoding.UTF8.GetMaxByteCount(1)];
                int byteCount, charCount;
                do {
                    byteCount = ies.Read(buff, 0, buff.Length);
                    charCount = d.GetChars(buff, 0, byteCount, cbuff, 0);
                    actual.Append(new String(cbuff, 0, charCount));
                } while (byteCount > 0);
            }
            Assert.AreEqual(expected.ToString(), actual.ToString());
        }
    }
}
