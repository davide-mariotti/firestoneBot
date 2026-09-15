using System;
using System.Collections;
using Firebot.Core.Tasks;
using Firebot.GameModel.Features.Town;
using Firebot.GameModel.Primitives;
using Firebot.GameModel.Shared;
using Firebot.Infrastructure;
using TownScreen = Firebot.GameModel.Features.Town.Town;

namespace Firebot.Tasks.Town;

/// <summary>
///     Sub-task requested alongside "Gamer" (see GamerQuestTask): converts passively-accumulated
///     beer into Tavern game tokens whenever the BeerExchange notification badge is up, so there are
///     always enough tokens for the day's 10 card draws. No prior precedent.
///     IMPORTANT: see Paths.TavernMarketLoc's doc comment - which of the two purchase buttons on the
///     5-token offer is beer vs. gems could not be confirmed via UnityPy. Only the assumed-beer
///     button is ever clicked here.
/// </summary>
public class BeerExchangeTask : BotTask
{
    internal override TaskGroup Group => TaskGroup.Quests;
    protected override int MinimumCharacterLevel => 15;

    private static readonly TimeSpan RecheckDelay = TimeSpan.FromHours(2);

    protected override string NotificationPath => Paths.BattleLoc.NotificationsLoc.BeerExchangeBtn;

    public override IEnumerator Execute()
    {
        // Fast path - safe no-op if not up.
        yield return Notifications.BeerExchange;

        // Guaranteed path regardless of the notification - same reasoning as every other task.
        yield return TownScreen.Open;
        yield return TownScreen.OpenTavern;
        yield return Tavern.OpenMarket;

        var buyBtn = new GameButton(Paths.TavernMarketLoc.BuyFiveTokensWithBeerBtn);
        while (buyBtn.IsClickable()) yield return buyBtn.Click();

        yield return TavernMarket.Close;
        yield return Tavern.Close;
        yield return TownScreen.Close;

        NextRunTime = DateTime.Now + RecheckDelay;
    }
}
