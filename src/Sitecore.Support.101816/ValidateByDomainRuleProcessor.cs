namespace Sitecore.Support.FXM.Pipelines.Tracking.ValidateRequest
{
  using Sitecore.Abstractions;
  using Sitecore.FXM.Abstractions;
  using Sitecore.FXM.Matchers;
  using Sitecore.FXM.Pipelines.Tracking.ValidateRequest;
  using Sitecore.FXM.Rules.Contexts;
  using Sitecore.Rules;
  using Sitecore.Support.FXM.Rules.Contexts;
  using System;
  using System.Linq;

  public class ValidateByDomainRuleProcessor : AbstractValidateRequestProcessor<ValidateRequestArgs>
  {
    public ValidateByDomainRuleProcessor()
    {
    }

    public ValidateByDomainRuleProcessor(IRuleFactory ruleFactory, IConfigurationFactory configurationFactory, IDomainMatcherRepository domainMatcherRepo) : base(ruleFactory, configurationFactory, domainMatcherRepo)
    {
    }

    protected override bool Match(ValidateRequestArgs args, Sitecore.FXM.Rules.Contexts.RequestRuleContext ruleContext, DomainMatcher domainMatcherItem)
    {
      bool flag = true;
      Sitecore.Support.FXM.Rules.Contexts.RequestRuleContext context = new Sitecore.Support.FXM.Rules.Contexts.RequestRuleContext(ruleContext.Request);
      if (!domainMatcherItem.RuleField.Domains.Any<string>())
      {
        return ((context.Url != null) && domainMatcherItem.Domain.Equals(context.Url.Host, StringComparison.InvariantCultureIgnoreCase));
      }
      RuleList<Sitecore.Support.FXM.Rules.Contexts.RequestRuleContext> list = base.RuleFactory.ParseRules<Sitecore.Support.FXM.Rules.Contexts.RequestRuleContext>(base.SitecoreContext.Database.Database, domainMatcherItem.RuleField.InnerField.Value);
      if (list.Count == 0)
      {
        return flag;
      }
      list.Run(context);
      return (context.IsValid && flag);
    }

    protected override bool ShouldProcess(ValidateRequestArgs args, Sitecore.FXM.Rules.Contexts.RequestRuleContext ruleContext) =>
        (args.DomainMatcher == null);
  }
}
