namespace JulianSangillo.GitBookDefaultDocumentationPlugin.Internals {
    internal sealed partial class TableOfContents {
        private sealed class MarkdownTemplate : Template {
            public override string TocTmpl => @"
# Table of Contents

<HomePage>

<PageGroups>
";

            public override string PageGroupTmpl => @"
## <PageGroup.Heading>

<RootPages>
";

            public override string PageTmpl => "<RootPage>";

            public override string ChildPageTmpl => "<Indents>* [<Page.Title>](<Page.Link>)";

            public override string ChildPageWithoutLinkTmpl => "<Indents>* <Page.Title>";

            protected override string Indent => "  ";
        }
    }
}
