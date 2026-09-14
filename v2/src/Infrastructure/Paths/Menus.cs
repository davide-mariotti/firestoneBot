namespace Firebot.Infrastructure;

/// <summary>Paths rooted at menusRoot (popups/screens opened over the battle view), grown as needed.</summary>
public static partial class Paths
{
    public static class MenusLoc
    {
        internal const string Root = "menusRoot/menuCanvasParent/SafeArea/menuCanvas";

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

            public const string ExpeditionsBtn = Root + "/expeditions";
        }

        // Unlike the "menus" screens above, Expeditions lives under menuCanvas/popups - same root
        // convention as v1's PopupsLoc (event-triggered overlay rather than a permanent hub screen).
        public static class ExpeditionsLoc
        {
            private const string Root = MenusLoc.Root + "/popups/Expeditions";

            public const string CloseBtn = Root + "/bg/closeButton";

            public const string NextRunTimeTxt = Root + "/bg/timeLeftBg/timeLeftText";

            private const string ActiveExpeditionRoot =
                Root + "/bg/expeditionsParent/activeExpeditionParent/activeExpedition";

            public const string ActiveExpedition = ActiveExpeditionRoot;

            public const string ClaimBtn = ActiveExpeditionRoot + "/claimButton";

            public const string CurrentRunTimeTxt = ActiveExpeditionRoot + "/expeditionProgressBg/timeLeftText";

            public const string StartBtn =
                Root +
                "/bg/expeditionsParent/pendingExpeditionsParent/expeditionsScroll/Viewport/grid/expeditionPending0/startButton";
        }

        public static class TownIrongardLoc
        {
            private const string Root = MenusLoc.Root + "/menus/TownIrongard";

            public const string CloseBtn = Root + "/closeButton";

            public const string EngineerBtn = Root + "/townBg/parent/engineer";

            public const string MagicQuartersBtn = Root + "/townBg/parent/magicQuarters";

            public const string OracleBtn = Root + "/townBg/parent/oracle";

            public const string AlchemistBtn = Root + "/townBg/parent/alchemist";

            public const string LibraryBtn = Root + "/townBg/parent/library";

            public const string TempleOfEternalsBtn = Root + "/townBg/parent/templeOfEternals";
        }

        public static class TempleOfEternalsLoc
        {
            private const string Root = MenusLoc.Root + "/menus/TempleOfEternals";

            private const string PrestigeSubmenuRoot = Root + "/submenus/bgNew/prestigeSubmenu";

            public const string CloseBtn = Root + "/closeButton";

            public const string EmpowerBtn = PrestigeSubmenuRoot + "/adventureInfo/openEmpowerButton";

            public const string AdventureTimePlayedTxt = PrestigeSubmenuRoot + "/adventureInfo/adventureTimePlayed";

            public const string FirestonesFoundTxt = PrestigeSubmenuRoot + "/adventureInfo/firestonesFound";

            public const string FirestonesYouOwnTxt =
                PrestigeSubmenuRoot + "/progress/firestonesYouOwnBg/firestonesYouOwn";
        }

        // Confirmation popup opened by TempleOfEternalsLoc.EmpowerBtn.
        public static class EmpowerPopupLoc
        {
            private const string Root = MenusLoc.Root + "/popups/EmpowerPopup";

            public const string CloseBtn = Root + "/bg/closeButton";

            public const string EmpowerBtn = Root + "/bg/empowerBg/empowerButton";
        }

        // Generic "this costs something, confirm?" gate - only used by the Empower flow so far.
        public static class ActionRequiredLoc
        {
            private const string Root = MenusLoc.Root + "/popups/ActionRequired";

            public const string ConfirmBtn = Root + "/bg/confirmButton";

            public const string CancelBtn = Root + "/bg/cancelButton";
        }

        // "Temple Of Eternals Prestige Complete" - dismissal popup shown right after a successful empower.
        public static class TOEPrestigeCompleteLoc
        {
            private const string Root = MenusLoc.Root + "/popups/TOEPrestigeComplete";

            public const string ConfirmBtn = Root + "/bg/confirmButton";
        }

        public static class EngineerLoc
        {
            private const string Root = MenusLoc.Root + "/menus/Engineer";

            public const string CloseBtn = Root + "/closeButton";

            public const string ClaimBtn = Root + "/submenus/bg/engineerSubmenu/toolsProductionSection/claimToolsButton";

            public const string NextRunTimeTxt = ClaimBtn + "/cooldownOn/cooldownTimeLeft";
        }

        public static class MagicQuartersLoc
        {
            private const string Root = MenusLoc.Root + "/menus/MagicQuarters";

