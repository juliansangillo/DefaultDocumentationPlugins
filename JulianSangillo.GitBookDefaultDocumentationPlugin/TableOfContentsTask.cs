using System;
using System.IO;
using JetBrains.Annotations;
using JulianSangillo.GitBookDefaultDocumentationPlugin.Internals;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace JulianSangillo.GitBookDefaultDocumentationPlugin {
    [PublicAPI]
    public sealed class TableOfContentsTask : Task {
        [Required]
        public string OutputPath { get; set; }

        public override bool Execute() {
            try {
                TableOfContents.Generate(new DirectoryInfo(OutputPath));
                return true;
            } catch (Exception e) {
                Log.LogErrorFromException(e, true);
                return false;
            }
        }
    }
}
