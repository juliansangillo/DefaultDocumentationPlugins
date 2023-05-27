using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JulianSangillo.GitBookDefaultDocumentationPlugin.Internals {
    internal sealed partial class TableOfContents {
        private abstract class Template : ITemplate {
            public abstract string TocTmpl { get; }

            public abstract string PageGroupTmpl { get; }

            public abstract string PageTmpl { get; }

            public abstract string ChildPageTmpl { get; }

            public abstract string ChildPageWithoutLinkTmpl { get; }

            protected abstract string Indent { get; }

            public string Fill(TableOfContents model) {
                string result = model.Home == null
                    ? TocTmpl.Remove(
                        TocTmpl.IndexOf("<HomePage>", StringComparison.Ordinal),
                        Regex.Match(TocTmpl, "<HomePage>\n*").Length)
                    : TocTmpl.Replace("<HomePage>", Fill(model.Home));

                return result
                    .Replace("<PageGroups>", Fill(model.PageGroups))
                    .Trim('\n') + '\n';
            }

            private string Fill(IEnumerable<PageGroup> pageGroups) {
                var builder = new StringBuilder();

                builder = pageGroups.Aggregate(builder, (current, pageGroup) =>
                    current
                        .AppendLine(Fill(pageGroup)))
                        .AppendLine();

                return builder.ToString();
            }

            private string Fill(PageGroup pageGroup) =>
                PageGroupTmpl
                    .Replace("<PageGroup.Heading>", pageGroup.Heading)
                    .Replace("<RootPages>", Fill(pageGroup.Pages))
                    .Trim('\n');

            private string Fill(IEnumerable<Page> pages) {
                var builder = new StringBuilder();

                builder = pages.Aggregate(builder, (current, page) =>
                    current
                        .AppendLine(Fill(page)))
                        .AppendLine();

                return builder.ToString();
            }

            private string Fill(Page page) =>
                PageTmpl.Replace("<RootPage>", Fill(page, 0)).Trim('\n');

            private string Fill(Page page, int level) {
                string indents = Enumerable.Repeat(Indent, level)
                    .Aggregate(string.Empty, (current, next) => current + next);

                StringBuilder builder = new StringBuilder()
                    .AppendLine(
                        page.Link != null
                        ? ChildPageTmpl
                            .Replace("<Indents>", indents)
                            .Replace("<Page.Title>", page.Title)
                            .Replace("<Page.Link>", page.Link)
                            .Trim('\n')
                        : ChildPageWithoutLinkTmpl
                            .Replace("<Indents>", indents)
                            .Replace("<Page.Title>", page.Title)
                            .Trim('\n'));

                builder = page.Children
                    .Aggregate(builder, (current, child) =>
                        current.Append(Fill(child, level + 1)));

                return builder.ToString();
            }
        }
    }
}
