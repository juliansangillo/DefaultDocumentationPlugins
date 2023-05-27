using System.Collections.Generic;
using System.Linq;

namespace JulianSangillo.GitBookDefaultDocumentationPlugin.Internals {
    internal sealed partial class TableOfContents {
        private sealed class Page {
            public Page(string title, string link) {
                Title = title;
                Link = link;
                Children = Enumerable.Empty<Page>();
            }

            public Page(string title, string link, IEnumerable<Page> children) {
                Title = title;
                Link = link;
                Children = children;
            }

            public string Title { get; }

            public string Link { get; }

            public IEnumerable<Page> Children { get; }
        }
    }
}
