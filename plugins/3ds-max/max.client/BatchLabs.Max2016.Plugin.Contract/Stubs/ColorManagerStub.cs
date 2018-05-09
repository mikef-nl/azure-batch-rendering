
using System;
using System.Drawing;

using Autodesk.Max;
using Autodesk.Max.BaseInterface;
using Autodesk.Max.IColorManager;

namespace BatchLabs.Max2016.Plugin.Contract.Stubs
{
    public class ColorManagerStub : IIColorManager
    {
        public Color GetColor(GuiColors kind)
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

        #region Unused Properties and Overrides

        bool IEquatable<IInterfaceServer>.Equals(IInterfaceServer other)
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        public IntPtr NativePointer { get; }

        IBaseInterface IInterfaceServer.GetInterface(IInterface_ID id)
        {
            throw new NotImplementedException();
        }

        bool IBaseInterface.RegisterNotifyCallback(IInterfaceNotifyCallback incb)
        {
            throw new NotImplementedException();
        }

        void IBaseInterface.UnRegisterNotifyCallback(IInterfaceNotifyCallback incb)
        {
            throw new NotImplementedException();
        }

        void IBaseInterface.ReleaseInterface()
        {
            throw new NotImplementedException();
        }

        void IBaseInterface.DeleteInterface()
        {
            throw new NotImplementedException();
        }

        IBaseInterface IBaseInterface.CloneInterface(IntPtr remapDir)
        {
            throw new NotImplementedException();
        }

        public IInterface_ID Id { get; }

        public LifetimeType LifetimeControl { get; }

        public IBaseInterface AcquireInterface { get; }

        int IFPInterface._DispatchFn(short fid, int t, IFPValue result, IFPParams p)
        {
            throw new NotImplementedException();
        }

        int IFPInterface.Invoke(short fid, int t, IFPParams params_)
        {
            throw new NotImplementedException();
        }

        int IFPInterface.Invoke(short fid, IFPParams params_)
        {
            throw new NotImplementedException();
        }

        int IFPInterface.Invoke(short fid, int t, IFPValue result, IFPParams params_)
        {
            throw new NotImplementedException();
        }

        int IFPInterface.Invoke(short fid, IFPValue result, IFPParams params_)
        {
            throw new NotImplementedException();
        }

        short IFPInterface.FindFn(string name)
        {
            throw new NotImplementedException();
        }

        bool IFPInterface.IsEnabled(short actionID)
        {
            throw new NotImplementedException();
        }

        bool IFPInterface.IsChecked(short actionID)
        {
            throw new NotImplementedException();
        }

        bool IFPInterface.IsVisible(short actionID)
        {
            throw new NotImplementedException();
        }

        short IFPInterface.GetIsEnabled(short actionID)
        {
            throw new NotImplementedException();
        }

        short IFPInterface.GetIsChecked(short actionID)
        {
            throw new NotImplementedException();
        }

        short IFPInterface.GetIsVisible(short actionID)
        {
            throw new NotImplementedException();
        }

        void IFPInterface.EnableActions(bool onOff)
        {
            throw new NotImplementedException();
        }

        public IFPInterfaceDesc Desc { get; }

        public IActionTable ActionTable { get; }

        void IFPInterfaceDesc.Init()
        {
            throw new NotImplementedException();
        }

        void IFPInterfaceDesc.LoadDescriptor(IInterface_ID id, string int_name, IntPtr descr, IClassDesc cd, uint flag, params object[] param5)
        {
            throw new NotImplementedException();
        }

        void IFPInterfaceDesc.AppendFunction(int id, params object[] param1)
        {
            throw new NotImplementedException();
        }

        void IFPInterfaceDesc.AppendProperty(int id, params object[] param1)
        {
            throw new NotImplementedException();
        }

        void IFPInterfaceDesc.AppendEnum(int id, params object[] param1)
        {
            throw new NotImplementedException();
        }

        void IFPInterfaceDesc.SetClassDesc(IClassDesc i_cd)
        {
            throw new NotImplementedException();
        }

        IFPFunctionDef IFPInterfaceDesc.GetFnDef(short fid)
        {
            throw new NotImplementedException();
        }

