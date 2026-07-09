using System.Collections.Generic;
using Robust.Shared.Utility;

namespace Content.Shared._SV.CharacterDocuments;

/// <summary>
///     Helpers for safely handling the user-authored markup stored in a character document's
///     <c>DocContent</c>.
/// </summary>
public static class CharacterDocumentMarkup
{
    /// <summary>
    ///     Parses <paramref name="content"/> permissively and returns a <see cref="FormattedMessage"/>
    ///     whose tags are guaranteed to be balanced: unmatched closing tags are dropped and any tags
    ///     left open at the end are closed. Safe to feed straight into a rich-text control.
    /// </summary>
    public static FormattedMessage BuildBalancedMessage(string? content)
    {
        FormattedMessage parsed;
        try
        {
            parsed = FormattedMessage.FromMarkupPermissive(content ?? string.Empty);
        }
        catch
        {
            // Even the permissive parser can throw on pathological input. Fall back to rendering
            // the raw content as plain, un-parsed text — never crash on a document.
            var plain = new FormattedMessage();
            plain.AddText(content ?? string.Empty);
            return plain;
        }

        var result = new FormattedMessage();
        // Names of tags currently open, innermost last. Mirrors the FormattedMessage's own
        // internal open-node stack so result.Pop() always closes the matching tag.
        var open = new Stack<string>();

        foreach (var node in parsed)
        {
            if (node.Name == null)
            {
                result.AddText(node.Value.StringValue ?? string.Empty);
                continue;
            }

            if (!node.Closing)
            {
                // Re-add the original opening node verbatim so colours/attributes are preserved.
                result.PushTag(node);
                open.Push(node.Name);
                continue;
            }

            // Only honour a closing tag if it actually closes the innermost open tag. A stray
            // closer (no matching open, or improperly nested) is dropped rather than allowed to
            // underflow the renderer's stack.
            if (open.Count > 0 && open.Peek() == node.Name)
            {
                open.Pop();
                result.Pop();
            }
        }

        // Close anything the user left open, innermost first.
        while (open.Count > 0)
        {
            open.Pop();
            result.Pop();
        }

        return result;
    }

    /// <summary>
    ///     Returns <paramref name="content"/> with its markup tags rebalanced as a markup string.
    ///     Use this on the persistence/print paths where a plain string is required; use
    ///     <see cref="BuildBalancedMessage"/> directly when feeding a rich-text control.
    /// </summary>
    public static string Balance(string? content)
    {
        return BuildBalancedMessage(content).ToMarkup();
    }
}
