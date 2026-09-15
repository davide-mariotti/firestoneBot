using System.Collections;
using System.Linq;
using Firebot.GameModel.Base;
using Firebot.GameModel.Primitives;
using Firebot.Infrastructure;

namespace Firebot.GameModel.Features.Town.Library.MeteoriteResearch;

public class MeteoriteNode : GameElement
{
    // researchPath0..12 (13 decorative connector lines) precede research0..12 (13 real buttons) as
    // siblings under each tree - confirmed via UnityPy child-order dump, no prior precedent (this
    // feature was never automated before).
    private const int PathLineCount = 13;

    public MeteoriteNode() : base(Paths.MenusLoc.LibraryLoc.MeteoriteResearchLoc.TreesRoot) { }

    private GameElement GetTree() => GetChildren().First(tree => tree.IsVisible());

    public IEnumerator Select(int index)
    {
        var tree = GetTree();
        var child = tree.GetChild(PathLineCount + index);

        if (!child.IsVisible())
            yield break;

        yield return new GameButton(parent: child).Click();
    }

    public IEnumerator NextTree =>
        new GameButton(Paths.MenusLoc.LibraryLoc.MeteoriteResearchLoc.NextTreeBtn).Click();

    public IEnumerator PreviousTree =>
        new GameButton(Paths.MenusLoc.LibraryLoc.MeteoriteResearchLoc.PreviousTreeBtn).Click();
}