        string IFPInterfaceDesc.GetRsrcString(IntPtr id)
        {
            throw new NotImplementedException();
        }

        IInterface_ID IFPInterfaceDesc.Id_ { get; set; }

        string IFPInterfaceDesc.InternalName { get; set; }

        IntPtr IFPInterfaceDesc.Description { get; set; }

        IClassDesc IFPInterfaceDesc.Cd { get; set; }

        ushort IFPInterfaceDesc.Flags { get; set; }

        ITab<IFPFunctionDef> IFPInterfaceDesc.Functions { get; set; }

        ITab<IFPPropDef> IFPInterfaceDesc.Props { get; set; }

        ITab<IFPEnum> IFPInterfaceDesc.Enumerations { get; set; }

        IRollout IFPInterfaceDesc.Rollout { get; set; }

        IActionTable IFPInterfaceDesc.ActionTable_ { get; set; }

        IntPtr IFPInterfaceDesc.HInstance { get; }

        bool IIColorManager.RegisterColor(uint id, string pName, string pCategory, Color defaultValue)
        {
            throw new NotImplementedException();
        }

        bool IIColorManager.LoadColorFile(string pFileName)
        {
            throw new NotImplementedException();
        }

        bool IIColorManager.SaveColorFile(string pFileName)
        {
            throw new NotImplementedException();
        }

        bool IIColorManager.SetColor(GuiColors id, Color color)
        {
            throw new NotImplementedException();
        }

        IPoint3 IIColorManager.GetColorAsPoint3(GuiColors id)
        {
            throw new NotImplementedException();
        }

        IntPtr IIColorManager.GetBrush(GuiColors id)
        {
            throw new NotImplementedException();
        }

        string IIColorManager.GetName(GuiColors id)
        {
            throw new NotImplementedException();
        }

        string IIColorManager.GetCategory(GuiColors id)
        {
            throw new NotImplementedException();
        }

        Color IIColorManager.CustSysColor(GuiColors which)
        {
            throw new NotImplementedException();
        }

        IntPtr IIColorManager.CustSysColorBrush(GuiColors which)
        {
            throw new NotImplementedException();
        }

        IPoint3 IIColorManager.GetOldUIColor(GuiColors which)
        {
            throw new NotImplementedException();
        }

        void IIColorManager.SetOldUIColor(GuiColors which, IPoint3 clr)
        {
            throw new NotImplementedException();
        }

        IPoint3 IIColorManager.GetOldDefaultUIColor(GuiColors which)
        {
            throw new NotImplementedException();
        }

        float IIColorManager.GetIconColorScale(IconType type, IconColorScale which)
        {
            throw new NotImplementedException();
        }

        void IIColorManager.SetIconColorScale(IconType type, IconColorScale which, float value)
        {
            throw new NotImplementedException();
        }

        bool IIColorManager.GetIconColorInvert(IconType type)
        {
            throw new NotImplementedException();
        }

        void IIColorManager.SetIconColorInvert(IconType type, bool value)
        {
            throw new NotImplementedException();
        }

        Color IIColorManager.GetDefaultColor(GuiColors id)
        {
            throw new NotImplementedException();
        }

        Color IIColorManager.GetOldUIColorCOLORREF(GuiColors which)
        {
            throw new NotImplementedException();
        }

        void IIColorManager.RepaintUI(RepaintType type)
        {
            throw new NotImplementedException();
        }

        bool IIColorManager.SetIconFolder(string pFolder)
        {
            throw new NotImplementedException();
        }

        void IIColorManager.ReInitIcons()
        {
            throw new NotImplementedException();
        }

        bool IIColorManager.ResolveIconFolder(string pFilename, ref string path)
        {
            throw new NotImplementedException();
        }

        ColorSchemeType IIColorManager.ColorSchemeType { get; set; }

        string IIColorManager.ColorFile { get; }

        AppFrameColorTheme IIColorManager.AppFrameColorTheme { get; set; }

        string IIColorManager.FileName { get; }

        string IIColorManager.IconFolder { get; }

        #endregion
    }
}
