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

        // Live children dump (SystemMailTask diagnostic, 2026-09-17, see git history) of every
        // direct child of SafeArea proved the previous "bottomLeftSideUI/mail" guess wrong in that
        // session: "leftSideUINew" had 6 children - menuButtons, dps, mail, chat, chatLastMsgBg,
        // notifications - while "bottomLeftSideUI" existed but was empty. BUT a same-timing retest
        // right after found the exact opposite (leftSideUINew/mail missing again) - this looks like
        // the game instantiates one of two alternate HUD prefab sets (matches the "New" vs "Desktop"
        // ambiguity already flagged elsewhere in this file for other regions), likely picked by
        // screen resolution/aspect ratio, not a single fixed layout. SystemMail.Open tries this one
        // first, then falls back to BottomLeftSideUILoc below - whichever one the game actually
        // instantiated this session, one of the two should resolve.
        public static class LeftSideUINewLoc
        {
            // Public, not private: shared with the sibling NotificationsLoc class below (both are
            // rooted under this same real parent).
            public const string Root = BattleLoc.Root + "/leftSideUINew";

            public const string MailBtn = Root + "/mail";
        }

        // The "normal" (non-"New") HUD variant's equivalent of LeftSideUINewLoc above - fallback
        // target when that one doesn't resolve. Never independently confirmed as populated in any
        // live session yet (always saw either leftSideUINew populated, or this one empty) - kept as
        // the fallback on the strength of the resolution/variant-switch theory, not a live
        // confirmation of its own content.
        public static class BottomLeftSideUILoc
        {
            private const string Root = BattleLoc.Root + "/bottomLeftSideUI";

            public const string MailBtn = Root + "/mail";
        }

        public static class NotificationsLoc
        {
            // Same two-variant situation as LeftSideUINewLoc/BottomLeftSideUILoc above, for the same
            // reason (this whole grid lives right under leftSideUINew, next to mail) - a previous
            // single-root "correction" to leftSideUI (or, before that, this "New" root) always
            // eventually turned out wrong in some session (same mistake pattern as TownIrongard's
            // popups/ detour, see PLAN.md). Every "quick access" notification check in every task
            // used to silently fail and fall through to the guaranteed navigation path - never broke
            // anything (that path is a safe no-op fast path), just never actually fired. Exposed as
            // two roots + bare badge names (not precomputed full paths) so both NotificationBtn(name)
            // below and Notifications.cs can build and try both candidates.
            public const string Root = LeftSideUINewLoc.Root + "/notifications/Viewport/grid";

            public const string FallbackRoot = BattleLoc.Root + "/leftSideUI/notifications/Viewport/grid";

            public const string OraclesGift = "OraclesGift";

            public const string CheckIn = "CheckIn";

            public const string MysteryBox = "MysteryBox";

            public const string Quests = "Quests";

            public const string FreePickaxes = "FreePickaxes";

            public const string Engineer = "Engineer";

            public const string Expeditions = "Expeditions";

            public const string GuardianTraining = "GuardianTraining";

            public const string OracleRituals = "OracleRituals";

            public const string Experiments = "Experiments";

            public const string WarfrontCampaign = "WarfrontCampaign";

            public const string MapMissions = "MapMissions";

            public const string FirestoneResearch = "FirestoneResearch";

            // Unlike every other entry above, this one has no prior precedent to cross-check against -
            // the original TempleOfEternalsTask never used a notification at all, only the manual chain below.
            // Sourced only from the static doc scan (docs/path.firestone.html, "TemplePrestige...
            // probabile alias di Temple of Eternals") - lowest-trust tier per the path-verification
            // convention. Flag for live verification before relying on it.
            public const string TemplePrestige = "TemplePrestige";

            // Same situation as TemplePrestige above - never implemented Meteorite Research at
            // all (docs/path.firestone.html explicitly notes this badge as "Rimossa dal bot, feature
            // mai raggiunta"), so there's no live-tested precedent. Flag for live verification.
            public const string MeteoriteResearch = "MeteoriteResearch";

            // Confirmed present (unlike TemplePrestige/MeteoriteResearch above, these were directly
            // verified via UnityPy, not just the static doc scan) - but still no prior precedent, since
            // never implemented Scarab Game at all.
            public const string ScarabGame = "ScarabGame";

            public const string ScarabGameShopFreeToken = "ScarabGameShopFreeToken";

            // Sourced from the static doc scan only (docs/path.firestone.html), not independently
            // verified via UnityPy - unlike ScarabGame above. No prior precedent either.
            public const string ArcaneCrystal = "ArcaneCrystal";

            // User-suggested, confirmed present via UnityPy (like ScarabGame above). No prior precedent.
            // Fires when accumulated beer can be exchanged for Tavern Market game tokens.
            public const string BeerExchange = "BeerExchange";

            // Confirmed present via UnityPy (like ScarabGame/BeerExchange above). No prior precedent.
            // Fires whenever an unspent talent point is available (Character screen, Talents tab).
            public const string TalentAvailable = "TalentAvailable";

            // Confirmed present via UnityPy (like ScarabGame/BeerExchange above). No prior precedent.
            // Presumed to fire when an Arena of Kings battle token is available - opportunistic fast
            // path only, not used as NotificationPath (same reasoning as the daily-quest tasks: a
            // periodic recheck is simple/reliable enough, no need to lean on an unconfirmed badge for
            // scheduling priority).
            public const string ArenaTokens = "ArenaTokens";

            // Confirmed present via UnityPy, same grid as every entry above. No prior precedent. Used as
            // an opportunistic fast path only (see HallOfHeroesGearTask) - the real entry point is
            // the Town building icon (TownIrongardLoc.HallOfHeroesBtn), same as most other features.
            public const string HallOfHeroes = "HallOfHeroes";

            // DIAGNOSTIC (2026-09-18): best-guess name, same lowest-trust tier as TemplePrestige/
            // MeteoriteResearch above - not independently confirmed via UnityPy or live, since
            // docs/screens/PirateShip.html only dumped the screen itself (its own in-screen tab badge,
            // "bg/submenuButtons/piratesPrize/button/text/notification", is a DIFFERENT thing from
            // this shared HUD rail). Opportunistic fast path only - the real entry point is the Town
            // building icon (TownIrongardLoc.PirateShipBtn), same as most other features.
            public const string PiratesPrize = "PiratesPrize";
        }

        public static class RightSideUILoc
        {
            private const string Root = BattleLoc.Root + "/rightSideUI/menuButtons";

            public const string StoreBtn = Root + "/storeButton";

            public const string GuildBtn = Root + "/guildButton";

            public const string TownBtn = Root + "/townButton";

            public const string MapBtn = Root + "/mapButton";

            // A THIRD, independent live-confirmed location for Path of Glory (2026-09-17 recursive
            // search, same session where neither BottomSideUIMobileLoc nor BottomSideUIDesktopLoc
            // below was active) - this one was activeSelf=True/activeInHierarchy=True right then,
            // alongside Store/Guild/Town/Map above. The bottom-bar HUD variant switches dynamically
            // within a session (not fixed once at launch like the notification rail), so rather than
            // picking one "correct" root, every known location is tried in turn - see UiVariantButton.
            public const string PathOfGloryBtn = Root + "/pathOfGloryButton";

            public const string PathOfGloryNotification = PathOfGloryBtn + "/notification";
        }

        // A third, independent live-confirmed location for Inventory/Party (2026-09-17 recursive
        // search) - active (activeSelf=True/activeInHierarchy=True) in the same session where
        // neither BottomSideUIMobileLoc nor BottomSideUIDesktopLoc below was. PartyBtn mirrors
        // InventoryBtn's confirmed menuButtons row but isn't itself independently confirmed live.
        public static class BottomRightSideUINewLoc
        {
            private const string Root = BattleLoc.Root + "/bottomRightSideUINew/menuButtons";

            public const string InventoryBtn = Root + "/inventoryButtonUI";

            public const string PartyBtn = Root + "/partyButtonUI";
        }

        public static class StageProgressionLoc
        {
            private const string Root = BattleLoc.Root + "/topSideUI/stageProgression";

            public const string CurrentStageNumTxt = Root + "/currentStage/stageNum";

            public const string GoBackBtn = Root + "/goBackStage";
        }

        // A DIFFERENT battle-screen HUD region from BottomSideUINewLoc below - both show
        // active:true in the static prefab dump, but only BottomSideUINewLoc has a proven precedent
        // (Hero Upgrade, live-tested for months). Live diagnostic dump (PathOfGloryTask,
        // 2026-09-17, see git history) found this one populated with only leaderPanel (combat HUD)
        // and menuButtons/upgradesButtonUI in that session - no pathOfGloryButton, no
        // inventoryButtonUI, no partyButtonUI anywhere under it. Same two-variant-HUD situation as
        // LeftSideUINewLoc/NotificationsLoc, this time on the bottom bar: BottomSideUIMobileLoc
        // below had the real content that session (offersLayout/pathOfGloryButton,
        // menuButtons/inventoryButtonUI, menuButtons/partyButtonUI). Kept as the fallback target -
        // never independently confirmed populated itself, but plausible for an actual desktop-
        // resolution client per the resolution-dependent theory.
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

        // The "Mobile" HUD variant - live-confirmed (PathOfGloryTask diagnostic, 2026-09-17) as the
        // one actually populated in that session: menuButtons (partyButtonUI, fellowshipButtonUI,
        // inventoryButtonUI, upgradesButtonUI), leaderPanel (combat HUD), offersLayout
        // (pathOfGloryButton, eventsButton, starterPackButton, specialOffersButton). Tried first
        // (primary, not fallback) since this is the one with actual live confirmation, unlike
        // BottomSideUIDesktopLoc above.
        public static class BottomSideUIMobileLoc
        {
            private const string Root = BattleLoc.Root + "/bottomSideUIMobile";

            public const string PathOfGloryBtn = Root + "/offersLayout/pathOfGloryButton";

            public const string PathOfGloryNotification = PathOfGloryBtn + "/notification";

            public const string InventoryBtn = Root + "/menuButtons/inventoryButtonUI";

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
