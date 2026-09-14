namespace Firebot.Infrastructure;

/// <summary>Paths rooted at menusRoot (popups/screens opened over the battle view), grown as needed.</summary>
public static partial class Paths
{
    public static class MenusLoc
    {
        private const string Root = "menusRoot/menuCanvasParent/SafeArea/menuCanvas";

        public static class OracleStoreLoc
        {
            private const string Root = MenusLoc.Root + "/menus/OracleStore";

            public const string CloseBtn = Root + "/closeButton";

            public const string OraclesGiftBtn =
                Root + "/bg/submenus/valueBundles/Scroll View/Viewport/items/oraclesGift";

            public const string OraclesGiftRenewTxt = OraclesGiftBtn + "/Graphics/renewText";
        }
    }
}
