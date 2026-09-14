namespace Firebot.Infrastructure;

/// <summary>World Map screen (menus/WorldMap), its two tabs, related popups, and the mission pins
/// that live on the always-visible background map layer.</summary>
public static partial class Paths
{
    public static class WorldMapLoc
    {
        private const string Root = MenusLoc.Root + "/menus/WorldMap";

        public const string CloseBtn = Root + "/closeButton";

        public const string MapMissionsTabBtn = Root + "/submenuButtons/mapMissionsButton";

        public const string WarfrontCampaignTabBtn = Root + "/submenuButtons/warfrontCampaignButton";

        public static class MapMissionsLoc
        {
            private const string Root =
                WorldMapLoc.Root + "/submenus/mapMissionsSubmenu/bottomLeftUI/missionRefreshCanvas";

            public const string NextRunTimeTxt = Root + "/missionRefreshBg/missionRefreshText";
        }

        public static class WarfrontLoc
        {
            private const string LootBtn = WorldMapLoc.Root + "/submenus/warfrontCampaignSubmenu/loot";

            public const string NextRunTimeTxt = LootBtn + "/nextLootTimeLeft";

            public const string ClaimBtn = LootBtn + "/claimButton";
        }
    }

    // Popup shown when a mission pin is clicked - lives under menuCanvas/popups, not menus/WorldMap.
    public static class PreviewMissionLoc
    {
        private const string Root = MenusLoc.Root + "/popups/PreviewMission";

        public const string CloseBtn = Root + "/bg/closeButton";

        public const string StartBtn = Root + "/bg/managementBg/container/startMissionButton";

        public const string SpeedupBtn = Root + "/bg/managementBg/container/speedUpButton";

        public const string SpeedupFinishDesc = SpeedupBtn + "/finishDesc";

        public const string NotEnoughSquadsTxt = Root + "/bg/managementBg/previewMissionNotEnoughSquads";

        public const string NextRunTimeTxt =
            Root + "/bg/rewardBg/previewMissionTime/previewBar/missionProgress/activeMissionProgressText";
    }

    public static class MissionRewardsLoc
    {
        private const string Root = MenusLoc.Root + "/popups/MissionRewards";

        public const string CloseBtn = Root + "/bg/closeButton";
    }

    public static class MissionPinLoc
    {
        // Lives under menusRoot/mapRoot (the always-visible background map layer), NOT under
        // menuCanvas/menus like every other screen in this file - confirmed against v1's already
        // live-tested path. MenusLoc.Root can't be reused here since it already includes the
        // menuCanvasParent/SafeArea/menuCanvas suffix that this branch doesn't have.
        public const string Root = "menusRoot/mapRoot/mapElements/missions";

        public const string ActiveIcon = "/missionActiveIcon";

        public const string TimeReq = "/missionBg/missionTimeBg/missionTimeReq";

        public const string Tick = "/missionBg/completedTick";
    }
}
