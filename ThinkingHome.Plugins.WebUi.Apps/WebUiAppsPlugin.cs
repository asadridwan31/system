﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ThinkingHome.Core.Plugins;
using ThinkingHome.Plugins.WebServer.Attributes;
using ThinkingHome.Plugins.WebServer.Handlers;
using ThinkingHome.Plugins.WebUi.Attributes;

namespace ThinkingHome.Plugins.WebUi.Apps
{
    [JavaScriptResource("/static/web-ui/apps/common.js", "ThinkingHome.Plugins.WebUi.Apps.Resources.common.js")]
    [JavaScriptResource("/static/web-ui/apps/system.js", "ThinkingHome.Plugins.WebUi.Apps.Resources.system.js")]
    public class WebUiAppsPlugin : PluginBase
    {
        private readonly List<AppSectionAttribute> sections = new List<AppSectionAttribute>();

        private IEnumerable<AppSectionAttribute> GetPluginSections(PluginBase p)
        {
            return p.GetType().GetTypeInfo().GetCustomAttributes<AppSectionAttribute>();
        }

        public override void InitPlugin()
        {
            var list = Context.GetAllPlugins()
                .SelectMany(GetPluginSections)
                .OrderBy(s => s.SortOrder);

            sections.AddRange(list);
        }


        #region web api

        [WebApiMethod("/api/web-ui/apps/user")]
        public object LoadUserSections(HttpRequestParams request)
        {
            return GetSectionList(SectionType.User);
        }

        [WebApiMethod("/api/web-ui/apps/system")]
        public object LoadSystemSections(HttpRequestParams request)
        {
            return GetSectionList(SectionType.System);
        }

        private object GetSectionList(SectionType type)
        {
            return sections
                .Where(s => s.Type == type)
                .Select(s => new
                {
                    title = s.Title,
                    icon = s.Icon,
                    url = s.GetClientUrl()
                })
                .ToArray();
        }

        #endregion
    }
}