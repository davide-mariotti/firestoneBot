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
        }

        public static class RightSideUILoc
        {
            private const string Root = BattleLoc.Root + "/rightSideUI/menuButtons";

            public const string StoreBtn = Root + "/storeButton";

            public const string GuildBtn = Root + "/guildButton";

            public const string TownBtn = Root + "/townButton";
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
