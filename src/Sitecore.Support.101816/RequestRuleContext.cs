namespace Sitecore.Support.FXM.Rules.Contexts
{
  using Sitecore.FXM.Rules.Contexts;
  using System;
  using System.Reflection;
  using System.Runtime.CompilerServices;
  using System.Web;

  public class RequestRuleContext : Sitecore.FXM.Rules.Contexts.RequestRuleContext
  {
    private Uri url;

    public RequestRuleContext(HttpRequestBase request) : base(request)
    {
      this.Request = request;
      url = Url;
      AssignBaseClassUrl(url);
    }

    public HttpRequestBase Request { get; protected set; }

    public Uri Url
    {
      get
      {
        if ((this.url == null) && (this.Request.QueryString != null))
        {
          string str = this.Request.QueryString["url"];
          if (!string.IsNullOrWhiteSpace(str))
          {
            try
            {
              this.url = new Uri(str);
            }
            catch
            {
            }
          }
          else
          {
            str = this.Request.QueryString["page"];
            if (!string.IsNullOrWhiteSpace(str))
            {
              try
              {
                this.url = new Uri(str);
              }
              catch
              {
              }
            }
          }
        }
        return (this.url ?? (this.url = this.Request.Url));
      }
    }

    protected void AssignBaseClassUrl(Uri urlToAssign)
    {
      FieldInfo fi = typeof(Sitecore.FXM.Rules.Contexts.RequestRuleContext).GetField("url", BindingFlags.Instance | BindingFlags.NonPublic);
      fi.SetValue(((Sitecore.FXM.Rules.Contexts.RequestRuleContext)this), urlToAssign);
      
    }
  }
}
