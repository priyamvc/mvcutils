using System;
using System.Collections;
using System.IO;
using System.Text;

namespace seanfoy.mvcutils {
    /// <summary>
    /// Adapt arbitrary <c>IEnumerable</c>s
    /// to the Stream interface.
    /// </summary>
    public class IEnumerableStream : Stream {
        public IEnumerator src { get; set; }
        public Encoding enc { get; set; }
        override public Boolean CanRead {
            get {
                return true;
            }
        }
        private long positionField = -1;
        override public long Position {
            get { return positionField; }
            set {
                throw new NotImplementedException();
            }
        }
        public IEnumerableStream(IEnumerable source, Encoding enc) {
            this.src = source.GetEnumerator();
            this.enc = enc;
            internalBuffer = new MemoryStream();
        }
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (!disposing) return;

            if (src != null) {
                IDisposable disposable = src as IDisposable;
                if (disposable != null) {
                    disposable.Dispose();
                }
                src = null;
            }
            if (internalBuffer != null) {
                internalBuffer.Dispose();
                internalBuffer = null;
            }
            GC.SuppressFinalize(this);
        }
        private MemoryStream internalBuffer;
        private Byte [] bytes;
        public override int Read(Byte [] buffer, int offset, int count) {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (offset < 0) {
                throw new ArgumentOutOfRangeException("offset", "must be non-negative");
            }
            if (count < 0) {
                throw new ArgumentOutOfRangeException("count", "must be non-negative");
            }
            int sumBytesRead = 0;
            while (sumBytesRead < count) {
                long ibavail = internalBuffer.Length - internalBuffer.Position;
                if (ibavail > 0) {
                    int flow = (int)Math.Min(ibavail, count - sumBytesRead);
                    flow = internalBuffer.Read(buffer, offset + sumBytesRead, flow);
                    sumBytesRead += flow;
                    if (flow == ibavail) {
                        internalBuffer.SetLength(0);
                    }
                    continue;
                }
                if (!src.MoveNext()) break;
                var c = src.Current;
                var s = c.ToString();
                var byteCount = enc.GetByteCount(s);
                if (bytes == null || bytes.Length < byteCount) {
                    bytes = new Byte[byteCount];
                }
                byteCount = enc.GetBytes(s, 0, s.Length, bytes, 0);
                internalBuffer.Write(
                    bytes,
                    0,
                    byteCount);
                internalBuffer.Seek(0, SeekOrigin.Begin);
            }
            positionField += sumBytesRead;
            return sumBytesRead;
        }

        public override void Write(byte [] b, int i, int j) {
            throw new NotImplementedException();
        }
        public override void SetLength(long l) {
            throw new NotImplementedException();
        }
        public override long Seek(long l, SeekOrigin o) {
            throw new NotImplementedException();
        }
        public override void Flush() {
            throw new NotImplementedException();
        }
        public override long Length {
            get {
                throw new NotImplementedException();
            }
        }
        public override Boolean CanWrite {
            get {
                return false;
            }
        }
        public override Boolean CanSeek {
            get {
                return false;
            }
        }
    }
}
