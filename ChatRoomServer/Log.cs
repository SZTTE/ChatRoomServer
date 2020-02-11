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

        private readonly object lockObject = new object();
        private string Path { get; set; }
        public Log(string fileName = "log.txt")
        {
            Path = fileName;
        }

        public void Write(string message, LogType type = LogType.Normal, char endChar = '\n')
        {
            lock (lockObject)
            {
                using StreamWriter writer = new StreamWriter(Path, true);
                writer.Write(type + ":" + message + DateTime.Now.GetDateTimeString() + endChar);
            }
        }
    }
}