            public const string CloseBtn = Root + "/closeButton";

            public const string GuardiansRoot = Root + "/guardianList";

            // Relative to a guardian child - GuardianLoc below.
            public const string GuardianStarsIcon = "/starsParent";

            private const string UnlockedGuardianRoot = Root + "/submenus/bg/infoSubmenu/activities/unlocked";

            public const string EnlightenmentBtn = UnlockedGuardianRoot + "/enlightenment/enlightenmentButton";

            public const string TrainBtn = UnlockedGuardianRoot + "/train/trainButton";

            public const string NextRunTimeTxt = TrainBtn + "/cooldownOn/cooldownTimeLeft";
        }

        // Overlay shown if a still-locked guardian is clicked - separate popup, own close button.
        public static class LockedGuardianLoc
        {
            private const string Root = MenusLoc.Root + "/popups/LockedGuardian";

            public const string CloseBtn = Root + "/bg/closeButton";
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

        // The "Oracle" building/screen (rituals) - distinct from OracleStoreLoc below (the value-bundle
        // shop opened via the OraclesGift notification, Task 2).
        public static class OracleLoc
        {
            private const string Root = MenusLoc.Root + "/menus/Oracle";

            public const string CloseBtn = Root + "/closeButton";

            public static class RitualLoc
            {
                private const string Root = OracleLoc.Root + "/submenus/bg/ritualSubmenu";

                public const string Rituals = Root + "/ritualsGrid";

                public const string ClaimBtn = "/claimButton";

                public const string CurrentRunTimeTxt = "/ritualProgressBg/timeLeftText";

                public const string StartBtn = "/startButton";

                public const string NextRunTimeTxt = Root + "/timeBg/timeLeft";
            }
        }

        public static class AlchemistLoc
        {
            private const string Root = MenusLoc.Root + "/menus/Alchemist";

            public const string CloseBtn = Root + "/closeButton";

            public static class ExperimentsLoc
            {
                public const string Root = AlchemistLoc.Root + "/submenus/bg/experimentsSubmenu/experiments";

                public const string StartBtn = "/startExperiment";

                public const string ClaimBtn = "/claimButton";

                public const string NextRunTimeTxt = "/progressBarBg/timeLeftText";

                public const string SpeedupBtn = "/speedUpButton";

                public const string SpeedupFinishDesc = SpeedupBtn + "/finishDesc";
            }
        }

        public static class LibraryLoc
        {
            private const string Root = MenusLoc.Root + "/menus/Library";

            public const string CloseBtn = Root + "/closeButton";

            // Library has two tabs (meteoriteResearch, firestoneResearch) and firestoneResearch is
            // NOT selected by default when the screen opens (docs/screens/Library.html line 95) -
            // must be clicked explicitly, same as every other multi-tab screen in this codebase.
            public const string FirestoneResearchTabBtn = Root + "/submenuButtons/firestoneResearch";

            public static class ResearchPanelLoc
            {
                public const string Root = LibraryLoc.Root + "/submenus/firestoneResearch/researchPanel";

                public const string SelectResearchTable = Root + "/selectResearchTable";

                public const string UnlockSlotBtn = Root + "/unlockResearchSlot/confirmButton";

                public const string ClaimBtn = "/container/claimButton";

                public const string NextRunTimeTxt = "/container/researchInfo/progressBarBg/timeLeftText";

                public const string SpeedupBtn = "/container/speedUpButton";

                public const string SpeedupFinishDesc = SpeedupBtn + "/finishDesc";
            }

            public static class NodeLoc
            {
                public const string Root =
                    LibraryLoc.Root + "/submenus/firestoneResearch/researchScrollView/viewport/content/submenus";

                public const string Glow = "/glow";

                public const string ProgressBar = "/progressBarBg";

                public const string CompletedTxt = "/genericText";
            }
        }

        // Popup shown when a research node is clicked - lives under menuCanvas/popups, not menus/Library.
        public static class FirestoneResearchPreviewLoc
        {
            private const string Root = MenusLoc.Root + "/popups/FirestoneResearchPreview";

            public const string CloseBtn = Root + "/bg/closeButton";

            public const string LevelTxt = Root + "/bg/innerBg/researchLevelText";

            public const string UnlockedTxt = Root + "/bg/innerBg/unlocked";

            public const string MaxedTxt = Root + "/bg/innerBg/maxed";

            public const string RealTimeTxt = UnlockedTxt + "/researchPending/realTime";

            public const string ActivateBtn = UnlockedTxt + "/buttonHolder/researchActivateButton";
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
