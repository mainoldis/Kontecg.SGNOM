using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using Kontecg.Dependency;

namespace Kontecg.Runtime
{
    /// <summary>
    ///   Provides access to the command line args passed to the application at startup.
    /// </summary>
    /// <remarks>
    ///   Supports the following switches: "/", "-", "--"
    ///   Supports the following switch/value delimiters: "=", ":"
    ///   Example: /noui /username=jdoe /password=secret /operation=export /outputfile=exportoutput.xml
    /// </remarks>
    public class CommandLineParserService : ITransientDependency
    {
        private readonly NameValueCollection _args;

        /// <summary>
        ///   Initializes a new instance of the CommandLineArgs class
        /// </summary>
        public CommandLineParserService()
        {
            _args = new NameValueCollection();
            Parse(Environment.GetCommandLineArgs());
        }

        /// <summary>
        ///   Initializes a new instance of the CommandLineArgs class
        /// </summary>
        /// <param name="args"> An array of string arguments to parse </param>
        public CommandLineParserService(string[] args)
        {
            _args = new NameValueCollection();
            Parse(args);
        }

        /// <summary>
        ///   Returns the number of args that were found and parsed from the command line
        /// </summary>
        public int ArgsCount
        {
            get { return _args.Count; }
        }

        /// <summary>
        ///   Returns the value for the command line arg at the specified index
        /// </summary>
        public string this[int index]
        {
            get { return _args[index]; }
        }

        /// <summary>
        ///   Returns the value for the command line arg with the specified name
        /// </summary>
        public string this[string name]
        {
            get { return _args[name]; }
        }

        /// <summary>
        ///   Parses the argument list into a name/value collection
        /// </summary>
        /// <param name="args"> The command line args to parse </param>
        private void Parse(string[] args)
        {
            // <devnote>
            // The following regular expression is the pattern in use. It is here
            // in it's entirety so that it may be copied to a regular expression
            // testing tool such as Expresso for testing.
            // Expression: (^/|^-{1,2})(?<arg>(\w+))(?(=|:)(?<value>(.+)))
            // SampleData: Example: /noui /username=jdoe /password=secret /operation=export /outputfile=exportoutput.xml
            // </devnote>
            const string RegexArgGroupName = "arg"; // this value is in the pattern below as the name of a capture group
            const string RegexValueGroupName = "value";
            // this value is in the pattern below as the name of a capture group
            const string RegexPattern =
                @"(^/|^-{1,2})(?<" + RegexArgGroupName + @">(\w+))(?(=|:)(?<" + RegexValueGroupName + @">(.+)))";

            var regex = new Regex(RegexPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            foreach (var arg in args)
            {
                // try and match each arg
                var match = regex.Match(arg);
                while (match.Success)
                {
                    // snag the arg group
                    var argGroup = match.Groups[RegexArgGroupName];
                    if (argGroup != null)
                    {
                        // snag the value group
                        var valueGroup = match.Groups[RegexValueGroupName];
                        _args.Add(argGroup.Value, valueGroup != null ? valueGroup.Value : string.Empty);
                    }

                    // go to the next match
                    match = match.NextMatch();
                }
            }
        }

        /// <summary>
        ///   Determines if the specified arg exists by name
        /// </summary>
        /// <param name="name"> The name of the arg to search for </param>
        /// <returns> </returns>
        public bool Exists(string name)
        {
            return _args.AllKeys.Any(argName => argName == name);
        }

        /// <summary>
        ///   Returns the value for the command line arg with the specified name as a string
        /// </summary>
        /// <param name="name"> </param>
        /// <param name="defaultValue"> </param>
        /// <returns> </returns>
        public string GetArgValue(string name, string defaultValue)
        {
            if (Exists(name))
            {
                string value = this[name];

                // this is such a complete tared up hack
                // i know you can replace with regex, just didn't take the time to look it up
                // we'll come back to this a little later and fix this gheyness
                value = value.Trim('"', '=', ':');
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        ///   Returns the value for the command line arg with the specified name as a System.Boolean
        /// </summary>
        /// <param name="name"> </param>
        /// <param name="defaultValue"> </param>
        /// <returns> </returns>
        public bool GetArgValueAsBoolean(string name, bool defaultValue)
        {
            try
            {
                return Exists(name) ? Convert.ToBoolean(this[name]) : defaultValue;
            }
            catch (FormatException e)
            {
                return true;
            }
        }

        /// <summary>
        ///   Returns the value for the command line arg with the specified name as an System.Int32
        /// </summary>
        /// <param name="name"> </param>
        /// <param name="defaultValue"> </param>
        /// <returns> </returns>
        public int GetArgValueAsInt32(string name, int defaultValue)
        {
            try
            {
                return Exists(name) ? Convert.ToInt32(this[name]) : defaultValue;
            }
            catch (FormatException e)
            {
                return defaultValue;
            }
            catch (OverflowException e)
            {
                return int.MaxValue;
            }
        }
    }
}
