using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

/* If you want to run in isolation,manually run from
 cd D:\GitRepos\MyProgram\src\MyProgram\bin\Debug\net8.0\
MyProgram.exe
and comment  AssemblyInitialize
 */

[TestClass]
public static class ServiceTestSetup
{
    public static Process ServiceProcess { get;  set; }

    // ignore warning  UTA031: class ServiceTestSetup does not have valid TestContext property. TestContext must be of type TestContext, must be non-static, public and must not be read-only. For example: public TestContext TestContext.
    public static TestContext TestContext { get; set; } //https://chatgpt.com/c/eabcf717-8cf0-41e8-aba7-c2e9b043ad84

    [AssemblyInitialize]
    public static void Initialize(TestContext context)
    {
        // Path to your .NET MVC service executable
        string servicePath = "MyProgram.exe";
#if !DEBUG
        servicePath = "MyProgram"; //in docker app, there is no  "MyProgram.exe", just the name
#endif
        Console.WriteLine($"ServiceTestSetup.Initialize starts for {servicePath}");
        // Ensure the executable exists
        if (!File.Exists(servicePath))
        {
            var currentDirectory=System.IO.Directory.GetCurrentDirectory();
            var message= $"The specified file was not found {servicePath}" + Environment.NewLine; ;
            message+= $"In the following directory:{currentDirectory}" + Environment.NewLine;
            foreach (string file in Directory.EnumerateFiles(currentDirectory, "*.*", SearchOption.TopDirectoryOnly))
            {
                message+=$"{file}"+Environment.NewLine;
            }
            throw new FileNotFoundException(message, servicePath);
        }
        //If required to kill process see https://www.revisitclass.com/networking/how-to-kill-a-process-which-is-using-port-8080-in-windows/
        // netstat -ano | findstr 5000
        // In Admin    taskkill /F /PID 12345  
       
        var useShellExecute = false;  //use true, if want to debug output locally 
        StartService(servicePath, "", useShellExecute);
    }
    private static void StartService(string servicePath, string dllPath, bool useShellExecute=true)
    {
        bool redirectOutput = !useShellExecute;
        ServiceProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = servicePath,
                Arguments = dllPath, // Add any necessary arguments here
                UseShellExecute = useShellExecute,
                RedirectStandardOutput = redirectOutput,
                RedirectStandardError = redirectOutput,
                CreateNoWindow = false
            }
        };
         
        Console.WriteLine("serviceProcess.Start");
        ServiceProcess.Start();

        Thread.Sleep(1000); // Adjust the delay as necessary based on your service's startup time
        Console.WriteLine("CheckIfRunningLogOutput");
        CheckIfRunningLogOutput(ServiceProcess, redirectOutput);
    }

    [AssemblyCleanup]
    public static void Cleanup()
    {
        if (ServiceProcess != null && !ServiceProcess.HasExited)
        {
            TestContext?.WriteLine("Killing the service process.");
            ServiceProcess.Kill();
            ServiceProcess.Dispose();
        }
    }
    private static void CheckIfRunningLogOutput(Process process, bool redirectOutput=true)
    {
        try
        {
            if (process.HasExited)
            {
                // Read the output stream
                string output = redirectOutput ? process.StandardOutput.ReadToEnd() : "not redirected";
                string error = redirectOutput ? process.StandardError.ReadToEnd() : "not redirected";
                // Check the exit code
                int exitCode = process.ExitCode;

                // Handle the output and error
                if (exitCode == 0)
                {
                    Log("Process completed successfully.");
                    Log("Output: " + output);
                }
                else
                {
                    Log("Process completed with errors.");
                    Log("Error: " + error);
                    Log("Output: " + output);
                }
            }
            else
            {
                Log("Process started and is running.");
            }
        }
        catch (Exception ex)
        {
            Log("Exception occurred: " + ex.ToString());
        }
    }
    public static void Log(string message)
    {
        // TestContext is always null from AssemblyInitialize invoke
        Console.WriteLine(message + $". IsTestContextNull:{TestContext == null}");
        if (TestContext != null)
        {
            TestContext.WriteLine(message);
        }
    }
}
