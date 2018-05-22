
using System;
using System.Drawing;

using Autodesk.Max;
using Autodesk.Max.BaseInterface;
using Autodesk.Max.IColorManager;

namespace BatchLabs.Max2019.Plugin.Contract.Stubs
{
    /// <summary>
    /// Override functionality from the Max SDK for when we are running in local debug mode. 
    /// </summary>
    public class ColorManagerStub : IIColorManager
    {
        public Color GetColor(GuiColors kind, State state)
        {
            switch (kind)
            {
                case GuiColors.Background:
                    return Color.FromArgb(255, 67, 66, 66);

                case GuiColors.Text:
                    return Color.FromArgb(208, 208, 208);

                default:
                    return Color.DeepPink;
            }
        }

        public Color CustSysColor(GuiColors which)
        {
            return GetColor(which, State.Normal);
        }

        #region Unused Properties and Overrides

        public bool RegisterColor(uint id, string pName, string pCategory, Color defaultValue, Color defaultValueDisabled, Color defaultValueHover)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IInterfaceServer other)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IntPtr NativePointer { get; }
        public IBaseInterface GetInterface(IInterface_ID id)
        {
            throw new NotImplementedException();
        }

        public bool RegisterNotifyCallback(IInterfaceNotifyCallback incb)
        {
            throw new NotImplementedException();
        }

        public void UnRegisterNotifyCallback(IInterfaceNotifyCallback incb)
        {
            throw new NotImplementedException();
        }

        public void ReleaseInterface()
        {
            throw new NotImplementedException();
        }

        public void DeleteInterface()
        {
            throw new NotImplementedException();
        }

        public IBaseInterface CloneInterface(IntPtr remapDir)
        {
            throw new NotImplementedException();
        }

        public IInterface_ID Id { get; }

        public LifetimeType LifetimeControl { get; }

        public IBaseInterface AcquireInterface { get; }

        public int _DispatchFn(short fid, int t, IFPValue result, IFPParams p)
        {
            throw new NotImplementedException();
        }

        public int Invoke(short fid, int t, IFPParams params_)
        {
            throw new NotImplementedException();
        }

        public int Invoke(short fid, IFPParams params_)
        {
            throw new NotImplementedException();
        }

        public int Invoke(short fid, int t, IFPValue result, IFPParams params_)
        {
            throw new NotImplementedException();
        }

        public int Invoke(short fid, IFPValue result, IFPParams params_)
        {
            throw new NotImplementedException();
        }

        public short FindFn(string name)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(short actionID)
        {
            throw new NotImplementedException();
        }

        public bool IsChecked(short actionID)
        {
            throw new NotImplementedException();
        }

        public bool IsVisible(short actionID)
        {
            throw new NotImplementedException();
        }

        public short GetIsEnabled(short actionID)
        {
            throw new NotImplementedException();
        }

        public short GetIsChecked(short actionID)
        {
            throw new NotImplementedException();
        }

        public short GetIsVisible(short actionID)
        {
            throw new NotImplementedException();
        }

        public void EnableActions(bool onOff)
        {
            throw new NotImplementedException();
        }

        public IFPInterfaceDesc Desc { get; }
        public IActionTable ActionTable { get; }
        public void Init()
        {
            throw new NotImplementedException();
        }

        public void LoadDescriptor(IInterface_ID id, string int_name, IntPtr descr, IClassDesc pCD, uint flag, params object[] param5)
        {
            throw new NotImplementedException();
        }

        public void AppendFunction(int id, params object[] param1)
        {
            throw new NotImplementedException();
        }

        public void AppendProperty(int id, params object[] param1)
        {
            throw new NotImplementedException();
        }

        public void AppendEnum(int id, params object[] param1)
        {
            throw new NotImplementedException();
        }

        public void SetClassDesc(IClassDesc i_cd)
        {
            throw new NotImplementedException();
        }

        public IFPFunctionDef GetFnDef(short fid)
        {
            throw new NotImplementedException();
        }

        public string GetRsrcString(IntPtr id)
        {
            throw new NotImplementedException();
        }

        public IInterface_ID Id_ { get; set; }
        public string InternalName { get; set; }
        public IntPtr Description { get; set; }
        public IClassDesc Cd { get; set; }
        public ushort Flags { get; set; }
        public ITab<IFPFunctionDef> Functions { get; set; }
        public ITab<IFPPropDef> Props { get; set; }
        public ITab<IFPEnum> Enumerations { get; set; }
        public IRollout Rollout { get; set; }
        public IActionTable ActionTable_ { get; set; }
        public IntPtr HInstance { get; }

        public bool LoadColorFile(string pFileName)
        {
            throw new NotImplementedException();
        }

        public bool SaveColorFile(string pFileName)
        {
            throw new NotImplementedException();
        }

        public bool SetColor(GuiColors id, Color color, State state)
        {
            throw new NotImplementedException();
        }

        public IPoint3 GetColorAsPoint3(GuiColors id, State state)
        {
            throw new NotImplementedException();
        }

        public IntPtr GetBrush(GuiColors id, State state)
        {
            throw new NotImplementedException();
        }

        public string GetName(GuiColors id)
        {
            throw new NotImplementedException();
        }

        public string GetCategory(GuiColors id)
        {
            throw new NotImplementedException();
        }

        public IntPtr CustSysColorBrush(GuiColors which)
        {
            throw new NotImplementedException();
        }

        public IPoint3 GetOldUIColor(GuiColors which)
        {
            throw new NotImplementedException();
        }

        public void SetOldUIColor(GuiColors which, IPoint3 clr)
        {
            throw new NotImplementedException();
        }

        public IPoint3 GetOldDefaultUIColor(GuiColors which)
        {
            throw new NotImplementedException();
        }

        public float GetIconColorScale(IconType type, IconColorScale which)
        {
            throw new NotImplementedException();
        }

        public void SetIconColorScale(IconType type, IconColorScale which, float value)
        {
            throw new NotImplementedException();
        }

        public bool GetIconColorInvert(IconType type)
        {
            throw new NotImplementedException();
        }

        public void SetIconColorInvert(IconType type, bool value)
        {
            throw new NotImplementedException();
        }

        public Color GetDefaultColor(GuiColors id, State state)
        {
            throw new NotImplementedException();
        }

        public Color GetOldUIColorCOLORREF(GuiColors which)
        {
            throw new NotImplementedException();
        }

        public void RepaintUI(RepaintType type)
        {
            throw new NotImplementedException();
        }

        public bool SetIconFolder(string pFolder)
        {
            throw new NotImplementedException();
        }

        public void ReInitIcons()
        {
            throw new NotImplementedException();
        }

        public bool ResolveIconFolder(string pFilename, ref string path)
        {
            throw new NotImplementedException();
        }

        public string ColorFile { get; }

        public AppFrameColorTheme AppFrameColorTheme { get; set; }

        public string FileName { get; }

        public string IconFolder { get; }

        #endregion
    }
}
