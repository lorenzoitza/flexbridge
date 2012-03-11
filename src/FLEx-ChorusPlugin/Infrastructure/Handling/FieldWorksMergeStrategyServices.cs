﻿using System;
using System.Collections.Generic;
using Chorus.merge;
using Chorus.merge.xml.generic;

namespace FLEx_ChorusPlugin.Infrastructure.Handling
{
	/// <summary>
	/// Services class used by FieldWorksMergingStrategy to create ElementStrategy instances
	/// (some shared and some not shared).
	/// </summary>
	internal static class FieldWorksMergeStrategyServices
	{
		private static readonly FindByKeyAttribute WsKey = new FindByKeyAttribute(SharedConstants.Ws);
		private static readonly FindByKeyAttribute GuidKey = new FindByKeyAttribute(SharedConstants.GuidStr);
		private static readonly FindFirstElementWithSameName SameName = new FindFirstElementWithSameName();
		private static readonly FieldWorkObjectContextGenerator ContextGen = new FieldWorkObjectContextGenerator();
		private const string MutableSingleton = "MutableSingleton";
		private const string ImmutableSingleton = "ImmutableSingleton";

		internal static XmlMerger CreateXmlMergerForFieldWorksData(MergeSituation mergeSituation, MetadataCache mdc)
		{
			var merger = new XmlMerger(mergeSituation);
			BootstrapSystem(mdc, merger);
			return merger;
		}

