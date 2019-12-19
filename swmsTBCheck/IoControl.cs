using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swmsTBCheck
{
    public class IoControl
    {
        public class DI
        {

        }

        public class AI
        {

        }

        public class DO_Multi
        {
            public byte moduleAddress;
            public byte functionCode;
            public ushort startRegisterAddress;
            public ushort outputChannels;
            public byte dataBytes;
            public byte writeData;
            public ushort crcCheck;
        }

        public class Do_Single
        {
            public byte moduleAddress;
            public byte functionCode;
            public ushort startRegisterAddress;
            public ushort writeData;
            public ushort crcCheck;
        }
    }
}
