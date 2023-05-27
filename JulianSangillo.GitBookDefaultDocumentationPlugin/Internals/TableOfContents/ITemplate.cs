namespace JulianSangillo.GitBookDefaultDocumentationPlugin.Internals {
    internal sealed partial class TableOfContents {
        private interface ITemplate {
            string TocTmpl { get; }

            string PageGroupTmpl { get; }

            string PageTmpl { get; }

            string ChildPageTmpl { get; }

            string ChildPageWithoutLinkTmpl { get; }

            string Fill(TableOfContents model);
        }
    }
}
