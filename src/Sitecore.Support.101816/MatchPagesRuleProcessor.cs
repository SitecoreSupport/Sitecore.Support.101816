namespace Sitecore.Support.FXM.Pipelines.Tracking.MatchPages
{
  using Rules;
  using Sitecore.Abstractions;
  using Sitecore.Collections;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.FXM.Abstractions;
  using Sitecore.FXM.Matchers.Element;
  using Sitecore.FXM.Pipelines.Tracking.MatchPages;
  using Sitecore.FXM.Rules;
  using Sitecore.FXM.Rules.Contexts;
  using Sitecore.Rules;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class MatchPagesRuleProcessor : IMatchPagesProcessor<MatchPagesArgs>
  {
    protected internal readonly ISitecoreContext Context;
    protected internal readonly ILog Logger;
    protected internal readonly IRuleFactory RuleFactory;

    public MatchPagesRuleProcessor() : this(new ValidatingRuleFactory(), new LogWrapper(), new SitecoreContextWrapper())
    {
    }

    public MatchPagesRuleProcessor(IRuleFactory ruleFactory, ILog log, ISitecoreContext context)
    {
      Assert.ArgumentNotNull(ruleFactory, "ruleFactory");
      Assert.ArgumentNotNull(log, "log");
      Assert.ArgumentNotNull(context, "context");
      this.RuleFactory = ruleFactory;
      this.Logger = log;
      this.Context = context;
    }

    private void AddItemMatchModel(Item item, MatchPagesArgs args)
    {
      ItemMatchModel model = new ItemMatchModel
      {
        Id = item.ID.ToString()
      };
      if (item.ID == args.DomainMatcher.Item.ID)
      {
        model.MatchType = "domain";
      }
      args.ElementMatches.Add(model);
    }

    private IEnumerable<Item> GetPageMatcherChildren(Item item)
    {
      ChildList children = item.Children;
      if (children == null)
      {
        return Enumerable.Empty<Item>();
      }
      return (from x in children
              where x.TemplateID.ToString() == "{B889DE68-77E0-432C-9786-DE61D129D3DF}"
              select x);
    }

    public void Process(MatchPagesArgs args)
    {
      Assert.IsNotNull(args.DomainMatcher, "Domain matcher item not found.");
      if (args.DomainMatcher.DoNotTrack)
      {
        this.AddItemMatchModel(args.DomainMatcher.Item, args);
      }
      Item latestVersion = args.DomainMatcher.Item.Versions.GetLatestVersion(this.Context.Language);
      IEnumerable<Item> pageMatcherChildren = this.GetPageMatcherChildren(latestVersion);
      this.ProcessMatcher(pageMatcherChildren, args);
    }

    private bool ProcessMatcher(IEnumerable<Item> items, MatchPagesArgs args)
    {
      bool flag = false;
      foreach (Item item in items)
      {
        bool isValid = item.TemplateID.ToString() == "{036DB470-1850-4848-A48A-0931F825B867}";
        if (!isValid)
        {
          string str = item["{55028B4D-1547-4799-995E-E14FE0378257}"];
          if (!string.IsNullOrWhiteSpace(str))
          {
            RuleList<Sitecore.Support.FXM.Rules.Contexts.RequestRuleContext> list = this.RuleFactory.ParseRules<Sitecore.Support.FXM.Rules.Contexts.RequestRuleContext>(item.Database, str);
            Sitecore.Support.FXM.Rules.Contexts.RequestRuleContext ruleContext = new Sitecore.Support.FXM.Rules.Contexts.RequestRuleContext(args.Request);
            list.Run(ruleContext);
            if (ruleContext.IsValid)
            {
              this.Logger.Debug($"[PageFilter] Matched filter: {item.Name} ", null);
            }
            isValid = ruleContext.IsValid;
          }
        }
        if (isValid)
        {
          if (args.DomainMatcher.DoNotTrack)
          {
            this.AddItemMatchModel(item, args);
          }
          IEnumerable<Item> pageMatcherChildren = this.GetPageMatcherChildren(item);
          if (!this.ProcessMatcher(pageMatcherChildren, args))
          {
            args.Matches.Add(item);
            flag = true;
          }
        }
      }
      return flag;
    }
  }
}
