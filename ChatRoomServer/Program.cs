using System;

namespace ChatRoomServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Service service = new Service();
            try
            {  
                service.StartService();
            }
            catch
            {
                service.CloseService();
            }
        }
    }
}
