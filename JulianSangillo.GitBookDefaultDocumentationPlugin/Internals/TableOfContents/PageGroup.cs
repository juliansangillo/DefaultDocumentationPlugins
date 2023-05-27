using System.Collections.Generic;

namespace JulianSangillo.GitBookDefaultDocumentationPlugin.Internals {
    internal sealed partial class TableOfContents {
        private sealed class PageGroup {
            public PageGroup(string heading, IEnumerable<Page> pages) {
                Heading = heading;
                Pages = pages;
            }

            public string Heading { get; }

            public IEnumerable<Page> Pages { get; }
        }
    }
}
