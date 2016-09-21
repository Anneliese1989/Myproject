
using System.Runtime.InteropServices;


namespace Jingge.Common
{
    public static class NetWorkCheck
    {
        private const int INTERNET_CONNECTION_MODEM = 1;
        private const int INTERNET_CONNECTION_LAN = 2;
        [DllImport ( "winInet.dll" )]
        private static extern bool InternetGetConnectedState ( ref int dwFlag, int dwReserved );
        public static int NetConnection ( )
        {

            //调用的方法(Winform为例,放一个按钮,单击即可):   

            System.Int32 dwFlag = new int ( );
            if (!InternetGetConnectedState ( ref dwFlag, 0 ))
                return 0;
            else
            if (( dwFlag & INTERNET_CONNECTION_MODEM ) != 0)
                return 1;
            else
            if (( dwFlag & INTERNET_CONNECTION_LAN ) != 0)
                return 2;
            else
                return -1;
        }

    }
}
