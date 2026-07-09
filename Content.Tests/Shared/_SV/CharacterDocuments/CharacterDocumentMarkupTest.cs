using System.Collections.Generic;
using Content.Shared._SV.CharacterDocuments;
using NUnit.Framework;
using Robust.Shared.Utility;

namespace Content.Tests.Shared._SV.CharacterDocuments;

/// <summary>
///     Tests for <see cref="CharacterDocumentMarkup"/>, the helper that rebalances the
///     user-authored markup in a character document so it can't crash the rich-text renderer.
///
///     Background: document content is arbitrary user text rendered as markup. The renderer
///     (RobustToolbox <c>RichTextEntry.Update</c>/<c>Draw</c>) seeds the colour/font draw stack
///     with a single default entry and then pops it unconditionally for every closing
///     <c>[/color]</c>/<c>[/font]</c> tag (see <c>ColorTag.PopDrawContext</c>). Two unmatched
///     closing tags therefore pop an empty stack and throw <see cref="System.InvalidOperationException"/>
///     on every measure/draw pass, permanently breaking the document console for that document.
/// </summary>
[Parallelizable]
[TestFixture]
[TestOf(typeof(CharacterDocumentMarkup))]
public sealed class CharacterDocumentMarkupTest
{
    /// <summary>
    ///     Faithfully mirrors the renderer's colour/font stack handling: each stack starts with
    ///     the one default entry the renderer pushes, an opening tag pushes, a closing tag pops,
    ///     and a pop on an empty stack is the <see cref="System.InvalidOperationException"/> that
    ///     crashes the console. Returns true if rendering this message would underflow.
    /// </summary>
    private static bool RenderWouldThrow(FormattedMessage message)
    {
        // Only colour and font use the draw-context stacks that can underflow.
        var depth = new Dictionary<string, int> { ["color"] = 1, ["font"] = 1 };

        foreach (var node in message)
        {
            if (node.Name is not ("color" or "font"))
                continue;

            if (!node.Closing)
            {
                depth[node.Name]++;
                continue;
            }

            if (depth[node.Name] == 0)
                return true; // pop on empty stack -> InvalidOperationException

            depth[node.Name]--;
        }

        return false;
    }

    private static bool RenderWouldThrow(string markup)
        => RenderWouldThrow(FormattedMessage.FromMarkupPermissive(markup));

    [Test]
    public void RawDoubleClosingColorReproducesTheCrash()
    {
        // Sanity-check the harness: the exact content from the bug report must underflow when
        // rendered raw, otherwise the tests below would prove nothing.
        Assert.That(RenderWouldThrow("[/color][/color]"), Is.True);
    }

    [Test]
    [TestCase("[/color][/color]")]
    [TestCase("[/color][/color][/color]")]
    [TestCase("hello [/color][/color] world")]
    [TestCase("[color=red]hi[/color][/color]")]
    [TestCase("[/font][/font]")]
    [TestCase("[color=red]unterminated")]
    [TestCase("[color=red][font]bad nesting[/color][/font]")]
    [TestCase("")]
    [TestCase("plain document text, no tags")]
    [TestCase("[color=#ff0000]valid[/color]")]
    public void BalancedContentNeverUnderflows(string raw)
    {
        var balanced = CharacterDocumentMarkup.BuildBalancedMessage(raw);
        Assert.That(RenderWouldThrow(balanced), Is.False,
            $"Balanced markup still underflows the render stack: '{balanced.ToMarkup()}'");
    }

    [Test]
    public void BalancingIsIdempotent()
    {
        const string raw = "[color=red]hi[/color][/color] [font]x";
        var once = CharacterDocumentMarkup.Balance(raw);
        var twice = CharacterDocumentMarkup.Balance(once);
        Assert.That(twice, Is.EqualTo(once));
    }

    [Test]
    public void WellFormedFormattingTextIsPreserved()
    {
        // Balancing must not corrupt the visible text of already-valid content.
        var msg = CharacterDocumentMarkup.BuildBalancedMessage("[color=red]Red[/color] and [bold]bold[/bold]");
        Assert.That(msg.ToString(), Is.EqualTo("Red and bold"));
    }

    [Test]
    public void MultilineTextIsPreserved()
    {
        // Documents are multi-line; newlines must survive the parse/rebuild round-trip.
        var balanced = CharacterDocumentMarkup.Balance("line one\nline two\nline three");
        Assert.That(FormattedMessage.FromMarkupPermissive(balanced).ToString(),
            Is.EqualTo("line one\nline two\nline three"));
    }

    [Test]
    public void NullContentIsHandled()
    {
        Assert.That(CharacterDocumentMarkup.Balance(null!), Is.Empty);
    }
}
