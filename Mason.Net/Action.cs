﻿namespace Mason.Net
{
  public class Action
  {
    public string name { get; set; }

    public string type { get; set; }

    public string href { get; set; }

    public string description { get; set; }


    public Action(string name, string type, string href, string description = null)
    {
      this.name = name;
      this.type = type;
      this.href = href;
      this.description = description;
    }
  }
}