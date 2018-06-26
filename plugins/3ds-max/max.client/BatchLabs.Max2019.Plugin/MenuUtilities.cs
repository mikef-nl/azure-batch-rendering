
using System;
using System.Collections.Generic;

using Autodesk.Max;

using BatchLabs.Max2019.Plugin.Common;

namespace BatchLabs.Max2019.Plugin
{
    public class MenuUtilities
    {
        private const string MainMenuName = "Azure Batch Rendering";
        private static Dictionary<string, IActionItem> _actionItems;

        /// <summary>
        /// This method setups all possible callbacks to handle menus add/remove in various user actions. For example, 
        /// if the user changes workspaces, the entire menu bar is updated, so this handles adding it in all workspaces as switched.
        /// The drawback is that 3ds Max calls some more than once, so you get some seemingly unnecessary add/remove. But it's safer 
        /// if you always want your menu present. Of course you could also call the add/remove in other conexts and callbacks 
        /// depending on the 3ds max state where you need your menu to display.
        /// </summary>
        public static void SetupMenuHandlers()
        {
            var global = GlobalInterface.Instance;
            _actionItems = new Dictionary<string, IActionItem>();
            Log.Instance.Debug("Setting up main menu handlers");

            // this only needs to be done once
            global.COREInterface.MenuManager.RegisterMenuBarContext(0x58527952, MainMenuName);
            
            // register menu callbacks
            global.RegisterNotification(MenuPostLoadHandler, null, SystemNotificationCode.CuiMenusPostLoad);
            global.RegisterNotification(MenuPreSaveHandler, null, SystemNotificationCode.CuiMenusPreSave);
            global.RegisterNotification(MenuPostSaveHandler, null, SystemNotificationCode.CuiMenusPostSave);

            // this will add it at startup and for some scenerios is enough. But a commercial app should
            // consider above comments about workspace switching.
            global.RegisterNotification(MenuSystemStartupHandler, null, SystemNotificationCode.SystemStartup);
        }

        private static void MenuPostLoadHandler(IntPtr objPtr, INotifyInfo infoPtr)
        {
            Log.Instance.Debug("*** MenuPostLoadHandler ***");
            var global = GlobalInterface.Instance;
            var ip = global.COREInterface13;
            IIMenuManager manager = ip.MenuManager;
            IIMenu menu = manager.FindMenu(MainMenuName);
            if (menu == null)
            {
                InstallMenu();
            }
        }

        private static void MenuPreSaveHandler(IntPtr objPtr, INotifyInfo infoPtr)
        {
            Log.Instance.Debug("*** MenuPreSaveHandler ***");
            var global = GlobalInterface.Instance;
            var ip = global.COREInterface13;
            IIMenuManager manager = ip.MenuManager;
            IIMenu menu = manager.FindMenu(MainMenuName);
            if (menu != null)
            {
                RemoveMenu(MainMenuName);
            }
        }

        private static void MenuPostSaveHandler(IntPtr objPtr, INotifyInfo infoPtr)
        {
            Log.Instance.Debug("*** MenuPostSaveHandler ***");
            var global = GlobalInterface.Instance;
            var ip = global.COREInterface13;
            IIMenuManager manager = ip.MenuManager;
            IIMenu menu = manager.FindMenu(MainMenuName);
            if (menu == null)
            {
                InstallMenu();
            }
        }

        private static void MenuSystemStartupHandler(IntPtr objPtr, INotifyInfo infoPtr)
        {
            Log.Instance.Debug("*** MenuSystemStartupHandler ***");
            var global = GlobalInterface.Instance;
            var ip = global.COREInterface13;
            IIMenuManager manager = ip.MenuManager;
            IIMenu menu = manager.FindMenu(MainMenuName);
            if (menu == null)
            {
                InstallMenu();
            }
        }

        /// <summary>
        /// Note, this method is iterating all action tbales and actions (for this example to also be able to find built-in actions).
        /// If you only need your own, you can use actionManager.FindTable(context);
        /// </summary>
        private static void LookupActions()
        {
            Log.Instance.Debug("Looking up actions");
            var actionManager = GlobalInterface.Instance.COREInterface.ActionManager;
            // TODO: use this instead ... actionManager.FindTable() ... id is 394 in 2018

            for (var actionTableIndex = 0; actionTableIndex < actionManager.NumActionTables; ++actionTableIndex)
            {
                var actionTable = actionManager.GetTable(actionTableIndex);
                if (actionTable.Name.Equals(Loader.ActionCategory, StringComparison.InvariantCultureIgnoreCase))
                {
                    Log.Instance.Debug($"Found plugin action table :: {actionTable.Id_} - {actionTable.Name}");
                    for (var actionIndex = 0; actionIndex < actionTable.Count; ++actionIndex)
                    {
                        var action = actionTable[actionIndex];
                        if (action != null)
                        {
                            Log.Instance.Debug($"ACTION :: {action.DescriptionText}");
                            _actionItems[action.DescriptionText] = action;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Installs the menu from scratch
        /// </summary>
        /// <returns>1 when successfully installed, or 0 in error state</returns>
        private static uint InstallMenu()
        {
            try
            {
                LookupActions();
                Log.Instance.Debug("Installing main menu");

                var global = GlobalInterface.Instance;
                var menuManager = global.COREInterface.MenuManager;

                IIMenu adnSampleMenu = global.IMenu;
                adnSampleMenu.Title = MainMenuName;
                menuManager.RegisterMenu(adnSampleMenu, 0);

                foreach (var actionItem in _actionItems)
                {
                    var menuItem1 = global.IMenuItem;
                    menuItem1.ActionItem = actionItem.Value;
                    adnSampleMenu.AddItem(menuItem1, -1);
                }


                IIMenuItem adnMenu = global.IMenuItem;
                adnMenu.Title = MainMenuName;
                adnMenu.SubMenu = adnSampleMenu;
                menuManager.MainMenuBar.AddItem(adnMenu, -1);
                global.COREInterface.MenuManager.UpdateMenuBar();

            }
            catch (Exception ex)
            {
                Log.Instance.Error($"{ex.Message}\n{ex}", "Failed to add menu items", true);
                return 0;
            }

            return 1;
        }

        /// <summary>
        /// Removes the menu if it's found
        /// </summary>
        /// <param name="menuName"></param>
        private static void RemoveMenu(string menuName)
        {
            Log.Instance.Debug($"removing plugin menu: {menuName}");

            var menuManager = GlobalInterface.Instance.COREInterface.MenuManager;
            var customMenu = menuManager.FindMenu(menuName);
            if (customMenu != null)
            {
                Log.Instance.Debug("found and removing menu");
                menuManager.UnRegisterMenu(customMenu);
                GlobalInterface.Instance.ReleaseIMenu(customMenu);
            }
        }
    }
}
