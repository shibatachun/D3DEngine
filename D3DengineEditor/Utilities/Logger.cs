﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;


namespace D3DengineEditor.Utilities
{
    enum MessageType
    {
        Info = 0x01,
        Warning = 0x02,
        Error = 0x04,

    }

    class LogMessage
    {
        public DateTime Time { get; }
        public MessageType MessageType { get; }
        public string Message { get; }
        public string File { get; }
        public string Caller { get; }
        public int Line { get; }
        public string MetaData => $"{File} : {Caller} ({Line})";

        public LogMessage(MessageType type, string msg, string file, string caller, int line)
        {
            Time = DateTime.Now;
            MessageType = type;
            Message = msg;
            File = Path.GetFileName(file);
            Caller = caller;
            Line = line;
        }
    }
    static class Logger
    {
        //把三种错误信息组合起来: 0x01 | 0x02 | 0x04 也就是 001 | 010 | 100 或运算表示有1结果就是1,所以结果为111,表示三种都能接受
        private static int _messageFilter = (int)(MessageType.Info | MessageType.Warning | MessageType.Error);

        //use of static
        private readonly static ObservableCollection<LogMessage> _messages = new ObservableCollection<LogMessage>();

        public static ReadOnlyObservableCollection<LogMessage> Messages
        {
            get;
        } = new ReadOnlyObservableCollection<LogMessage>(_messages);

        public static CollectionViewSource FilteredMessages
        { get; } = new CollectionViewSource() { Source = Messages };
        //call from different threads, not have to be from UI thread, need to add to the message list ,than use async (need to ask GPT later)
        public static async void Log(MessageType type, string msg,
            [CallerFilePath] string file = "", [CallerMemberName] string caller = "",
            [CallerLineNumber] int line = 0)
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _messages.Add(new LogMessage(type, msg, file, caller, line));

            }));
        }
        //Clear the message
        public static async void Clear()
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _messages.Clear();

            }));
        }

        public static void SetMessageFilter(int mask)
        {
            _messageFilter = mask;
            FilteredMessages.View.Refresh();
        }
        static Logger()
        {
            //按位运算来检测message是否符合fitler的条件
            FilteredMessages.Filter += (s, e) =>
            {
                var type = (int)(e.Item as LogMessage).MessageType;
                e.Accepted = (type & _messageFilter) != 0;
            };
        }


    }
}
