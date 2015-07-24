﻿namespace Microsoft.DocAsCode
{
    using System;
    using System.IO;
    using System.Linq;

    using Microsoft.DocAsCode.EntityModel;
    using Microsoft.DocAsCode.Utility;

    class PackSubCommand : ISubCommand
    {
        public ParseResult Exec(Options options)
        {
            var outputFile = Path.Combine(options.PackVerb.OutputFolder ?? Environment.CurrentDirectory, options.PackVerb.Name ?? "externalreference.rpk");
            try
            {
                var baseUri = new Uri(options.PackVerb.BaseUrl);
                if (!baseUri.IsAbsoluteUri)
                {
                    return new ParseResult(ResultLevel.Error, "BaseUrl should be absolute url.");
                }
                var source = options.PackVerb.Source.TrimEnd('/', '\\');
                using (var package = options.PackVerb.AppendMode ? ExternalReferencePackageWriter.Append(outputFile, baseUri) : ExternalReferencePackageWriter.Create(outputFile, baseUri))
                {
                    var files = GlobPathHelper.GetFiles(source, options.PackVerb.Glob).ToList();
                    if (options.PackVerb.FlatMode)
                    {
                        package.AddFiles(string.Empty, files);
                    }
                    else
                    {
                        foreach (var g in from f in files
                                          group f by Path.GetDirectoryName(f) into g
                                          select new
                                          {
                                              Folder = g.Key.Substring(source.Length).Replace('\\', '/').Trim('/'),
                                              Files = g.ToList(),
                                          })
                        {
                            if (g.Folder.Length == 0)
                            {
                                package.AddFiles(string.Empty, g.Files);
                            }
                            else
                            {
                                package.AddFiles(g.Folder + "/", g.Files);
                            }
                        }
                    }
                }
                return ParseResult.SuccessResult;
            }
            catch (Exception ex)
            {
                return new ParseResult(ResultLevel.Error, ex.ToString());
            }
        }
    }
}