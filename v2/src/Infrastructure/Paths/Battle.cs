namespace Firebot.Infrastructure;

/// <summary>
///     Split by screen (Battle.cs, Town.cs, ...) instead of one monolithic file like v1 - grown one
///     task at a time, only the paths an actual task uses. Source: docs/index.html + docs/screens/*.html.
/// </summary>
public static partial class Paths
{
    public static class BattleLoc
    {
        private const string Root = "battleRoot/battleMain/battleCanvas/SafeArea";

        public static class PlayerAvatarLoc
        {
            // The avatar itself is the button that opens the Character screen.
            public const string OpenBtn = BattleLoc.Root + "/topLeftSideUI/playerAvatar";

            public const string CharacterLevel = OpenBtn + "/characterLevelBg/characterLevel";
        }

        public static class NotificationsLoc
        {
            // "leftSideUINew", not "leftSideUI" - confirmed against v1's already-live-tested path.
            // The static doc scan (docs/path.firestone.html) picked the wrong one of two near-
            // identical prefab variants here, same ambiguity flagged there for bottomSideUINew vs
            // bottomSideUIDesktop - worth fixing in the docs too, but v1's proven path wins here.
            private const string Root = BattleLoc.Root + "/leftSideUINew/notifications/Viewport/grid";

            public const string OraclesGiftBtn = Root + "/OraclesGift";

            public const string CheckInBtn = Root + "/CheckIn";

            public const string MysteryBoxBtn = Root + "/MysteryBox";

            public const string QuestsBtn = Root + "/Quests";

            public const string FreePickaxesBtn = Root + "/FreePickaxes";

            public const string EngineerBtn = Root + "/Engineer";

            public const string ExpeditionsBtn = Root + "/Expeditions";

            public const string GuardianTrainingBtn = Root + "/GuardianTraining";

            public const string OracleRitualsBtn = Root + "/OracleRituals";

            public const string ExperimentsBtn = Root + "/Experiments";

            public const string WarfrontCampaignBtn = Root + "/WarfrontCampaign";

            public const string MapMissionsBtn = Root + "/MapMissions";

            public const string FirestoneResearchBtn = Root + "/FirestoneResearch";

            // Unlike every other entry above, this one has NO v1 precedent to cross-check against -
            // v1's TempleOfEternalsTask never used a notification at all, only the manual chain below.
            // Sourced only from the static doc scan (docs/path.firestone.html, "TemplePrestige...
            // probabile alias di Temple of Eternals") - lowest-trust tier per the path-verification
            // convention. Flag for live verification before relying on it.
            public const string TemplePrestigeBtn = Root + "/TemplePrestige";

            // Same situation as TemplePrestigeBtn above - v1 never implemented Meteorite Research at
            // all (docs/path.firestone.html explicitly notes this badge as "Rimossa dal bot, feature
            // mai raggiunta"), so there's no live-tested precedent. Flag for live verification.
            public const string MeteoriteResearchBtn = Root + "/MeteoriteResearch";

            // Confirmed present (unlike TemplePrestige/MeteoriteResearch above, these were directly
            // verified via UnityPy, not just the static doc scan) - but still no v1 precedent, since
            // v1 never implemented Scarab Game at all.
            public const string ScarabGameBtn = Root + "/ScarabGame";

            public const string ScarabGameShopFreeTokenBtn = Root + "/ScarabGameShopFreeToken";

            // Sourced from the static doc scan only (docs/path.firestone.html), not independently
            // verified via UnityPy - unlike ScarabGame above. No v1 precedent either.
            public const string ArcaneCrystalBtn = Root + "/ArcaneCrystal";

            // User-suggested, confirmed present via UnityPy (like ScarabGame above). No v1 precedent.
            // Fires when accumulated beer can be exchanged for Tavern Market game tokens.
            public const string BeerExchangeBtn = Root + "/BeerExchange";
        }

        public static class RightSideUILoc
        {
            private const string Root = BattleLoc.Root + "/rightSideUI/menuButtons";

            public const string StoreBtn = Root + "/storeButton";

            public const string GuildBtn = Root + "/guildButton";

            public const string TownBtn = Root + "/townButton";

            public const string MapBtn = Root + "/mapButton";
        }

        public static class StageProgressionLoc
        {
            private const string Root = BattleLoc.Root + "/topSideUI/stageProgression";

            public const string CurrentStageNumTxt = Root + "/currentStage/stageNum";

            public const string GoBackBtn = Root + "/goBackStage";
        }

        // A DIFFERENT battle-screen HUD region from BottomSideUINewLoc below - both show
        // active:true in the static prefab dump, but only BottomSideUINewLoc has a v1 precedent
        // (Hero Upgrade, live-tested for months). This one ("Desktop" - likely a platform-specific
        // layout variant) has never been touched by any proven code, so its exact on-screen
        // visibility is unverified. Sourced from a fresh UnityPy scan, not just the docs (which
        // already flagged this exact ambiguity in path.firestone.html without resolving it).
        public static class BottomSideUIDesktopLoc
        {
            private const string Root = BattleLoc.Root + "/bottomSideUIDesktop";

            public const string PathOfGloryBtn = Root + "/pathOfGloryButton";

            // Sub-element on the button itself, not a separate leftSideUINew rail entry - Battle
            // Pass has no badge on that rail at all (confirmed against the full 48-badge notification
            // list in docs/path.firestone.html). The button doubles as its own notification.
            public const string PathOfGloryNotification = PathOfGloryBtn + "/notification";

            // Confirmed by the user directly (independent of the ambiguity note above) as the real
            // way to open the bag/Inventory screen - a second independent confirmation that this HUD
            // region is actually live, alongside pathOfGloryButton.
            public const string InventoryBtn = Root + "/menuButtons/inventoryButtonUI";
        }

        public static class BottomSideUINewLoc
        {
            private const string Root = BattleLoc.Root + "/bottomSideUINew/bgBlack";

            public static class LeaderPanelLoc
            {
                private const string Root = BottomSideUINewLoc.Root + "/LeaderPanel/leaderPanelNew";

                public const string LvlUpBtn = Root + "/lvlUpButtonBig";
            }

            public static class HeroSlotsLoc
            {
                public const string Root = BottomSideUINewLoc.Root + "/layout";

                public const string LvlUpBtn = "/lvlUpButtonBig";
            }

            public static class ChangeLevelUpModeLoc
            {
                public const string Button = BottomSideUINewLoc.Root + "/rightSide/changeLevelUpModeButton";

                public const string Text = Button + "/text";
            }
        }
    }
}
