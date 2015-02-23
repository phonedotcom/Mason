﻿using ApiExplorer.Utilities;
using ApiExplorer.ViewModels;
using MasonBuilder.Net;
using Microsoft.Practices.Composite.Presentation.Commands;
using Newtonsoft.Json.Linq;
using Ramone;
using System.Collections.ObjectModel;


namespace ApiExplorer.MediaTypeHandlers.ApplicationMason.ViewModels
{
  public class LinkViewModel : ControlViewModel
  {
    public override string ControlType { get { return "Link"; } }


    public LinkViewModel(ViewModel parent, JProperty link, BuilderContext context)
      : base(parent, link, context)
    {
    }


    #region Commands

    protected override void ActivateControl(object args)
    {
      ISession session = RamoneServiceManager.Session;

      Request req = session.Bind(HRef).Method("GET");

      Publish(new ExecuteWebRequestEventArgs(session, req));
    }

    #endregion
  }
}
