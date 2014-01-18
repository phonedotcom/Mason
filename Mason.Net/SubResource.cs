﻿using Newtonsoft.Json;
using System.Collections.Generic;


namespace Mason.Net
{
  public class SubResource : DynamicDictionary
  {
    [JsonProperty(MasonProperties.Prefix + "links")]
    public List<Link> Links { get; set; }

    [JsonProperty(MasonProperties.Prefix + "link-templates")]
    public List<LinkTemplate> LinkTemplates { get; set; }

    [JsonProperty(MasonProperties.Prefix + "actions")]
    public List<Action> Actions { get; set; }


    public void AddLink(Link l)
    {
      if (Links == null)
        Links = new List<Link>();
      Links.Add(l);
    }


    public void AddLinkTemplate(LinkTemplate t)
    {
      if (LinkTemplates == null)
        LinkTemplates = new List<LinkTemplate>();
      LinkTemplates.Add(t);
    }


    public void AddAction(Action a)
    {
      if (Actions == null)
        Actions = new List<Action>();
      Actions.Add(a);
    }
  }
}
