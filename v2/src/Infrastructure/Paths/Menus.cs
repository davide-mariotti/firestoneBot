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

            // Confirmed via a targeted UnityPy scan (Character/bg/submenuButtons/talents), including
            // its own notification bell (text/lock/notification/bell/amountTMP) - see
            // Talents.cs for the full 89-node tree behind this tab.
            public const string TalentsTabBtn = Root + "/bg/submenuButtons/talents";

            public static class QuestsLoc
            {
                private const string Root = CharacterLoc.Root + "/bg/submenus/quests";

                public const string DailyTabBtn = Root + "/bg/submenuButtons/dailyButton";

                public const string WeeklyTabBtn = Root + "/bg/submenuButtons/weeklyButton";

                public const string DailyQuestsGridRoot = Root + "/bg/submenus/dailyQuestsScroll/Viewport/grid";

                public const string WeeklyQuestsGridRoot = Root + "/bg/submenus/weeklyQuestsScroll/Viewport/grid";

                // Shared between both tabs - shows whichever tab's countdown is currently selected
                // (docs/screens/Character.html: "rinnovo delle missioni (giornaliere/settimanali)").
                public const string RenewTxt = Root + "/questsRenewBg/questsRenewText";
            }
        }

        public static class TownGuildLoc
        {
            private const string Root = MenusLoc.Root + "/menus/TownGuild";

            public const string CloseBtn = Root + "/closeButton";

            public const string GuildShopBtn = Root + "/guildShop";

            public const string ExpeditionsBtn = Root + "/expeditions";

            public const string ArcaneCrystalBtn = Root + "/arcaneCrystal";

            public const string TreeOfLifeBtn = Root + "/treeOfLife";

            public const string AwakeningBtn = Root + "/awakening";
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

            public const string TavernBtn = Root + "/townBg/parent/tavern";

            public const string ExoticMerchantBtn = Root + "/townBg/parent/exoticMerchant";

            // Opens WFMenuSelectionLoc, a hub with 2 options (campaign/arena) - confirmed via
            // UnityPy. Matches the wiki's "Arena of Kings [...] is accessible from the Battles
            // building".
            public const string BattlesBtn = Root + "/townBg/parent/battles";

            // Confirmed via UnityPy full child dump of townBg/parent (24 building icons) - missed in
            // the first pass over Hall of Heroes (only grepped the icons already mapped in this file
            // instead of dumping the live list), corrected after the user pointed out live in-game
            // that Hall of Heroes is reached through Town, not just the notification rail.
            public const string HallOfHeroesBtn = Root + "/townBg/parent/hallOfHeroes";
        }

        // The Tavern's own card-flip minigame screen. Its "shop" action button is the real gateway
        // to Scarab's Game (see ScarabGame.cs comment) - confirmed via the wiki
        // (firestone-idle-rpg.fandom.com/wiki/Tavern: "The scarab's game is a part of the Tavern"),
        // corrected after initially assuming Scarab's Game had no permanent manual entry point at all.
        public static class TavernLoc
        {
            private const string Root = MenusLoc.Root + "/menus/Tavern";

            public const string CloseBtn = Root + "/closeButton";

            public const string OpenScarabGameBtn = Root + "/helpCanvas/actionButtons/shop";

            // Opens TavernMarket - "Stormy, the tavern keeper" per the wiki's Tavern Market section.
            public const string OpenMarketBtn = Root + "/helpCanvas/stormyButton";

            // Draws one card, costing game tokens (confirmed via wiki: "Card draws require Game
            // Tokens") - the button's own costText shows the exact amount, read live rather than
            // hardcoded.
            public const string PlayBtn = Root + "/helpCanvas/bottomUI/playButton";

            // Generic pooled currency counter (see path-verification-hierarchy memory) - assumed to
            // show the game token balance while on this screen, since that's this screen's primary
            // spendable resource. Not independently confirmed which currency it's bound to.
            public const string GameTokenCountTxt = Root + "/helpCanvas/counters/counterInteraction/quantity";
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

            // Opens WarMachinesLoc - confirmed via UnityPy. A separate "GarageSelection" hub popup
            // (same 3 destinations: engineer/garage/trainingBase) also exists but wasn't used here,
            // since this button reuses the already-live-confirmed Engineer screen entry point instead
            // of an unconfirmed second route.
            public const string WarMachinesBtn = Root + "/warMachinesButton";
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

            public const string MeteoriteResearchTabBtn = Root + "/submenuButtons/meteoriteResearch";

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
                private const string SubmenuRoot = LibraryLoc.Root + "/submenus/firestoneResearch";

                public const string Root = SubmenuRoot + "/researchScrollView/viewport/content/submenus";

                public const string Glow = "/glow";

                public const string ProgressBar = "/progressBarBg";

                public const string CompletedTxt = "/genericText";

                // 3 trees total, one visible/active at a time - same carousel pattern as
                // MeteoriteResearchLoc below, confirmed against the raw prefab dump (both submenus
                // have their own navigation/goBackTree+goForthTree, not just a single scroll view).
                public const string NextTreeBtn = SubmenuRoot + "/navigation/goForthTree";

                public const string PreviousTreeBtn = SubmenuRoot + "/navigation/goBackTree";
            }

            // Meteorite Research tab: 5 trees x 13 nodes (research0..12), carousel navigation like
            // NodeLoc above. Unlike firestoneResearch nodes (which show level/progress/time inline),
            // these only show an icon + level - clicking one always opens MeteoriteResearchPreviewLoc
            // to see cost/unlock state, confirmed via a fresh UnityPy scan (docs/screens/Library.html
            // didn't capture this popup, same static-analysis gap hit before with Empower's popups).
            public static class MeteoriteResearchLoc
            {
                private const string SubmenuRoot = LibraryLoc.Root + "/submenus/meteoriteResearch";

                private const string NavigationRoot = SubmenuRoot + "/navigation";

                public const string NextTreeBtn = NavigationRoot + "/goForthTree";

                public const string PreviousTreeBtn = NavigationRoot + "/goBackTree";

                // Each tree's children are researchPath0..12 (13 decorative connector lines) FOLLOWED
                // by research0..12 (13 real node buttons) - confirmed via UnityPy child-order dump, so
                // GetChild(13 + index) reaches research{index} for index 0..12.
                public const string TreesRoot = SubmenuRoot + "/submenus";
            }
        }

        // Popup shown when a research node is clicked - lives under menuCanvas/popups, not menus/Library.
        public static class FirestoneResearchPreviewLoc
        {
            private const string Root = MenusLoc.Root + "/popups/FirestoneResearchPreview";

            public const string CloseBtn = Root + "/bg/closeButton";

            // Confirmed via UnityPy. Used to prioritize "Raining Gold" over other unlocked talents,
            // per the user (matches an external tips guide's advice - see FirestoneResearchTask).
            public const string NameTxt = Root + "/bg/innerBg/researchName";

            public const string LevelTxt = Root + "/bg/innerBg/researchLevelText";

            public const string UnlockedTxt = Root + "/bg/innerBg/unlocked";

            public const string MaxedTxt = Root + "/bg/innerBg/maxed";

            public const string RealTimeTxt = UnlockedTxt + "/researchPending/realTime";

            public const string ActivateBtn = UnlockedTxt + "/buttonHolder/researchActivateButton";
        }

        // Popup shown when a meteorite research node is clicked. Found via a fresh UnityPy scan (not
        // in docs/screens/Library.html, which only captured the node grid itself) - no v1 precedent
        // either (v1 never implemented this feature at all). Root path inferred from the same
        // popups/<Name> convention every other popup in this file already uses and that v1's
        // live-tested code confirms for FirestoneResearchPreview/EmpowerPopup/etc - flag for live
        // verification since this specific instance has no direct cross-check.
        public static class MeteoriteResearchPreviewLoc
        {
            private const string Root = MenusLoc.Root + "/popups/MeteoriteResearchPreview";

            public const string CloseBtn = Root + "/bg/closeButton";

            // Confirmed via UnityPy. Used to prioritize "Raining Gold" over other unlocked talents,
            // per the user (matches an external tips guide's advice - see MeteoriteResearchTask).
            public const string NameTxt = Root + "/bg/innerBg/researchName";

            public const string LevelTxt = Root + "/bg/innerBg/level";

            // Shown only when the node's prerequisites are met (sibling to "locked", which shows
            // requirement info instead when they're not) - same on/off pattern as
            // FirestoneResearchPreviewLoc.UnlockedTxt above.
            public const string UnlockedRoot = Root + "/bg/innerBg/unlocked";

            public const string ResearchBtn = UnlockedRoot + "/researchButton";

            public const string CostTxt = ResearchBtn + "/cost";
        }

        public static class OracleStoreLoc
        {
            private const string Root = MenusLoc.Root + "/menus/OracleStore";

            public const string CloseBtn = Root + "/closeButton";

            public const string OraclesGiftBtn =
                Root + "/bg/submenus/valueBundles/Scroll View/Viewport/items/oraclesGift";

            public const string OraclesGiftRenewTxt = OraclesGiftBtn + "/Graphics/renewText";
        }

        // Battle Pass ("Path of Glory"). No v1 precedent at all - v1 never touched this feature.
        // Paths confirmed via a fresh UnityPy scan of the live game assets (docs/screens/BattlePass.html
        // abbreviated some intermediate decorative nodes - "goldenPassBg/.../rewardRoot", the exact
        // "..." was found by dumping a real pathOfGloryTier node directly).
        public static class BattlePassLoc
        {
            private const string Root = MenusLoc.Root + "/menus/BattlePass";

            public const string CloseBtn = Root + "/bg/closeButton";

            // Confirmed active by default on open (docs/screens/BattlePass.html), but clicked
            // unconditionally anyway per the usual "never trust the default tab" rule.
            public const string RewardsTabBtn = Root + "/bg/submenuButtons/rewards";

            public static class RewardsLoc
            {
                public const string TrackRoot =
                    BattlePassLoc.Root + "/bg/submenus/rewards/bg/scrollView/viewport/content/layout";

                // Both relative to a pathOfGloryTier (N) child. Free needs no ownership; Golden
                // requires owning the premium pass (purchased separately via getGoldenPassButton,
                // never auto-clicked) - but claiming an already-unlocked Golden reward isn't itself a
                // purchase, and the button is inactive/non-interactable (safe no-op) when locked or
                // not yet reached, same convention as every other claim button in this codebase.
                public const string FreeClaimBtn = "/freeBg/glowOutlineFree/rewardRoot/claimButton";

                public const string GoldenClaimBtn = "/goldenPassBg/glowOutlineGolden/rewardRoot/claimButton";
            }
        }
    }
}
