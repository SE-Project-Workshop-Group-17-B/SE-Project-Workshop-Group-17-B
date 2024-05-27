using System;
using System.IO;




// -------------- Logger base class -----------------------------------------------------------------------------------------------------------


public class Logger
{
    private readonly string logFilePath;
    private readonly object lockObject = new object();

    public Logger(string file_name)
    {
        this.logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Log", file_name);

        if (!File.Exists(logFilePath))
        {
            File.Create(logFilePath).Dispose();
        }
    }

    public void Log(string message)
    {
        string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\t\t {message}";
   
        File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
    }
}



// -------------- Error Logger -----------------------------------------------------------------------------------------------------------


public class ErrorLogger : Logger
{


    // -------------- singleton related --------------------------------------

    
    private static ErrorLogger instance;
    private static readonly object error_lock = new object();


    public ErrorLogger() : base("error_logger.txt")
    {
        Log("");
        Log("\t\t\t\t\t ----- Error Logger ----- \n\n\n");

    }

    public static ErrorLogger Instance
    {
        get
        {
            lock (error_lock)
            {
                if (instance == null)
                {
                    instance = new ErrorLogger();
                }
                return instance;
            }
        }
    }


    // -------------- Log into text file ---------------------------------------


    public void Log(string message)
    {
        Log("error", message);
    }

    public void Log(string type, string message)
    {
        string informative_message = $"| {type,15} | " + message;

        base.Log(message);
    }

    public void LogSpecialError()
    {
        // currently not implemented
    }



}


// -------------- Information Logger -----------------------------------------------------------------------------------------------------------


public class InfoLogger : Logger
{

    // -------------- singleton related --------------------------------------


    private static InfoLogger instance;
    private static readonly object info_lock = new object();


    public InfoLogger() : base("info_logger.txt")
    {
        Log("");
        Log("\n\n ----- Info Logger ----- \n\n");
    }

    public static InfoLogger Instance
    {
        get
        {
            lock (info_lock)
            {
                if (instance == null)
                {
                    instance = new InfoLogger();
                }

                return instance;
            }
        }
        
    }


    // -------------- Log into text file ---------------------------------------


    public void Log(string message)
    {
        Log("info", message);
    }

    private void Log(string type, string message)
    {
        string informativeMessage = $"| {type,15} | {message}";
        base.Log(informativeMessage);
    }

    public void LogSpecialInfo()
    {
        // currently not implemented
    }



}
