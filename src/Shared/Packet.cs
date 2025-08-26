#pragma warning disable

using System.Drawing;

namespace Monitoring.Shared
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

        public byte[] header
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