		/// <summary>
		/// Bootstrap a merger for the new-styled (nested) files.
		/// </summary>
		/// <remarks>
		/// 1. A generic 'header' element will be handled, although it may not appear in the file.
		/// 2. All classes  will be included.
		/// 3. Merge strategies for class properties (regular or custom) will have keys of "classname+propname" to make them unique, system-wide.
		/// </remarks>
		private static void BootstrapSystem(MetadataCache metadataCache, XmlMerger merger)
		{
			merger.MergeStrategies.KeyFinder = new FieldWorksKeyFinder();

			var sharedElementStrategies = new Dictionary<string, ElementStrategy>();
			CreateSharedElementStrategies(sharedElementStrategies);

			var strategiesForMerger = merger.MergeStrategies;
			ContextGen.MergeStrategies = strategiesForMerger;

			foreach (var sharedKvp in sharedElementStrategies)
				strategiesForMerger.SetStrategy(sharedKvp.Key, sharedKvp.Value);

			var headerStrategy = CreateSingletonElementType(false);
			headerStrategy.ContextDescriptorGenerator = ContextGen;
			strategiesForMerger.SetStrategy(SharedConstants.Header, headerStrategy);

			// There are two abstract class names used: CmAnnotation and DsChart.
			// Chorus knows how to find the matching element for these, as they use <CmAnnotation class='concreteClassname'.
			// So, add two keyed strategies for each of them.
			var keyedStrat = ElementStrategy.CreateForKeyedElement(SharedConstants.GuidStr, false);
			keyedStrat.AttributesToIgnoreForMerging.Add(SharedConstants.Class);
			keyedStrat.AttributesToIgnoreForMerging.Add(SharedConstants.GuidStr);
			strategiesForMerger.SetStrategy(SharedConstants.CmAnnotation, keyedStrat);

			keyedStrat = ElementStrategy.CreateForKeyedElement(SharedConstants.GuidStr, false);
			keyedStrat.AttributesToIgnoreForMerging.Add(SharedConstants.Class);
			keyedStrat.AttributesToIgnoreForMerging.Add(SharedConstants.GuidStr);
			strategiesForMerger.SetStrategy(SharedConstants.DsChart, keyedStrat);

			// The lint file has a collection of odd stuff.
			keyedStrat = ElementStrategy.CreateForKeyedElement(SharedConstants.GuidStr, false);
			keyedStrat.AttributesToIgnoreForMerging.Add(SharedConstants.Class);
			keyedStrat.AttributesToIgnoreForMerging.Add(SharedConstants.GuidStr);
			keyedStrat.AttributesToIgnoreForMerging.Add("curiositytype");
			keyedStrat.AttributesToIgnoreForMerging.Add("tempownerguid");
			strategiesForMerger.SetStrategy(SharedConstants.curiosity, keyedStrat);

			foreach (var classInfo in metadataCache.AllConcreteClasses)
			{
				var classStrat = MakeClassStrategy(ContextGen);
				// ScrDraft instances can only be added or removed, but not changed, according to John Wickberg (18 Jan 2012).
				classStrat.IsImmutable = classInfo.ClassName == "ScrDraft";
				// Didn't work, since the paras are actually in an 'ownseq' element.
				// So, use a new ownseatomic element tag.
				// classStrat.IsAtomic = classInfo.ClassName == "StTxtPara" || classInfo.ClassName == "ScrTxtPara";

				switch (classInfo.ClassName)
				{
					case "LexEntry":
						strategiesForMerger.SetStrategy(classInfo.ClassName, MakeClassStrategy(new LexEntryContextGenerator()));
						break;
					case "WfiWordform":
						strategiesForMerger.SetStrategy(classInfo.ClassName, MakeClassStrategy(new WfiWordformContextGenerator()));
						break;
					case "Text":
						strategiesForMerger.SetStrategy(classInfo.ClassName, MakeClassStrategy(new TextContextGenerator()));
						break;
					case "RnGenericRec":
						strategiesForMerger.SetStrategy(classInfo.ClassName, MakeClassStrategy(new RnGenericRecContextGenerator()));
						break;
					case "ScrBook":
						strategiesForMerger.SetStrategy(classInfo.ClassName, MakeClassStrategy(new ScrBookContextGenerator()));
						break;
					case "ScrSection":
						strategiesForMerger.SetStrategy(classInfo.ClassName, MakeClassStrategy(new ScrSectionContextGenerator()));
						break;
					case "CmPossibilityList":
						strategiesForMerger.SetStrategy(classInfo.ClassName, MakeClassStrategy(new PossibilityListContextGenerator()));
						break;
						// These should be all the subclasses of CmPossiblity. It's unfortuate to have to list them here;
						// OTOH, if we ever want special handling for any of them, we can easily add a special generator.
						// Note that these will not usually be found as strategies, since they are owned in owning sequences
						// and ownseq has its own item. However, they can be found by the default object context generator code,
						// which has a special case for ownseq.
					case "MoMorphType":
					case "PartOfSpeech":
					case "ChkTerm":
					case "PhPhonRuleFeat":
					case "CmCustomItem":
					case "CmLocation":
					case "CmAnnotationDefn":
					case "CmPerson":
					case "CmAnthroItem":
					case "CmSemanticDomain":
					case "LexEntryType":
					case "LexRefType":
					case "CmPossibility":
						strategiesForMerger.SetStrategy(classInfo.ClassName, MakeClassStrategy(new PossibilityContextGenerator()));
						break;
					case "PhEnvironment":
						strategiesForMerger.SetStrategy(classInfo.ClassName, MakeClassStrategy(new EnvironmentContextGenerator()));
						break;
					case "DsConstChart":
					case "ConstChartRow":
					case "ConstChartWordGroup":
						strategiesForMerger.SetStrategy(classInfo.ClassName, MakeClassStrategy(new DiscourseChartContextGenerator()));
						break;
					case "PhNCSegments":
						strategiesForMerger.SetStrategy(classInfo.ClassName, MakeClassStrategy(new MultiLingualStringsContextGenerator("Natural Class", "Name", "Abbreviation")));
						break;
					case "FsClosedFeature":
						strategiesForMerger.SetStrategy(classInfo.ClassName, MakeClassStrategy(new MultiLingualStringsContextGenerator("Phonological Features", "Name", "Abbreviation")));
						break;
					default:
						strategiesForMerger.SetStrategy(classInfo.ClassName, classStrat);
						break;
				}
				foreach (var propertyInfo in classInfo.AllProperties)
				{
					var isCustom = propertyInfo.IsCustomProperty;
					var propStrategy = isCustom
										? CreateStrategyForKeyedElement(SharedConstants.Name, false)
										: CreateSingletonElementStrategy();
					switch (propertyInfo.DataType)
					{
						//default:
						//	break;
						// Not for DataType.TextPropBinary (yet anyway), becasue its contained <Prop> element is atomic.
						case DataType.GenDate:
							if (classInfo.ClassName == "CmPerson" || classInfo.ClassName == "RnGenericRec")
								propStrategy.IsImmutable = true; // Surely DateOfBirth, DateOfDeath, and DateOfEvent are fixed. onced they happen. :-)
							break;
						case DataType.Guid:
							if (classInfo.ClassName == "CmFilter" || classInfo.ClassName == "CmResource")
								propStrategy.IsImmutable = true;
							break;
						case DataType.Binary:
							propStrategy.IsAtomic = true;
							break;
						case DataType.Time:
							propStrategy.IsImmutable = true; // Immutable, because some of them are immutable leagally (date created), and we have pre-merged the rest to be so.
							break;
					}
					strategiesForMerger.SetStrategy(String.Format("{0}{1}_{2}", isCustom ? "Custom_" : "", classInfo.ClassName, propertyInfo.PropertyName), propStrategy);
				}
			}
		}

