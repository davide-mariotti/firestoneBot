namespace Firebot2.Infrastructure;

/// <summary>
///     Split by screen (Battle.cs, Town.cs, ...) instead of one monolithic file like v1 - grown one
///     task at a time, only the paths an actual task uses. Source: docs/index.html + docs/screens/*.html.
/// </summary>
public static partial class Paths
{
    public static class BattleLoc
    {
        private const string Root = "battleRoot/battleMain/battleCanvas/SafeArea";

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
