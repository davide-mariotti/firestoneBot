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

        public static class CharacterLoc
        {
            private const string Root = MenusLoc.Root + "/menus/Character";

            // Root path convention not independently live-verified for this screen (new in v2, no
            // v1 code to cross-check against) - inferred from every other menu screen checked so far
            // (Store, OracleStore) consistently using menusRoot/.../menus/<Name>.
            public const string CloseBtn = Root + "/bg/closeButton";

            public const string QuestsTabBtn = Root + "/bg/submenuButtons/quests";

            public static class QuestsLoc
            {
                private const string Root = CharacterLoc.Root + "/bg/submenus/quests";

                public const string DailyTabBtn = Root + "/bg/submenuButtons/dailyButton";

                public const string DailyQuestsGridRoot = Root + "/bg/submenus/dailyQuestsScroll/Viewport/grid";

                public const string RenewTxt = Root + "/questsRenewBg/questsRenewText";
            }
        }

        public static class TownGuildLoc
        {
            private const string Root = MenusLoc.Root + "/menus/TownGuild";

            public const string CloseBtn = Root + "/closeButton";

            public const string GuildShopBtn = Root + "/guildShop";
        }

        public static class GuildShopLoc
        {
            private const string Root = MenusLoc.Root + "/menus/GuildShop";

            public const string CloseBtn = Root + "/closeButton";

            public const string SuppliesTabBtn = Root + "/bg/submenuButtons/supplies/button";

            public static class FreePickaxeLoc
            {
                private const string Root = GuildShopLoc.Root + "/bg/submenus/supplies/items/freePickaxe";

                public const string ClaimBtn = Root;

                public const string QuantityTxt = Root + "/claimBg/itemBg/itemQuantity";

                public const string NextRunTimeTxt = Root + "/nextFreeObj/progressBarBg/timeLeftText";
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
