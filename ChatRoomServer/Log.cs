using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatRoomServer
{
    class Log
    { 
        public enum LogType
        {
            Normal,
            Error
        }

        private string Path { get; set; }
        public Log(string fileName = "log.txt")
        {
            Path = fileName;
        }

        public void Write(string message, LogType type = LogType.Normal, char endChar = '\n')
        {
            using StreamWriter writer = new StreamWriter(Path, true);
            writer.Write(type + ":" + message + DateTime.Now.GetDateTimeString() + endChar);
        }
    }
}
