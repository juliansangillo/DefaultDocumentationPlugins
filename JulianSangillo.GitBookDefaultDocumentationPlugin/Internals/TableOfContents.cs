using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace JulianSangillo.GitBookDefaultDocumentationPlugin.Internals {
    internal sealed partial class TableOfContents {
        private static readonly IDictionary<string, string> SpecialPageGroupMap = new Dictionary<string, string> {
            { "api", "API Documentation" },
        };

        private TableOfContents(Page homePage, IEnumerable<PageGroup> pageGroups) {
            Home = homePage;
            PageGroups = pageGroups;
        }

        private Page Home { get; }

        private IEnumerable<PageGroup> PageGroups { get; }

        public static void Generate(DirectoryInfo outputPath) {
            if (!outputPath.EnumerateDirectories().Any())
                return;

            Page homePage = null;
            if (outputPath.EnumerateFiles("README.md").FirstOrDefault() is FileInfo file)
                homePage = new Page(GetMarkdownHeading(file), GetRelativeFile(outputPath, file));

            IEnumerable<DirectoryInfo> subDirectories = outputPath
                .EnumerateDirectories()
                .Where(d => d.EnumerateFiles("*.md", SearchOption.AllDirectories).Any());
            var toc = new TableOfContents(homePage, GetPageGroups(subDirectories, outputPath));

            var outputFile = new FileInfo($"{outputPath}{Path.DirectorySeparatorChar}SUMMARY.md");
            using (StreamWriter stream = outputFile.CreateText())
                stream.Write(toc.GenerateMd());
        }

        private static IEnumerable<PageGroup> GetPageGroups(
            IEnumerable<DirectoryInfo> directories,
            DirectoryInfo outputPath) =>
            directories.Select(d => GetPageGroup(d, outputPath));

        private static PageGroup GetPageGroup(DirectoryInfo directory, DirectoryInfo outputPath) =>
            SpecialPageGroupMap.TryGetValue(directory.Name, out string value)
                ? new PageGroup(
                    value,
                    GetPages(directory.EnumerateDirectories(), outputPath))
                : new PageGroup(
                    CultureInfo.InvariantCulture.TextInfo.ToTitleCase(
                        directory.Name
                            .Replace("-", " ")
                            .Replace("_", " ")
                            .ToLower(CultureInfo.InvariantCulture)),
                    GetPages(directory.EnumerateDirectories(), outputPath));

        private static IEnumerable<Page> GetPages(IEnumerable<DirectoryInfo> directories, DirectoryInfo outputPath) =>
            directories.Select(d => GetPage(d, outputPath));

        private static Page GetPage(DirectoryInfo directory, DirectoryInfo outputPath) {
            FileInfo readme = directory.EnumerateFiles("README.md").FirstOrDefault();
            IEnumerable<Page> childPages = GetPages(directory.EnumerateDirectories(), outputPath)
                .Concat(
                    directory.EnumerateFiles()
                        .Where(f => f.Name != "README.md")
                        .Select(f => GetPage(f, outputPath)));

            return new Page(GetMarkdownHeading(readme), GetRelativeFile(outputPath, readme), childPages);
        }

        private static Page GetPage(FileInfo file, DirectoryInfo outputPath) =>
            new Page(GetMarkdownHeading(file), GetRelativeFile(outputPath, file));

        private static string GetMarkdownHeading(FileInfo file) {
            string text;
            using (StreamReader stream = file.OpenText())
                text = stream.ReadToEnd();

            for (int i = 1; i <= 6; i++) {
                string hLevel = Enumerable.Repeat('#', i).Aggregate(
                    string.Empty,
                    (current, next) => current + next);

                string match = text.Split('\n')
                    .FirstOrDefault(l => l.StartsWith($"{hLevel} ", StringComparison.InvariantCulture));

                if (match != null)
                    return match.Trim('#').Trim(' ');
            }

            throw new InvalidOperationException();
        }

        private static string GetRelativeFile(DirectoryInfo relativeTo, FileInfo path) =>
            path?.ToString().Remove(0, relativeTo.ToString().Length).Trim(Path.DirectorySeparatorChar);

        private string GenerateMd() => Generate(new MarkdownTemplate());

        private string Generate(ITemplate template) => template.Fill(this);
    }
}