		private static ElementStrategy MakeClassStrategy(IGenerateContextDescriptor descriptor)
		{
			var classStrat = new ElementStrategy(false)
								{
									MergePartnerFinder = GuidKey,
									ContextDescriptorGenerator = descriptor,
									IsAtomic = false
								};
			if (ContextGen != null && descriptor is FieldWorkObjectContextGenerator)
				((FieldWorkObjectContextGenerator) descriptor).MergeStrategies = ContextGen.MergeStrategies;
			return classStrat;
		}

		private static ElementStrategy CreateSingletonElementStrategy()
		{
			var result = ElementStrategy.CreateSingletonElement();
			result.ContextDescriptorGenerator = ContextGen;
			return result;
		}

		private static void AddSharedImmutableSingletonElementType(Dictionary<string, ElementStrategy> sharedElementStrategies, string name, bool orderOfTheseIsRelevant)
		{
			var strategy = AddSharedSingletonElementType(sharedElementStrategies, name, orderOfTheseIsRelevant);
			strategy.IsImmutable = true;
		}

		private static ElementStrategy AddSharedSingletonElementType(Dictionary<string, ElementStrategy> sharedElementStrategies, string name, bool orderOfTheseIsRelevant)
		{
			var strategy = CreateSingletonElementType(orderOfTheseIsRelevant);
			sharedElementStrategies.Add(name, strategy);
			return strategy;
		}

		private static ElementStrategy CreateSingletonElementType(bool orderOfTheseIsRelevant)
		{
			var strategy = new ElementStrategy(orderOfTheseIsRelevant)
							{
								MergePartnerFinder = SameName,
								ContextDescriptorGenerator = ContextGen
							};
			return strategy;
		}

