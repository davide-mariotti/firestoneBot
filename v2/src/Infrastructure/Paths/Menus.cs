namespace Firebot.Infrastructure;

/// <summary>Paths rooted at menusRoot (popups/screens opened over the battle view), grown as needed.</summary>
public static partial class Paths
{
    public static class MenusLoc
    {
        private const string Root = "menusRoot/menuCanvasParent/SafeArea/menuCanvas";

        public static class StoreLoc
        {
            private const string Root = MenusLoc.Root + "/menus/Store";

            public const string CloseBtn = Root + "/closeButton";

            public static class TabsLoc
            {
                private const string Root = StoreLoc.Root + "/submenuButtons/grid";

                public const string DailyRewardsBtn = Root + "/dailyRewardsButton";

                public const string ValueBundleDailyBtn = Root + "/valueBundleDailyButton";
            }

            public static class DailyRewardsLoc
            {
                private const string Root = StoreLoc.Root + "/bg/submenus/dailyRewards/Viewport/transparentFrame";

                public const string CheckInBtn = Root + "/checkIn";

                public const string NextRunTimeTxt = Root + "/textHolder/timer";
            }

            public static class ValueBundleDailyLoc
            {
                private const string Root = StoreLoc.Root + "/bg/submenus/valueBundleDaily";

                // The one free slot in this tab (has a "freeText" label instead of a price) - NOT
                // the numbered valueBundle (0)/(1)/(2) slots next to it, those are real-money/premium
                // purchases and must never be auto-clicked.
                public const string FreeMysteryBoxBtn =
                    Root + "/Scroll View/Viewport/bundles/mysteryBox/Graphics/purchaseButton";

                public const string RenewTxt = Root + "/timeRenewBackground/renewText";
            }
        }

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
