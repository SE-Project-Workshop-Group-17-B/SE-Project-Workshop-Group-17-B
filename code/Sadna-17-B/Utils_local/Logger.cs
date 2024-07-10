using System;
using System.IO;




// -------------- Logger base class -----------------------------------------------------------------------------------------------------------


public abstract class Logger
{
    private readonly string logFilePath;
    private readonly object lockObject = new object();

    public Logger(string file_name)
    {
        string logFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Log");
        this.logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Log", file_name);


        try
        {
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Dispose();
            }
        } catch (Exception ignore) { }
    }

    public virtual void Log(string message)
    {
        string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\t\t {message}";

        try
        {
            File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
        } catch (Exception ignore) { }
    }

    public abstract void Log(string type, string message);
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
        Log("\n\n ----- Error Logger ----- \n\n");

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
        Log("ERROR", message);
    }

    public override void Log(string type, string message)
    {
        string informative_message = $"| {type,15} | " + message;

        base.Log(informative_message);
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


    public virtual void Log(string message)
    {
        Log("INFO", message);
    }

    public override void Log(string type, string message)
    {
        string informativeMessage = $"| {type,15} | {message}";
        base.Log(informativeMessage);
    }

    public void LogSpecialInfo()
    {
        // currently not implemented
    }



}