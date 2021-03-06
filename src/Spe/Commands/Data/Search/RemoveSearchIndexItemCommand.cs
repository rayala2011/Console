﻿using System.Linq;
using System.Management.Automation;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Maintenance;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data.Items;
using Spe.Abstractions.VersionDecoupling.Interfaces;
using Spe.Core.Extensions;
using Spe.Core.VersionDecoupling;

namespace Spe.Commands.Data.Search
{
    [Cmdlet(VerbsCommon.Remove, "SearchIndexItem", DefaultParameterSetName = "Name")]
    public class RemoveSearchIndexItem : BaseIndexCommand
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Name")]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Item")]
        public Item Item { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "SearchResultItem")]
        public SearchResultItem SearchResultItem { get; set; }

        [Parameter]
        public SwitchParameter AsJob { get; set; }

        protected override void ProcessRecord()
        {
            if (Item != null)
            {
                var itemDatabase = Item.Database.Name;
                var itemPath = Item.Paths.Path;
                var indexableId = new SitecoreIndexableItem(Item).Id;

                foreach (var index in WildcardFilter(Name, ContentSearchManager.Indexes, index => index.Name))
                {
                    if (!index.Crawlers.Any(c => c is SitecoreItemCrawler && ((SitecoreItemCrawler)c).Database.Is(itemDatabase))) continue;

                    DeleteItem(index, indexableId, itemPath);
                }
            }
            else if (SearchResultItem != null)
            {
                var itemPath = SearchResultItem.Path;
                var indexableId = (SitecoreItemId)SearchResultItem.ItemId;
                var indexname = SearchResultItem.Fields["_indexname"].ToString();

                foreach (var index in WildcardFilter(indexname, ContentSearchManager.Indexes, index => index.Name))
                {
                    DeleteItem(index, indexableId, itemPath);
                }
            }
        }


        private void DeleteItem(ISearchIndex index, IIndexableId indexableId, string itemPath)
        {
            WriteVerbose($"Removing item {itemPath} from index {index.Name}.");
            var indexManager = TypeResolver.ResolveFromCache<IIndexManager>();
            var job = indexManager.DeleteItem(index, indexableId);

            if (job == null || !AsJob) return;

            WriteVerbose($"Background job created: {job.Name}");
            WriteObject(job);
        }
    }
}