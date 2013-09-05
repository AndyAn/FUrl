using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NetTools
{
    internal class CmdletHelper
    {
        internal static Dictionary<string, string> GetArguments(string[] args)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Trim().StartsWith("-"))
                {
                    if (args[i].Trim() == "-h" || args[i].Trim() == "-?")
                    {
                        list.Clear();
                        list.Add(args[i].Trim(), "help");
                        break;
                    }
                    else
                    {
                        list.Add(args[i++].Trim().ToLower(), args[i].Trim());
                    }
                }
                else if (args[i].Trim().ToLower().StartsWith("http://"))
                {
                    list.Add("-url", args[i].Trim());
                }
                else
                {
                    list = null;
                }
            }

            return list;
        }

        internal static void ProgressStatus(int current, int maxCount)
        {
            int scrWidth = Console.WindowWidth - 17;
            string progressBar = "Progress: {0}% [{1}]{2}";

            int top = Console.CursorTop;
            int left = Console.CursorLeft;
            int cursor = current * scrWidth / maxCount;
            string pct = (current * 100 / maxCount).ToString().PadLeft(3);
            Console.SetCursorPosition(0, top);
            Console.Write(current < maxCount ? "Task Started..." : "Task Completed! ");
            Console.SetCursorPosition(0, top + 1);
            Console.Write(string.Format(progressBar, pct, (cursor > 0 ? new string('=', cursor - 1) + (current == maxCount ? "=" : ">") : "") + new string(' ', scrWidth - cursor), (current == maxCount ? "\n" : "")));
            if (current < maxCount)
            {
                Console.SetCursorPosition(left, top);
            }
        }

        internal static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        internal static void ShowError()
        {
            ShowMessage("Invalid parameters. Please use [Help] command to see the usage.\n");
        }

        private static string helpText = null;
        internal static void ShowHelp()
        {
            if (helpText == null)
            {
                var assembly = typeof(CmdletHelper).Assembly;
                helpText = string.Format("\n{0} [Version {1}]\n", AssemblyInfo.Title, AssemblyInfo.Version);
                helpText += string.Format("{0}  {1} {2}.\n\n", AssemblyInfo.Copyright, AssemblyInfo.Company, AssemblyInfo.Trademark);
                
                try
                {
                    using (StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(assembly.EntryPoint.DeclaringType.Namespace + ".help.txt")))
                    {
                        helpText += sr.ReadToEnd();
                    }
                }
                catch(Exception e)
                {
                    helpText += string.Format("Help on Usage({0})...", Path.GetFileNameWithoutExtension(assembly.Location));
                }
            }

            ShowMessage(helpText + "\n");
        }
    }

    internal static class AssemblyInfo
    {
        private static Assembly assembly = Assembly.GetExecutingAssembly();
        private static string fileName = System.IO.Path.GetFileNameWithoutExtension(assembly.CodeBase);
        private static string title = null;
        private static string company = null;
        private static string product = null;
        private static string copyright = null;
        private static string trademark = null;
        private static string version = null;

        public static string Title
        {
            get
            {
                if (string.IsNullOrEmpty(title))
                {
                    object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

                    if (attributes.Length > 0)
                    {
                        AssemblyTitleAttribute attribute = (AssemblyTitleAttribute)attributes[0];
                        if (attribute.Title.Length > 0)
                        {
                            title = attribute.Title;
                        }
                        else
                        {
                            title = fileName;
                        }
                    }
                    else
                    {
                        title = fileName;
                    }
                }

                return title;
            }
        }

        public static string Company
        {
            get
            {
                if (string.IsNullOrEmpty(company))
                {
                    object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

                    if (attributes.Length > 0)
                    {
                        AssemblyCompanyAttribute attribute = (AssemblyCompanyAttribute)attributes[0];
                        if (attribute.Company.Length > 0)
                        {
                            company = attribute.Company;
                        }
                        else
                        {
                            company = fileName;
                        }
                    }
                    else
                    {
                        company = fileName;
                    }
                }

                return company;
            }
        }

        public static string Product
        {
            get
            {
                if (string.IsNullOrEmpty(product))
                {
                    object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

                    if (attributes.Length > 0)
                    {
                        AssemblyProductAttribute attribute = (AssemblyProductAttribute)attributes[0];
                        if (attribute.Product.Length > 0)
                        {
                            product = attribute.Product;
                        }
                        else
                        {
                            product = fileName;
                        }
                    }
                    else
                    {
                        product = fileName;
                    }
                }

                return product;
            }
        }

        public static string Copyright
        {
            get
            {
                if (string.IsNullOrEmpty(copyright))
                {
                    object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

                    if (attributes.Length > 0)
                    {
                        AssemblyCopyrightAttribute attribute = (AssemblyCopyrightAttribute)attributes[0];
                        if (attribute.Copyright.Length > 0)
                        {
                            copyright = attribute.Copyright;
                        }
                        else
                        {
                            copyright = fileName;
                        }
                    }
                    else
                    {
                        copyright = fileName;
                    }
                }

                return copyright;
            }
        }

        public static string Trademark
        {
            get
            {
                if (string.IsNullOrEmpty(trademark))
                {
                    object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false);

                    if (attributes.Length > 0)
                    {
                        AssemblyTrademarkAttribute attribute = (AssemblyTrademarkAttribute)attributes[0];
                        if (attribute.Trademark.Length > 0)
                        {
                            trademark = attribute.Trademark;
                        }
                        else
                        {
                            trademark = fileName;
                        }
                    }
                    else
                    {
                        trademark = fileName;
                    }
                }

                return trademark;
            }
        }

        public static string Version
        {
            get
            {
                if (string.IsNullOrEmpty(version))
                {
                    version = assembly.GetName().Version.ToString();
                }

                return version;
            }
        }
    }
}
