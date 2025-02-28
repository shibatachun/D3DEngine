﻿using D3DengineEditor.Utilities;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using System.Text;
using System.Threading.Tasks;

namespace D3DengineEditor.GameDev
{
    static class VisualStudio
    {
        private static EnvDTE80.DTE2 _vsInstance = null;
        private static readonly string _progID = "VisualStudio.DTE.17.0";

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);
        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable ppot);
        public static void OpenVisualStudio(string solutionPath)
        {
            IRunningObjectTable rot = null;
            IEnumMoniker monikerTable = null;
            IBindCtx bindCtx = null;    
            try
            {


                if (_vsInstance == null)
                {
                    //find and open visual
                    var hResult = GetRunningObjectTable(0, out rot);
                    if (hResult < 0 || rot == null) throw new COMException($"GetRunningObjectTable() return HRESULT: {hResult:X8}");
                    rot.EnumRunning(out monikerTable);
                    monikerTable.Reset();

                    hResult = CreateBindCtx(0, out bindCtx);
                    if (hResult < 0 || rot == null) throw new COMException($"CreateBindCtx() return HRESULT: {hResult:X8}");
                    IMoniker[] currentMoniker = new IMoniker[1];
                    while(monikerTable.Next(1,currentMoniker,IntPtr.Zero) == 0)
                    {
                        string name = string.Empty;
                        currentMoniker[0]?.GetDisplayName(bindCtx, null, out name);
                        if (name.Contains(_progID))
                        {
                            hResult = rot.GetObject(currentMoniker[0], out object obj);
                            if (hResult < 0 || obj == null) throw new COMException($"Running object table's GetObject() return HRESULT: {hResult:X8}");
                            EnvDTE80.DTE2 dte = obj as EnvDTE80.DTE2;
                            var solutionName = dte.Solution.FullName;
                            if(solutionName == solutionPath)
                            {
                                _vsInstance = dte;
                                break;
                            }
                        }
                    }
                    if (_vsInstance == null)
                    {
                        Type visualStudioType = Type.GetTypeFromProgID(_progID, true);
                        _vsInstance = Activator.CreateInstance(visualStudioType) as EnvDTE80.DTE2;
                    }

                }
            }
            catch (Exception ex) { 
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, "failed to open Visual Studio");

            }

            finally
            {
                if(monikerTable != null) Marshal.ReleaseComObject(monikerTable);
                if(rot!=null) Marshal.ReleaseComObject(rot);
                if(bindCtx!=null) Marshal.ReleaseComObject(bindCtx);
            }
        }
        public static void CloseVisualStudio()
        {
            if(_vsInstance?.Solution.IsOpen == true)
            {
                _vsInstance.ExecuteCommand("File.SavaAll");
                _vsInstance.Solution.Close(true);
            }
            _vsInstance?.Quit();
        }
    }
}
