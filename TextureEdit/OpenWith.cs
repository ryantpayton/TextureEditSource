using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TextureEdit
{
    class OpenWith
    {
        public OpenWith(string FilePath)
        {
            this.FilePath = FilePath;
        }
        public string FilePath;
        public List<OpenWithApplication> GetApplications()
        {
            List<OpenWithApplication> apps = new List<OpenWithApplication>();
            using (RegistryKey classesRoot = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Default))
            {
                using (RegistryKey extensionKey = classesRoot.OpenSubKey(Path.GetExtension(FilePath)))
                {
                    string defaultHandler = (string)extensionKey.GetValue("");
                    using (RegistryKey progIds = extensionKey.OpenSubKey("OpenWithProgids"))
                    {
                        foreach (string valueName in progIds.GetValueNames())
                        {
                            if (valueName == defaultHandler)
                            {
                                continue;
                            }
                            if (valueName.Length > 4 && valueName.Substring(0, 4) == "AppX")
                            {
                                apps.Add(new OpenWithUwp(this, valueName));
                            } else
                            {
                                apps.Add(new OpenWithExe(this, valueName));
                            }
                        }
                    }
                }
            }
            return apps;
        }
        public OpenWithApplication GetDefault()
        {
            try
            {
                using (RegistryKey classesRoot = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Default))
                {
                    using (RegistryKey extensionKey = classesRoot.OpenSubKey(Path.GetExtension(FilePath)))
                    {
                        string valueName = (string)extensionKey.GetValue("");
                        if (valueName.Length > 4 && valueName.Substring(0, 4) == "AppX")
                        {
                            return new OpenWithUwp(this, valueName);
                        }
                        else
                        {
                            return new OpenWithExe(this, valueName);
                        }
                    }
                }
            } catch
            {
                return GetApplications()[0];
            }
        }
    }
    abstract class OpenWithApplication
    {
        public OpenWith File;
        public string Name;
        public string ProgID;
        public abstract Process Start();
    }
    class OpenWithUwp : OpenWithApplication
    {
        public enum ActivateOptions
        {
            None = 0x00000000,  // No flags set
            DesignMode = 0x00000001,  // The application is being activated for design mode, and thus will not be able to
                                      // to create an immersive window. Window creation must be done by design tools which
                                      // load the necessary components by communicating with a designer-specified service on
                                      // the site chain established on the activation manager.  The splash screen normally
                                      // shown when an application is activated will also not appear.  Most activations
                                      // will not use this flag.
            NoErrorUI = 0x00000002,  // Do not show an error dialog if the app fails to activate.                                
            NoSplashScreen = 0x00000004,  // Do not show the splash screen when activating the app.
        }

        [ComImport, Guid("2e941141-7f97-4756-ba1d-9decde894a3d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IApplicationActivationManager
        {
            // Activates the specified immersive application for the "Launch" contract, passing the provided arguments
            // string into the application.  Callers can obtain the process Id of the application instance fulfilling this contract.
            IntPtr ActivateApplication([In] String appUserModelId, [In] String arguments, [In] ActivateOptions options, [Out] out UInt32 processId);
            IntPtr ActivateForFile([In] String appUserModelId, [In] IntPtr /*IShellItemArray* */ itemArray, [In] String verb, [Out] out UInt32 processId);
            IntPtr ActivateForProtocol([In] String appUserModelId, [In] IntPtr /* IShellItemArray* */itemArray, [Out] out UInt32 processId);
        }

        [ComImport, Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")]//Application Activation Manager
        class ApplicationActivationManager : IApplicationActivationManager
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)/*, PreserveSig*/]
            public extern IntPtr ActivateApplication([In] String appUserModelId, [In] String arguments, [In] ActivateOptions options, [Out] out UInt32 processId);
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            public extern IntPtr ActivateForFile([In] String appUserModelId, [In] IntPtr /*IShellItemArray* */ itemArray, [In] String verb, [Out] out UInt32 processId);
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            public extern IntPtr ActivateForProtocol([In] String appUserModelId, [In] IntPtr /* IShellItemArray* */itemArray, [Out] out UInt32 processId);
        }
        [DllImport("shlwapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false, ThrowOnUnmappableChar = true)]
        public static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, IntPtr ppvReserved);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void SHCreateItemFromParsingName(
        [In][MarshalAs(UnmanagedType.LPWStr)] string pszPath,
        [In] IntPtr pbc,
        [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid,
        [Out] out IntPtr ppv);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void SHCreateShellItemArrayFromShellItem(
        [In] IntPtr IShellItem,
        [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid,
        [Out] out IntPtr ppv);

        private string AppUserModelID;
        public OpenWithUwp(OpenWith File, string ProgID)
        {
            this.File = File;
            this.ProgID = ProgID;
            using (RegistryKey classesRoot = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Default))
            {
                using (RegistryKey progId = classesRoot.OpenSubKey(this.ProgID))
                {
                    using (RegistryKey applicationInfo = progId.OpenSubKey("Application"))
                    {
                        string nameUnexpanded = (string)applicationInfo.GetValue("ApplicationName");
                        if (nameUnexpanded[0] == '@')
                        {
                            StringBuilder sb = new StringBuilder();
                            SHLoadIndirectString(nameUnexpanded, sb, sb.Capacity, IntPtr.Zero);
                            this.Name = sb.ToString();
                        } else
                        {
                            this.Name = nameUnexpanded;
                        }
                        this.AppUserModelID = (string)applicationInfo.GetValue("AppUserModelID");
                    }
                }
            }
        }
        public override Process Start()
        {
            IntPtr shellItem = IntPtr.Zero;
            Guid IShellItem = new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe");
            SHCreateItemFromParsingName(this.File.FilePath, IntPtr.Zero, IShellItem, out shellItem);
            Guid IShellItemArray = new Guid("B63EA76D-1F85-456F-A19C-48159EFA858B");
            IntPtr shellItemArray = IntPtr.Zero;
            SHCreateShellItemArrayFromShellItem(shellItem, IShellItemArray, out shellItemArray);
            ApplicationActivationManager aam = new ApplicationActivationManager();
            uint procId;
            aam.ActivateForFile(this.AppUserModelID, shellItemArray, "open", out procId);
            return Process.GetProcessById((int)procId);
        }
    }
    class OpenWithExe : OpenWithApplication
    {
        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW(
           [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine,
           out int pNumArgs);
        [DllImport("kernel32.dll")]
        static extern IntPtr LocalFree(IntPtr hMem);

        static string[] SplitArgs(string unsplitArgumentLine)
        {
            int numberOfArgs;
            IntPtr ptrToSplitArgs;
            string[] splitArgs;

            ptrToSplitArgs = CommandLineToArgvW(unsplitArgumentLine, out numberOfArgs);

            // CommandLineToArgvW returns NULL upon failure.
            if (ptrToSplitArgs == IntPtr.Zero)
                throw new ArgumentException("Unable to split argument.", new Win32Exception());

            // Make sure the memory ptrToSplitArgs to is freed, even upon failure.
            try
            {
                splitArgs = new string[numberOfArgs];

                // ptrToSplitArgs is an array of pointers to null terminated Unicode strings.
                // Copy each of these strings into our split argument array.
                for (int i = 0; i < numberOfArgs; i++)
                    splitArgs[i] = Marshal.PtrToStringUni(
                        Marshal.ReadIntPtr(ptrToSplitArgs, i * IntPtr.Size));

                return splitArgs;
            }
            finally
            {
                // Free memory obtained by CommandLineToArgW.
                LocalFree(ptrToSplitArgs);
            }
        }

        private string exeCommand;
        public OpenWithExe(OpenWith File, string ProgID)
        {
            this.File = File;
            this.ProgID = ProgID;
            using (RegistryKey classesRoot = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Default))
            {
                using (RegistryKey progid = classesRoot.OpenSubKey(ProgID))
                {
                    using (RegistryKey shell = progid.OpenSubKey("shell"))
                    {
                        using (RegistryKey open = shell.OpenSubKey("open"))
                        {
                            if (open == null)
                            {
                                using (RegistryKey edit = shell.OpenSubKey("edit"))
                                {
                                    if (edit == null)
                                    {
                                        throw new InvalidOperationException();
                                    }
                                    using (RegistryKey command = edit.OpenSubKey("command"))
                                    {
                                        string commandText = (string)command.GetValue("");
                                        exeCommand = commandText.Replace("%1", File.FilePath);
                                        string exePath = SplitArgs(exeCommand)[0];
                                        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(exePath);
                                        this.Name = fvi.ProductName;
                                    }
                                }
                            } else
                            {
                                using (RegistryKey command = open.OpenSubKey("command"))
                                {
                                    string commandText = (string)command.GetValue("");
                                    exeCommand = commandText.Replace("%1", File.FilePath);
                                    string exePath = SplitArgs(exeCommand)[0];
                                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(exePath);
                                    this.Name = fvi.ProductName;
                                }
                            }
                        }
                    }
                }
            }
        }
        public override Process Start()
        {
            string[] args = SplitArgs(exeCommand);
            string arguments = "";
            for (int i = 1; i < args.Length; i++)
            {
                arguments += " \"" + args[i] + "\"";
            }
            Process process = Process.Start(args[0], arguments);
            return process;
        }
    }
}
