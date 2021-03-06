﻿using ApiExplorer.ViewModels;
using MasonBuilder.Net;
using Microsoft.Practices.Composite.Presentation.Commands;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Linq;


namespace ApiExplorer.MediaTypeHandlers.ApplicationMason.ViewModels
{
  public class ResourceViewModel : JsonViewModel, IControlBuilder
  {
    #region UI properties

    public ObservableCollection<ControlViewModel> Controls { get; private set; }

    public bool HasControls { get { return Controls != null && Controls.Count > 0; } }

    public JToken ControlsJsonValue { get; private set; }

    public ObservableCollection<ViewModel> Properties { get; private set; }

    public string Description { get; set; }

    public bool HasDescription { get { return !string.IsNullOrEmpty(Description); } }

    public ObservableCollection<ControlViewModel> MetaControls { get; private set; }

    public bool HasMetaControls { get { return MetaControls != null && MetaControls.Count > 0; } }

    public JToken MetaControlsJsonValue { get; private set; }

    public JToken MetaJsonValue { get; private set; }

    #endregion


    #region Commands

    #endregion


    public ResourceViewModel(ViewModel parent, JObject resource, BuilderContext context)
      : base(parent, resource)
    {
      Properties = new ObservableCollection<ViewModel>();

      JObject namespaces = resource.SelectToken(MasonProperties.Namespaces) as JObject;
      if (namespaces != null)
        BuildNamespaces(namespaces, context);

      foreach (var pair in resource)
      {
        if (pair.Key == MasonProperties.Namespaces && pair.Value is JObject)
        {
          // Ignore - it has been handled
        }
        else if (pair.Key == MasonProperties.Controls && pair.Value is JObject)
        {
          ControlsJsonValue = pair.Value;
          Controls = new ObservableCollection<ControlViewModel>(
            pair.Value.Children().OfType<JProperty>().Select(n => BuildControlElement(this, n.Name, n.Value as JObject, context)).Where(n => n != null));
        }
        else if (pair.Key == MasonProperties.Meta && pair.Value is JObject)
        {
          MetaJsonValue = pair.Value;
          Description = GetValue<string>(pair.Value, MasonProperties.MetaProperties.Description);
          JToken metaControlsProperty = pair.Value[MasonProperties.Controls];
          if (metaControlsProperty is JObject)
          {
            MetaControlsJsonValue = metaControlsProperty;
            MetaControls = new ObservableCollection<ControlViewModel>(
              metaControlsProperty.Children().OfType<JProperty>().Select(l => BuildControlElement(this, l.Name, l.Value as JObject, context)));
          }
        }
        else if (pair.Key == MasonProperties.Error && pair.Value is JObject)
        {
          ResourcePropertyViewModel error = new ResourcePropertyViewModel(this, pair.Value, pair.Key, new ResourceViewModel(this, (JObject)pair.Value, context));
          error.IsError = true;
          Properties.Add(error);
        }
        else
        {
          Properties.Add(CreatePropertiesRecursively(pair.Key, pair.Value, context));
        }
      }

      if (Controls == null)
        Controls = new ObservableCollection<ControlViewModel>();
    }


    #region IControlBuilder

    public ControlViewModel BuildControlElement(ViewModel parent, string name, JObject value, BuilderContext context)
    {
      string encoding = GetValue<string>(value, "encoding", "none").ToLower();
      string method = GetValue<string>(value, "method", "GET").ToUpper();
      bool isHrefTemplate = GetValue<bool?>(value, "isHrefTemplate") ?? false;

      if (method == "GET" && encoding == MasonProperties.EncodingTypes.None)
      {
        if (isHrefTemplate)
          return new VoidActionViewModel(parent, name, value, context, this);
        else
          return new LinkViewModel(parent, name, value, context, this);
      }
      else if (encoding == MasonProperties.EncodingTypes.None)
        return new VoidActionViewModel(parent, name, value, context, this);
      else if (encoding == MasonProperties.EncodingTypes.JSON)
        return new JsonActionViewModel(parent, name, value, context, this);
      else if (encoding == MasonProperties.EncodingTypes.JSONFiles)
        return new JsonFilesActionViewModel(parent, name, value, context, this);

      return null;
    }

    #endregion

    private PropertyViewModel CreatePropertiesRecursively(string name, JToken json, BuilderContext context)
    {
      if (json is JArray)
      {
        int index=0;
        ObservableCollection<ViewModel> array = new ObservableCollection<ViewModel>(json.Select(i => CreatePropertiesRecursively(string.Format("[{0}]",index++), i, context)));
        return new ArrayPropertyViewModel(this, json, name, array);
      }
      else if (json is JObject)
      {
        return new ResourcePropertyViewModel(this, json, name, new ResourceViewModel(this, (JObject)json, context));
      }
      else
        return new PropertyViewModel(this, json, name, (json != null ? json.ToString() : ""));
    }


    private void BuildNamespaces(JObject namespaces, BuilderContext context)
    {
      foreach (JProperty ns in namespaces.Properties())
      {
        JObject nsDef = ns.Value as JObject;
        if (nsDef != null)
        {
          JToken jsonName = nsDef[MasonProperties.NamespaceProperties.Name];
          string ns_name = (jsonName != null && jsonName.Type == JTokenType.String ? jsonName.Value<string>() : null);
          string ns_prefix = ns.Name;
          if (!string.IsNullOrWhiteSpace(ns_prefix) && !string.IsNullOrWhiteSpace(ns_name))
            context.Namespaces.Namespace(ns_prefix, ns_name);
        }
      }
    }
  }
}
