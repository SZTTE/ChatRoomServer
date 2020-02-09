using System;
using System.Collections.Generic;
using System.Text;

namespace ChatRoomServer
{
    static class Tools
    {
        public static string GetDateTimeString(this DateTime dateTime)
        {
            return $"({dateTime})";
        }
    }
}
