﻿
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;

using BatchLabs.Max2016.Plugin.Models;

namespace BatchLabs.Max2016.Plugin.Common
{
    public class TemplateHelper
    {
        private const string AppIndexJsonUrl = "https://raw.githubusercontent.com/Azure/BatchLabs-data/master/ncj/3dsmax/index.json";

        /// <summary>
        /// Read the collection of 3ds Max application templates from our GitHub repo. They 
        /// are all listed in a file called index.json.
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetApplicationTemplates()
        {
            var templates = new List<KeyValuePair<string, string>>();

            try
            {
                var request = WebRequest.Create(AppIndexJsonUrl);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var responseSteam = response.GetResponseStream())
                    {
                        // in the unlkely event, just return an empty collection
                        if (responseSteam == null)
                        {
                            return templates;
                        }

                        // deserialize the json response into a collection of application templates
                        var jsonSerializer = new DataContractJsonSerializer(new List<ApplicationTemplate>().GetType());
                        var templateList = jsonSerializer.ReadObject(responseSteam) as List<ApplicationTemplate>;
                        if (templateList != null)
                        {
                            foreach (var template in templateList)
                            {
                                Log.Instance.Debug($"Got template: {template.Id}, with name: {template.Name} ");
                                templates.Add(new KeyValuePair<string, string>(template.Id, template.Name));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"{ex.Message}\n{ex}", "Failed to get 3ds Max templates", true);
            }

            return templates;
        }

        /// <summary>
        /// List the renderer options for the 3ds Max templates
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetRenderers()
        {
            return new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("arnold", "Arnold"),
                new KeyValuePair<string, string>("vray", "V-Ray")
            };
        }
    }
}
