using GitVersion;
using GitVersion.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace dotnet.gitversion
{
    public class Program
    {
        private const string ProjectJsonFile = "project.json";
        private const string CsprojFile = ".csproj";
        private static readonly Regex s_regex = new Regex(@"(?<json>project\.json)|(?<csproj>\.csproj$)");

        public static int Main(string[] args)
        {
            Logger.SetLoggers(
                Console.WriteLine,
                Console.WriteLine,
                Console.WriteLine);

            var options = Options.Parse(args);
            if (options.Help)
            {
                Console.WriteLine(Options.GetHelpMessage());
                return 0;
            }

            if (options.NoFilesSpecified && !TryFindNetCoreProject(out options))
            {
                Console.Error.WriteLine($"Could not locate {ProjectJsonFile} or {CsprojFile} in current directory and no command line arguments were specified.");
                Console.WriteLine(Options.GetHelpMessage());
                return -1;
            }

            var fs = new FileSystem();
            var gv = new GitVersion.ExecuteCore(fs);
            var auth = new GitVersion.Authentication();

            var version = gv.ExecuteGitVersion(null, null, auth, null, false, ".", null);
            Console.WriteLine($"Setting version: {version.LegacySemVerPadded}");

            if (!string.IsNullOrEmpty(options.ProjectJson))
            {
                UpdateProjectJson(options.ProjectJson, version.LegacySemVerPadded);
            }

            if (!string.IsNullOrEmpty(options.CsProj))
            {
                UpdateCsproj(options.CsProj, version.LegacySemVerPadded);
            }

            return 0;
        }

        private static bool TryFindNetCoreProject(out Options options)
        {
            options = new Options();
            var found = false;

            foreach (var file in new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles())
            {
                var match = s_regex.Match(file.Name);

                if (match.Success)
                {
                    found = true;

                    if (match.Groups["json"].Success)
                    {
                        options.ProjectJson = file.FullName;
                    }
                    else if (match.Groups["csproj"].Success)
                    {
                        options.CsProj = file.FullName;
                    }
                }
            }

            return found;
        }

        private static void UpdateProjectJson(string projectJsonFile, string legacySemVerPadded)
        {
            var projectJson = JObject.Parse(File.ReadAllText(projectJsonFile));
            projectJson["version"] = legacySemVerPadded;
            File.WriteAllText(projectJsonFile, projectJson.ToString());
        }

        /// <summary>
        /// Looks for msbuild element PackageVersion to update with the name.
        /// https://docs.microsoft.com/en-us/nuget/schema/msbuild-targets
        /// </summary>
        /// <param name="csproj"></param>
        /// <param name="legacySemVerPadded"></param>
        private static void UpdateCsproj(string csproj, string legacySemVerPadded)
        {
            var document = XDocument.Load(csproj);
            var packageVersion = XName.Get("PackageVersion");
            var isSet = false;
            var descendents = document.Root.Descendants().ToArray();
            foreach (var node in document.Root.Descendants(packageVersion))
            {
                node.Value = legacySemVerPadded;
                isSet = true;
            }

            if (!isSet)
            {
                var propertyGroup = new XElement("PropertyGroup");
                var version = new XElement(packageVersion)
                {
                    Value = legacySemVerPadded
                };

                propertyGroup.Add(version);

                document.Root.AddFirst(propertyGroup);
            }

            document.Save(File.OpenWrite(csproj));
        }
    }
}
