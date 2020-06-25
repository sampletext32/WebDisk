using System;
using System.Collections.Generic;

namespace Entities
{
    public class LogBuilder
    {
        private List<String> m_logLines;

        private static LogBuilder s_instance;

        public event Action<string> OnAppend;
        public event Action OnClear;

        public static LogBuilder Get()
        {
            if (s_instance == null)
            {
                s_instance = new LogBuilder();
            }

            return s_instance;
        }

        private LogBuilder()
        {
            m_logLines = new List<string>();
        }

        public void AppendInfo(object info)
        {
            string logLine = "Info: " + info;
            m_logLines.Add(logLine);
            OnAppend?.Invoke(logLine);
        }

        public void AppendError(object error)
        {
            string logLine = "Error: " + error;
            m_logLines.Add(logLine);
            OnAppend?.Invoke(logLine);
        }


        public List<string> GetLog()
        {
            return m_logLines;
        }

        public void Clear()
        {
            m_logLines.Clear();
            OnClear?.Invoke();
        }
    }
}