﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FLEx_ChorusPlugin.Infrastructure;
using FLEx_ChorusPlugin.Infrastructure.DomainServices;

namespace FLEx_ChorusPlugin.Contexts.Scripture
{
	internal static class ArchivedDraftsBoundedContextService
	{
		private const string DraftsFilename = "Drafts." + SharedConstants.ArchivedDraft;

		internal static void NestContext(XElement archivedDraftsProperty,
			string scriptureBaseDir,
			IDictionary<string, SortedDictionary<string, XElement>> classData,
			Dictionary<string, string> guidToClassMapping)
		{
			if (archivedDraftsProperty == null)
				return;
			var drafts = archivedDraftsProperty.Elements().ToList();
			if (!drafts.Any())
				return;

			var root = new XElement(SharedConstants.ArchivedDrafts);
			foreach (var draftObjSur in drafts)
			{
				var draftGuid = draftObjSur.Attribute(SharedConstants.GuidStr).Value.ToLowerInvariant();
				var className = guidToClassMapping[draftGuid];
				var draft = classData[className][draftGuid];

				CmObjectNestingService.NestObject(false, draft,
					classData,
					guidToClassMapping);

				root.Add(draft); // They should still be in the original sorted order, so just add them.
			}
			if (root.HasElements)
				FileWriterService.WriteNestedFile(Path.Combine(scriptureBaseDir, DraftsFilename), root);

			archivedDraftsProperty.RemoveNodes();
		}

		internal static void FlattenContext(
			SortedDictionary<string, XElement> highLevelData,
			SortedDictionary<string, XElement> sortedData,
			string scriptureBaseDir)
		{
			if (!Directory.Exists(scriptureBaseDir))
				return;
			var pathname = Path.Combine(scriptureBaseDir, DraftsFilename);
			if (!File.Exists(pathname))
				return;

			// Owned by Scripture in ArchivedDrafts coll prop.
			var scrElement = highLevelData[SharedConstants.Scripture];
			var scrOwningGuid = scrElement.Attribute(SharedConstants.GuidStr).Value.ToLowerInvariant();
			var sortedDrafts = new SortedDictionary<string, XElement>(StringComparer.OrdinalIgnoreCase);
			var doc = XDocument.Load(pathname);
			foreach (var draftElement in doc.Root.Elements("ScrDraft"))
			{
				CmObjectFlatteningService.FlattenObject(pathname,
					sortedData,
					draftElement,
					scrOwningGuid); // Restore 'ownerguid' to draftElement.
				var draftGuid = draftElement.Attribute(SharedConstants.GuidStr).Value.ToLowerInvariant();
				sortedDrafts.Add(draftGuid, BaseDomainServices.CreateObjSurElement(draftGuid));
			}

			// Restore scrElement ArchivedDrafts property in sorted order.
			if (sortedDrafts.Count == 0)
				return;
			var draftsOwningProp = scrElement.Element(SharedConstants.ArchivedDrafts)
								   ?? CmObjectFlatteningService.AddNewPropertyElement(scrElement, SharedConstants.ArchivedDrafts);
			foreach (var sortedDraft in sortedDrafts.Values)
				draftsOwningProp.Add(sortedDraft);
		}
	}
}