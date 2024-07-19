using Serilog.Sinks.PostgreSQL;
using Serilog;
using NpgsqlTypes;
using System.Diagnostics;
using Tbot;
using System.Text.RegularExpressions;



namespace MySerilogTestWithBot
{
    public class MySerilogWithBotClass
    {
        private readonly Serilog.ILogger _logger;
        private static string _msgFormat;
        private TelegramBotService _botService;
        public MySerilogWithBotClass(string connectionString, string chatId, string token, string msgFormat)
        {
            var columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "TimeStamp", new TimestampColumnWriter() },
            { "Level", new LevelColumnWriter() },
            { "Message", new RenderedMessageColumnWriter() },
            { "Exception", new ExceptionColumnWriter() },
            { "Properties", new PropertiesColumnWriter() },
            { "LogStatus", new SinglePropertyColumnWriter("LogStatus", (PropertyWriteMethod)NpgsqlDbType.Varchar) },
       //     { "ExecutionTime", new SinglePropertyColumnWriter("ExecutionTime", (PropertyWriteMethod)NpgsqlDbType.Interval) },
            { "Method", new SinglePropertyColumnWriter("Method", (PropertyWriteMethod)NpgsqlDbType.Varchar) },
            { "FilePlace", new SinglePropertyColumnWriter("FilePlace", (PropertyWriteMethod)NpgsqlDbType.Varchar) },
        };

           _msgFormat = msgFormat;
            _botService = new TelegramBotService(chatId, token);
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.PostgreSQL(
                    connectionString: connectionString,
                    tableName: "Logs",
                    needAutoCreateTable: true,
                    columnOptions: columnWriters)
                .CreateLogger();
        }

        //public void Information(string message)
        //{
        //    var stopwatch = Stopwatch.StartNew();
        //    try
        //    {
        //        _logger.Information(message);
        //    }
        //    finally
        //    {
        //        stopwatch.Stop();
        //        var executionTime = stopwatch.Elapsed;
        //        _logger.ForContext("ExecutionTime", executionTime)
        //               .Information("Execution time for message: {Message}", message);
        //    }
        //}

        private string MessageFormat(string message, string messageText, string exception, string logStatus, string method, string filePlace, string msgFormat)
        {
            string pattern = @"\{(.*?)\}";

            MatchCollection matches = Regex.Matches(message, pattern);

            foreach(Match match in matches)
            {
                var test = match.Groups[1].Value.ToLower();
                switch (match.Groups[1].Value.ToLower())
                {
                    case "timestamp":
                       message = message.Replace("{TimeStamp}", DateTime.Now.ToString() + "\r\n");
                        break;
                    case "message":
                        message = message.Replace("{Message}", messageText + "\r\n");
                        break;
                    case "exception":
                        message = message.Replace("{Exception}", exception + "\r\n");
                        break;
                    case "logstatus":
                        message = message.Replace("{LogStatus}", logStatus + "\r\n");
                        break;
                    case "method":
                        message = message.Replace("{Method}", method + "\r\n");
                        break;
                    case "fileplace":
                        message = message.Replace("{FilePlace}", filePlace + "\r\n");
                        break;
                    case "messagetype":
                        message = message.Replace("{MessageType}", msgFormat + "\r\n");
                        break;

                }
            }

            return message;
        }

        public void Information(string message)
        {
            _logger.ForContext("LogStatus", "Success")
                .Information(message);
            var logText = MessageFormat(_msgFormat, message, "null", "Success", "null", "null", "Information");
            _botService.SendMessageAsync(logText);
         
        }

        public void InformationLogStatus(string message, string logStatus)
        {
            _logger.ForContext("LogStatus", logStatus)
                .Information(message);

            var logText = MessageFormat(_msgFormat, message, "null", logStatus, "null", "null", "Information");
            _botService.SendMessageAsync(logText);
        }

        public void Information(string message, string methodName)
        {
            _logger.ForContext("LogStatus", "Success")
                .ForContext("Method", methodName)
                .Information(message);

            var logText = MessageFormat(_msgFormat, message, "null", "Success", methodName, "null", "Information");
            _botService.SendMessageAsync(logText);
            
        }

      
        public void Information(string message, string methodName, string filePlace)
        {
            _logger.ForContext("LogStatus", "Success")
                .ForContext("Method", methodName)
               .ForContext("FilePlace", filePlace)
               .Information(message);

            var logText = MessageFormat(_msgFormat, message, "null", "Success", methodName, filePlace, "Information");
            _botService.SendMessageAsync(logText);
        }

        public void Error(string message)
        {
            _logger.ForContext("LogStatus", "Error")
                .Error(message);
            var logText = MessageFormat(_msgFormat, message, "null", "Error", "null", "null", "Error");
            _botService.SendMessageAsync(logText);
        }

        public void Error(string message, string methodName)
        {
            _logger.ForContext("Method", methodName)
                .ForContext("LogStatus", "Error")
                .Error(message);
            var logText = MessageFormat(_msgFormat, message, "null", "Error", methodName, "null", "Error");
            _botService.SendMessageAsync(logText);
        }

        public void ErrorLogStatus(string message, string logStatus)
        {
            _logger.ForContext("LogStatus",logStatus)
                .Error(message);
            var logText = MessageFormat(_msgFormat, message, "null", logStatus, "null", "null", "Error");
            _botService.SendMessageAsync(logText);
        }

        public void Error(string message, string methodName, string filePlace)
        {
            _logger.ForContext("Method", methodName)
                .ForContext("FilePlace", filePlace)
                .ForContext("LogStatus", "Error")
                .Error(message);
            var logText = MessageFormat(_msgFormat, message, "null", "Error", methodName, filePlace, "Error");
            _botService.SendMessageAsync(logText);
        }

        public void Error(Exception ex, string message)
        {
            _logger.ForContext("LogStatus", "Error")
                .Error(ex, message);
            var logText = MessageFormat(_msgFormat, message, ex.Message, "Error", "null", "null", "Error");
            _botService.SendMessageAsync(logText);
        }


        public void Error(Exception ex, string message, string methodName)
        {
            _logger.ForContext("LogStatus", "Error")
                .ForContext("Method", methodName)
                .Error (ex, message);
            var logText = MessageFormat(_msgFormat, message, ex.Message, "Error", methodName, "null", "Error");
            _botService.SendMessageAsync(logText);
        }

        public void Error(Exception ex, string message, string methodName, string filePlace)
        {
            _logger.ForContext("LogStatus", "Error")
              .ForContext("Method", methodName)
              .ForContext("FilePlace", filePlace)
              .Error(ex, message);
            var logText = MessageFormat(_msgFormat, message, ex.Message, "Error", methodName, filePlace, "Error");
            _botService.SendMessageAsync(logText);
        }

    }
}