		private static void CreateSharedElementStrategies(Dictionary<string, ElementStrategy> sharedElementStrategies)
		{
			// Set up immutable strategies.
			// Skip this one, since all DateTime props are, either legally (created), or are pre-processed for merging.
			//AddSharedImmutableSingletonElementType(sharedElementStrategies, DateCreated, false);
			AddSharedImmutableSingletonElementType(sharedElementStrategies, ImmutableSingleton, false);

			AddSharedSingletonElementType(sharedElementStrategies, MutableSingleton, false);

			var elementStrategy = AddSharedSingletonElementType(sharedElementStrategies, SharedConstants.Str, false);
			elementStrategy.IsAtomic = true; // TsStrings are atomic

			elementStrategy = AddSharedSingletonElementType(sharedElementStrategies, SharedConstants.Binary, false);
			elementStrategy.IsAtomic = true; // Binary properties are atomic

			elementStrategy = AddSharedSingletonElementType(sharedElementStrategies, SharedConstants.Prop, false);
			elementStrategy.IsAtomic = true; // Prop is atomic

			AddSharedSingletonElementType(sharedElementStrategies, SharedConstants.Uni, false);
			AddSharedKeyedByWsElementType(sharedElementStrategies, SharedConstants.AStr, false, true); // final parm is for IsAtomic, which in this case is atomic.
			AddSharedKeyedByWsElementType(sharedElementStrategies, SharedConstants.AUni, false, false); // final parm is for IsAtomic, which in this case is not atomic.

			// Add element for "ownseq"
			elementStrategy = CreateStrategyForKeyedElement(SharedConstants.GuidStr, true);
			elementStrategy.AttributesToIgnoreForMerging.AddRange(new[] { SharedConstants.GuidStr, SharedConstants.Class });
			sharedElementStrategies.Add(SharedConstants.Ownseq, elementStrategy);

			// Add element for "ownseqatomic" // Atomic here means the whole elment is treated as effectively as if it were binary data.
			elementStrategy = CreateStrategyForKeyedElement(SharedConstants.GuidStr, true);
			elementStrategy.AttributesToIgnoreForMerging.AddRange(new[] { SharedConstants.GuidStr, SharedConstants.Class });
			elementStrategy.IsAtomic = true;
			sharedElementStrategies.Add(SharedConstants.OwnseqAtomic, elementStrategy);

			// Add element for SharedConstants.Objsur.
			// This is only good now for ref atomic.
			// No. atomic ref prop can't have multiples, so there is no need for a keyed lookup. CreateStrategyForKeyedElement(SharedConstants.GuidStr, false);
			elementStrategy = AddSharedSingletonElementType(sharedElementStrategies, SharedConstants.Objsur, false);
			elementStrategy.IsAtomic = true; // Testing to see if atomic here, or at the prop level is better, as per https://www.pivotaltracker.com/story/show/25402673

			// Add element for SharedConstants.Refseq
			elementStrategy = CreateStrategyForElementKeyedByGuidInList(); // JohnT's new Chorus widget that handles potentially repeating element guids for ref seq props.
			elementStrategy.AttributesToIgnoreForMerging.Add("t");
			sharedElementStrategies.Add(SharedConstants.Refseq, elementStrategy);

			// Add element for SharedConstants.Refcol
			// Order is not important in any kind of collection property, since they are mathematical sets with no ordering and no repeats.
			elementStrategy = CreateStrategyForKeyedElement(SharedConstants.GuidStr, false);
			elementStrategy.AttributesToIgnoreForMerging.Add("t");
			sharedElementStrategies.Add(SharedConstants.Refcol, elementStrategy);
		}

		private static ElementStrategy CreateStrategyForElementKeyedByGuidInList()
		{
			var result = ElementStrategy.CreateForKeyedElementInList(SharedConstants.GuidStr);
			result.ContextDescriptorGenerator = ContextGen;
			return result;
		}

		private static ElementStrategy CreateStrategyForKeyedElement(string guid, bool orderIsRelevant)
		{
			var result = ElementStrategy.CreateForKeyedElement(guid, orderIsRelevant);
			result.ContextDescriptorGenerator = ContextGen;
			return result;
		}

		private static void AddSharedKeyedByWsElementType(IDictionary<string, ElementStrategy> sharedElementStrategies, string elementName, bool orderOfTheseIsRelevant, bool isAtomic)
		{
			AddKeyedElementType(sharedElementStrategies, elementName, WsKey, orderOfTheseIsRelevant, isAtomic);
		}

		private static void AddKeyedElementType(IDictionary<string, ElementStrategy> sharedElementStrategies, string elementName, IFindNodeToMerge findBykeyAttribute, bool orderOfTheseIsRelevant, bool isAtomic)
		{
			var strategy = new ElementStrategy(orderOfTheseIsRelevant)
							{
								MergePartnerFinder = findBykeyAttribute,
								ContextDescriptorGenerator = ContextGen,
								IsAtomic = isAtomic
							};
			sharedElementStrategies.Add(elementName, strategy);
		}
	}
}