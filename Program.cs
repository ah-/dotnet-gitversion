using System;
using System.IO;
using GitVersion;
using GitVersion.Helpers;
using Newtonsoft.Json.Linq;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger.SetLoggers(
                Console.WriteLine,
                Console.WriteLine,
                Console.WriteLine);

            var fs = new FileSystem();
            var gv = new GitVersion.ExecuteCore(fs);
            var auth = new GitVersion.Authentication();

            var version = gv.ExecuteGitVersion(null, null, auth, null, false, ".", null);
            Console.WriteLine($"Setting version: {version.LegacySemVerPadded}");

            var filename = "project.json";
            var projectJson = JObject.Parse(File.ReadAllText(filename));
            projectJson["version"] = version.LegacySemVerPadded;
            File.WriteAllText(filename, projectJson.ToString());
        }
    }
}
