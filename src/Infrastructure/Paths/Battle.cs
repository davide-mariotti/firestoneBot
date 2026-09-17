namespace Firebot.Infrastructure;

/// <summary>
///     Split by screen (Battle.cs, Town.cs, ...) instead of one monolithic file - grown one task at a
///     time, only the paths an actual task uses. Source: docs/index.html + docs/screens/*.html.
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

        // Direct children of bottomLeftSideUI itself, not the notifications/Viewport/grid badge list
        // below (NotificationsLoc) - same general HUD region, different sub-path.
        public static class BottomLeftSideUILoc
        {
            private const string Root = BattleLoc.Root + "/bottomLeftSideUI";

            // Corrected against a live in-game runtime dump the user captured (docs/simple-path/
            // simple-path.txt): "bottomLeftSideUI/mail" is the real live path, not "leftSideUINew/mail" -
            // both "leftSideUI" and "leftSideUINew" exist as near-identical prefab variants in the
            // static assets (same ambiguity as BottomSideUIDesktopLoc vs BottomSideUINewLoc below),
            // and the earlier "leftSideUINew" pick for this whole HUD region turned out to be the
            // wrong one for the currently-installed game version - see NotificationsLoc just below.
            public const string MailBtn = Root + "/mail";
        }

        public static class NotificationsLoc
        {
            // Corrected to "leftSideUI" (not "leftSideUINew") against a live in-game runtime dump the
            // user captured (docs/simple-path/simple-path.txt) - confirms every notification badge
            // (Quests, Engineer, FreePickaxes, etc.) actually lives under leftSideUI right now. Both
            // variants exist as near-identical prefabs in the static assets (same "New" naming
            // ambiguity flagged for bottomSideUINew vs bottomSideUIDesktop below) - static analysis
            // alone can't tell which one the game actually instantiates, only a live capture can, and
            // this one had been guessed wrong.
            private const string Root = BattleLoc.Root + "/leftSideUI/notifications/Viewport/grid";

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

            // Unlike every other entry above, this one has no prior precedent to cross-check against -
            // the original TempleOfEternalsTask never used a notification at all, only the manual chain below.
            // Sourced only from the static doc scan (docs/path.firestone.html, "TemplePrestige...
            // probabile alias di Temple of Eternals") - lowest-trust tier per the path-verification
            // convention. Flag for live verification before relying on it.
            public const string TemplePrestigeBtn = Root + "/TemplePrestige";

            // Same situation as TemplePrestigeBtn above - never implemented Meteorite Research at
            // all (docs/path.firestone.html explicitly notes this badge as "Rimossa dal bot, feature
            // mai raggiunta"), so there's no live-tested precedent. Flag for live verification.
            public const string MeteoriteResearchBtn = Root + "/MeteoriteResearch";

            // Confirmed present (unlike TemplePrestige/MeteoriteResearch above, these were directly
            // verified via UnityPy, not just the static doc scan) - but still no prior precedent, since
            // never implemented Scarab Game at all.
            public const string ScarabGameBtn = Root + "/ScarabGame";

            public const string ScarabGameShopFreeTokenBtn = Root + "/ScarabGameShopFreeToken";

            // Sourced from the static doc scan only (docs/path.firestone.html), not independently
            // verified via UnityPy - unlike ScarabGame above. No prior precedent either.
            public const string ArcaneCrystalBtn = Root + "/ArcaneCrystal";

            // User-suggested, confirmed present via UnityPy (like ScarabGame above). No prior precedent.
            // Fires when accumulated beer can be exchanged for Tavern Market game tokens.
            public const string BeerExchangeBtn = Root + "/BeerExchange";

            // Confirmed present via UnityPy (like ScarabGame/BeerExchange above). No prior precedent.
            // Fires whenever an unspent talent point is available (Character screen, Talents tab).
            public const string TalentAvailableBtn = Root + "/TalentAvailable";

            // Confirmed present via UnityPy (like ScarabGame/BeerExchange above). No prior precedent.
            // Presumed to fire when an Arena of Kings battle token is available - opportunistic fast
            // path only, not used as NotificationPath (same reasoning as the daily-quest tasks: a
            // periodic recheck is simple/reliable enough, no need to lean on an unconfirmed badge for
            // scheduling priority).
            public const string ArenaTokensBtn = Root + "/ArenaTokens";

            // Confirmed present via UnityPy, same grid as every entry above. No prior precedent. Used as
            // an opportunistic fast path only (see HallOfHeroesGearTask) - the real entry point is
            // the Town building icon (TownIrongardLoc.HallOfHeroesBtn), same as most other features.
            public const string HallOfHeroesBtn = Root + "/HallOfHeroes";
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
        // active:true in the static prefab dump, but only BottomSideUINewLoc has a proven precedent
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

            // Opens the battle formation editor ("Party" screen) - confirmed present via UnityPy,
            // same menuButtons row as InventoryBtn above (not independently confirmed live, but high
            // confidence given InventoryBtn's confirmation covers this exact HUD region).
            public const string PartyBtn = Root + "/menuButtons/partyButtonUI";
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
