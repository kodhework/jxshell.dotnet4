using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace jxshell
{
	public class environment
	{
		private static int uniqueId_;

		public static string[] argv;

		public static string environmentPath;

		private static string exPath;

		public static string libraryPath;

		public static string appdataPath;

		public static string compilationPath;

		public static string applicationsPath;

		public static string languagePath;

		public static string globalAssemblyPath;

		public static string commonLibraryPath;

		public static string commonApplicationsPath;

		public static List<Assembly> assemblies;

		private static Random r;

		private static string _lastPath;

		public static List<string> directories;

		public static string executableFile
		{
			get
			{
				return Assembly.GetExecutingAssembly().Location;
			}
		}

		public static string executablePath
		{
			get
			{
				return environment.exPath;
			}
		}

		public static bool linux
		{
			get
			{
				return environment.os.Platform == PlatformID.Unix;
			}
		}

		public static OperatingSystem os
		{
			get
			{
				return Environment.OSVersion;
			}
		}

		public static bool osx
		{
			get
			{
				return environment.os.Platform == PlatformID.MacOSX;
			}
		}

		public static bool unix
		{
			get
			{
				return (environment.linux ? true : environment.osx);
			}
		}

		public static bool windows
		{
			get
			{
				return environment.os.Platform == PlatformID.Win32NT;
			}
		}

		static environment()
		{
			environment.uniqueId_ = 0;
			environment.argv = new string[0];
			environment.environmentPath = "";
			environment.exPath = "";
			environment.libraryPath = "";
			environment.appdataPath = "";
			environment.compilationPath = "";
			environment.applicationsPath = "";
			environment.languagePath = "";
			environment.globalAssemblyPath = "";
			environment.commonLibraryPath = "";
			environment.commonApplicationsPath = "";
			environment.assemblies = new List<Assembly>();
			environment.r = new Random();
			environment._lastPath = "";
			environment.directories = new List<string>();
		}

		public environment()
		{
		}

		public static string addBs(string path)
		{
			string str;
			if ((path.EndsWith("/") ? false : !path.EndsWith("\\")))
			{
				char altDirectorySeparatorChar = Path.AltDirectorySeparatorChar;
				str = string.Concat(path, altDirectorySeparatorChar.ToString());
			}
			else
			{
				str = path;
			}
			path = str;
			return path;
		}

		public static string getCompilationFile()
		{
			string str = string.Concat(environment.compilationPath, environment.uniqueId(), ".$$.dll");
			return str;
		}

		public static string getCompilationFile(string id)
		{
			return string.Concat(environment.compilationPath, id, ".DLL");
		}

		public static string getDirectoryPathForUri(Uri u)
		{
			string str;
			if (u.IsFile)
			{
				str = environment.addBs(Path.GetDirectoryName(u.LocalPath));
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (u.Scheme != "file")
				{
					stringBuilder.Append(u.Scheme).Append(":/");
				}
				else
				{
					stringBuilder.Append(u.Scheme).Append("://");
				}
				stringBuilder.Append(u.Authority);
				string str1 = string.Join("", u.Segments, 0, (int)u.Segments.Length - 1);
				stringBuilder.Append(str1);
				str = environment.addBs(stringBuilder.ToString());
			}
			return str;
		}

		public static string getLastPath()
		{
			return environment._lastPath;
		}

		public static TimeZoneInfo getTimeZoneInfo(string zone)
		{
			return TimeZoneInfo.FindSystemTimeZoneById(zone);
		}

		public static void initEnvironment()
		{
			environment.exPath = environment.addBs(Path.GetDirectoryName(environment.executableFile));
			PropertyInfo property = typeof(TimeZoneInfo).GetProperty("TimeZoneDirectory", BindingFlags.Static | BindingFlags.NonPublic);
			if (property != null)
			{
				string str2 = Path.Combine(environment.exPath, "zoneinfo");
				property.SetValue(null, str2, new object[0]);
			}
			if (environment.windows)
			{
				//environment.environmentPath = string.Concat(environment.addBs(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System))), ".jxshell\\");
				environment.environmentPath = string.Concat(Environment.GetEnvironmentVariable("USERPROFILE"), "/kodnet/");
			}
			else
			{
				environment.environmentPath = string.Concat(Environment.GetEnvironmentVariable("HOME"), "/kodnet/");
			}
			
			var commonPath = string.Concat(environment.addBs(environment.environmentPath), "common");
			environment.commonLibraryPath = string.Concat(environment.addBs(environment.environmentPath), "common/library/");
			environment.commonApplicationsPath = string.Concat(environment.addBs(environment.environmentPath), "common/applications/");
			environment.libraryPath = string.Concat(environment.environmentPath, "library/");
			environment.appdataPath = string.Concat(environment.environmentPath, "appdata/");
			environment.compilationPath = string.Concat(environment.environmentPath, "compilation/");
			environment.applicationsPath = string.Concat(environment.environmentPath, "applications/");
			environment.languagePath = string.Concat(environment.libraryPath, "lang/");
			environment.globalAssemblyPath = string.Concat(environment.environmentPath, "global/");
			
			
			environment.mkDir(environment.environmentPath);
			environment.mkDir(commonPath);
			environment.mkDir(environment.libraryPath);
			environment.mkDir(environment.appdataPath);
			environment.mkDir(environment.compilationPath);
			environment.mkDir(environment.applicationsPath);
			environment.mkDir(environment.globalAssemblyPath);
			try
			{
				environment.mkDir(environment.commonLibraryPath);
			}
			catch (Exception exception)
			{
			}
			environment.loadAssembly(Assembly.GetExecutingAssembly(), true);
			environment.loadAssembly(Assembly.GetAssembly(typeof(Microsoft.CSharp.RuntimeBinder.Binder)), true);
			environment.loadAssemblyPartialName("System.Core");
			environment.directories.Add(environment.appdataPath);
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler((object s, ResolveEventArgs e) => {
				Assembly assembly;
				Assembly assembly1;
				if (e.Name == "System")
				{
					assembly = typeof(WebRequest).Assembly;
				}
				else
				{
					string name = (new AssemblyName(e.Name)).Name;
					if (name.ToLower().EndsWith(".resources"))
					{
						name = name.Substring(0, name.Length - ".resources".Length);
						assembly = Assembly.GetExecutingAssembly();
					}
					else
					{
						if (environment.unix)
						{
							Assembly assembly2 = null;
							try
							{
								assembly2 = Assembly.Load(name);
							}
							catch
							{
							}
							if (assembly2 != null)
							{
								assembly = assembly2;
								assembly1 = assembly;
								return assembly1;
							}
						}
						string str = name;
						string str1 = string.Format(string.Concat(environment.exPath, "{0}.dll"), str);
						if (!File.Exists(str1))
						{
							str1 = string.Format(string.Concat(environment.compilationPath, "{0}.dll"), str);
							if (!File.Exists(str1))
							{
								str1 = string.Format(string.Concat(environment.libraryPath, "{0}.dll"), str);
								if (!File.Exists(str1))
								{
									str1 = string.Format(string.Concat(environment.appdataPath, "{0}.dll"), str);
									if (!File.Exists(str1))
									{
										str1 = environment.locateInGlobalPath(str);
										if (!File.Exists(str1))
										{
											bool flag = true;
											int num = 0;
											while (!File.Exists(str1) & flag)
											{
												if (environment.directories.Count == num)
												{
													flag = false;
												}
												else
												{
													str1 = string.Format(string.Concat(environment.addBs(environment.directories[num]), "{0}.dll"), str);
													num++;
												}
											}
										}
									}
								}
							}
						}
						assembly = Assembly.LoadFrom(str1);
					}
				}
				assembly1 = assembly;
				return assembly1;
			});
		}

		public static void loadAssembly(string name)
		{
			environment.loadAssembly(Assembly.Load(name), true);
		}

		public static void loadAssembly(Assembly a, bool include = true)
		{
			if (environment.assemblies.IndexOf(a) < 0)
			{
				environment.assemblies.Add(a);
			}
		}

		public static void loadAssemblyFromFile(string file)
		{
			try
			{
				environment.loadAssembly(Assembly.LoadFile(file), true);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw new Exception(string.Concat("Error al cargar el archivo '", file, "'. Detalle del error: ", exception.Message));
			}
		}

		public static void loadAssemblyPartialName(string name)
		{
			environment.loadAssembly(Assembly.LoadWithPartialName(name), true);
		}

		public static string locateInGlobalPath(string file)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
			string[] directories = Directory.GetDirectories(environment.globalAssemblyPath);
			string str = string.Format(string.Concat(environment.globalAssemblyPath, "{0}.dll"), fileNameWithoutExtension);
			if (!File.Exists(str))
			{
				string[] strArrays = directories;
				int num = 0;
				while (num < (int)strArrays.Length)
				{
					string str1 = strArrays[num];
					str = string.Format(string.Concat(environment.addBs(str1), "{0}.dll"), fileNameWithoutExtension);
					if (File.Exists(str))
					{
						break;
					}
					else
					{
						str = "";
						num++;
					}
				}
			}
			return str;
		}

		public static void mkDir(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public static string uniqueId()
		{
			environment.uniqueId_++;
			if (environment.uniqueId_ > 1000)
			{
				environment.uniqueId_ = 0;
			}
			string[] str = new string[] { "_", null, null, null, null };
			int hashCode = DateTime.Now.Ticks.GetHashCode();
			str[1] = hashCode.ToString("x");
			int num = environment.r.Next();
			str[2] = num.ToString();
			str[3] = "_";
			str[4] = environment.uniqueId_.ToString();
			return string.Concat(str);
		}
	}
}