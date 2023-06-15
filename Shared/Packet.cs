#pragma warning disable

namespace Shared
{
    public class Packet
    {
        public int seqno
        {
            get;
            set;
        }

        public int lastno
        {
            get;
            set;
        }

        public int size
        {
            get;
            set;
        }

        public byte[] data
        {
            get;
            set;
        }
    }
}