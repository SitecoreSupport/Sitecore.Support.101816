namespace Sitecore.Support.FXM.Pipelines.Tracking.ValidateRequest
{
    using Sitecore.Abstractions;
    using Sitecore.FXM.Abstractions;
    using Sitecore.FXM.Matchers;
    using Sitecore.FXM.Pipelines.Tracking.ValidateRequest;
    using Sitecore.FXM.Rules.Contexts;
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
            Sitecore.Support.FXM.Rules.Contexts.RequestRuleContext context = new Sitecore.Support.FXM.Rules.Contexts.RequestRuleContext(ruleContext.Request);
            if (!domainMatcherItem.RuleField.Domains.Any<string>())
            {
                return ((context.Url != null) && domainMatcherItem.Domain.Equals(context.Url.Host, StringComparison.InvariantCultureIgnoreCase));
            }
            base.RuleFactory.ParseRules<Sitecore.FXM.Rules.Contexts.RequestRuleContext>(base.Database.Database, domainMatcherItem.RuleField.InnerField.Value).Run(ruleContext);
            return ruleContext.IsValid;
        }

        protected override bool ShouldProcess(ValidateRequestArgs args, Sitecore.FXM.Rules.Contexts.RequestRuleContext ruleContext) => 
            (args.DomainMatcher == null);
    }
}
