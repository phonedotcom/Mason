﻿using Mason.Net;
using Newtonsoft.Json;


namespace Mason.CaseFile.Contract
{
  public class CaseFile : Resource
  {
    public string ID { get; set; }

    public string Title { get; set; }
  }
}
