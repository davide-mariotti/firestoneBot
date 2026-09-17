using System;
using System.Collections;
using System.Linq;
using Firebot.Core.Tasks;
using Firebot.GameModel.Base;
using Firebot.GameModel.Features.Town;
using Firebot.GameModel.Primitives;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using Logger = Firebot.Core.Logger;
using TownScreen = Firebot.GameModel.Features.Town.Town;

namespace Firebot.Tasks.Town;

/// <summary>
///     Sub-task requested alongside "Gamer" (see GamerQuestTask): converts passively-accumulated
///     beer into Tavern game tokens whenever the BeerExchange notification badge is up, so there are
///     always enough tokens for the day's 10 card draws. No prior precedent. See
///     Paths.TavernMarketLoc's doc comment for the real (live-confirmed) item structure - only the
///     confirmed beer-priced item/button is ever clicked here.
/// </summary>
public class BeerExchangeTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Quests;
    protected override int MinimumCharacterLevel => 15;

    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(2);

    protected override string NotificationBadgeName => Paths.BattleLoc.NotificationsLoc.BeerExchange;

    public override IEnumerator Execute()
    {
        // Fast path - safe no-op if not up.
        yield return Notifications.BeerExchange;

        // Guaranteed path regardless of the notification - same reasoning as every other task.
        yield return TownScreen.Open;

        // DIAGNOSTIC (2026-09-18, kept active): TavernBtn ("townBg/parent/tavern") doesn't resolve
        // even though every sibling building uses this same pattern successfully - dumping the real
        // building names to find what it's actually called.
        var buildings = new GameElement(Paths.MenusLoc.TownIrongardLoc.BuildingsRoot).GetChildren().ToList();
        Logger.Debug($"[DIAG] Town buildings ({buildings.Count}): " +
                     string.Join(", ", buildings.Select(b => $"'{b.Name}'(visible={b.IsVisible()})")));

        yield return TownScreen.OpenTavern;
        yield return Tavern.OpenMarket;

        // Live-confirmed, 2026-09-18 - see Paths.TavernMarketLoc doc comment: this is the real,
        // uniquely-named beer-priced item/button (not a guess anymore).
        var buyBtn = new GameButton(Paths.TavernMarketLoc.BuyFiveTokensWithBeerBtn);
        while (buyBtn.IsClickable()) yield return buyBtn.Click();

        yield return TavernMarket.Close;
        yield return Tavern.Close;
        yield return TownScreen.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
