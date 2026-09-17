using System.Collections;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Shared;

public static class SystemMail
{
    public static IEnumerator Open => new GameButton(Paths.BattleLoc.BottomLeftSideUILoc.MailBtn).Click();

    public static GameElement MailList => new(Paths.SystemMailLoc.MailListRoot);

    public static GameButton ClaimBtn => new(Paths.SystemMailLoc.ClaimBtn);

    public static IEnumerator Close => new GameButton(Paths.SystemMailLoc.CloseBtn).Click();
}
