using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dotnet.gitversion
{
    internal class Options
    {
        private static readonly IDictionary<string, string> _commandMapping =
            new Dictionary<string, string> {
            { "-j", "projectjson" },
            { "-c", "csproj" },
            { "-h", "help" },
            { "-?", "help" },
        };

        public string ProjectJson { get; set; }
        public string CsProj { get; set; }
        public bool Help { get; set; }

        public static string GetHelpMessage()
        {
            var builder = new StringBuilder("USAGE: dotnet gitversion <options>");
            builder.AppendLine("    If no options are specified, then it will look for the project.json or *.csproj in current directory.");
            builder.AppendLine();
            builder.AppendLine("OPTIONS:");

            foreach (var kvp in _commandMapping)
            {
                if (string.Equals(kvp.Value, "help", StringComparison.OrdinalIgnoreCase))
                {
                    builder.AppendLine($"{kvp.Key}|--{kvp.Value}");
                }
                else
                {
                    builder.AppendLine($"{kvp.Key}|--{kvp.Value} PathToFile");
                }
            }

            return builder.ToString();
        }

        public bool NoFilesSpecified
        {
            get { return string.IsNullOrEmpty(CsProj) && string.IsNullOrEmpty(ProjectJson); }
        }

        public static Options Parse(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddCommandLine(args, _commandMapping);

            var configure = new ConfigureFromConfigurationOptions<Options>(builder.Build());
            var options = new Options();

            configure.Action(options);
            return options;
        }
    }
}
