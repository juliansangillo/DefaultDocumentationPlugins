using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using DefaultDocumentation;
using DefaultDocumentation.Api;
using DefaultDocumentation.Models;
using JetBrains.Annotations;

namespace JulianSangillo.GitBookDefaultDocumentationPlugin {
    [PublicAPI]
    public sealed class FolderFileNameFactory : IFileNameFactory {
        private const string InvalidCharReplacementKey = "Markdown.InvalidCharReplacement";

        public string Name => "Folder";

        public void Clean(IGeneralContext context) {
            context.Settings.Logger.Debug($"Cleaning output folder \"{context.Settings.OutputDirectory}\"");

            var deletedItems = CleanOutputDirectory(context.Settings.OutputDirectory).ToList();

            int u = 3;
            while (deletedItems.Any(f => f.Exists) && u-- > 0)
                Thread.Sleep(1000);
        }

        public string GetFileName(IGeneralContext context, DocItem item) =>
            PathCleaner.Clean(
                item is AssemblyDocItem
                    ? Path.Combine(item.Name, item.FullName == "index" ? "AssemblyInfo" : item.FullName)
                    : Path.Combine(
                        item.GetParents()
                            .Where(docItem => !(docItem is AssemblyDocItem))
                            .Select(docItem => docItem.Name)
                            .Concat(Enumerable.Repeat(item.Name, 1))
                            .Concat(Enumerable.Repeat("README", 1))
                            .ToArray()),
                GetInvalidCharReplacement(context)) + ".md";

        private static string GetInvalidCharReplacement(IContext context) =>
            context.GetSetting<string>(InvalidCharReplacementKey);

        private static IEnumerable<FileSystemInfo> CleanOutputDirectory(DirectoryInfo outputDirectory) {
            if (!outputDirectory.Exists)
                yield break;

            IEnumerable<DirectoryInfo> directories = outputDirectory.EnumerateDirectories();
            foreach (DirectoryInfo directory in directories)
                foreach (FileSystemInfo results in CleanDirectory(directory))
                    yield return results;

            IEnumerable<FileInfo> files = outputDirectory
                .EnumerateFiles("*.md", SearchOption.TopDirectoryOnly)
                .Where(f => !string.Equals(
                    f.Name,
                    "readme.md",
                    StringComparison.OrdinalIgnoreCase));
            foreach (FileInfo file in files)
                yield return Delete(file);
        }

        private static IEnumerable<FileSystemInfo> CleanDirectory(DirectoryInfo directory) {
            IEnumerable<DirectoryInfo> subDirectories = directory.EnumerateDirectories();
            foreach (DirectoryInfo subDirectory in subDirectories)
                foreach (FileSystemInfo results in CleanDirectory(subDirectory))
                    yield return results;

            IEnumerable<FileInfo> files = directory.EnumerateFiles("*.md", SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in files)
                yield return Delete(file);

            if (!directory.GetFileSystemInfos().Any())
                yield return Delete(directory);
        }

        private static FileSystemInfo Delete(FileSystemInfo item) {
            for (int i = 3; i > 0; i--)
                try {
                    item.Delete();
                    break;
                } catch {
                    Thread.Sleep(100);
                    if (i == 1)
                        throw;
                }

            return item;
        }

        private static class PathCleaner {
            private static readonly string[] ToTrimChars = new[] { '=', ' ' }.Select(c => $"{c}").ToArray();
            private static readonly string[] InvalidChars = new[] { '\"', '<', '>', ':', '*', '?' }
                .Concat(Path.GetInvalidPathChars())
                .Select(c => $"{c}")
                .ToArray();

            public static string Clean(string value, string invalidCharReplacement) {
                value = ToTrimChars.Aggregate(value, (current, toTrimChar) =>
                    current.Replace(toTrimChar, string.Empty));

                invalidCharReplacement = string.IsNullOrEmpty(invalidCharReplacement) ? "_" : invalidCharReplacement;

                value = InvalidChars.Aggregate(value, (current, invalidChar) =>
                    current.Replace(invalidChar, invalidCharReplacement));

                return value.Trim(Path.PathSeparator);
            }
        }
    }
}
