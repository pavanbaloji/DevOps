using Microsoft.Practices.ESB.Filters;
using Microsoft.Practices.ESB.Itinerary;
using Microsoft.Practices.ESB.Resolver;
using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avista.ESB.Utilities.BrokerService
{
    [Serializable]
    public class OutPort
    {
        private IDictionary<string, string> parameterCollection = new Dictionary<string, string>();
		private string filterConfig;
		private IItineraryStep step;
		private string portName;
		private string portConfig;
        private static readonly string resolverStartInd = "<![CDATA[";
		private static readonly string resolverStrStartInd = "&lt;![CDATA[";
		private static readonly string resolverDelimeter = "]]>";
		private static readonly string resolverStrdelimeter = "]]&gt;";

        public OutPort(IItineraryStep step, string portName, string portConfig)
		{
			this.portName = portName;
			this.step = step;
			this.portConfig = portConfig;
			string config = this.ParseFilterConfiguration(portConfig);
			this.parameterCollection = this.ParsePortSettings(config);
		}

        public bool Execute(XLANGMessage msg, out string nextId)
		{
			nextId = null;
            Hashtable resolverResults = new Hashtable();
			string key = this.step.ResolverCollection[Convert.ToInt32(this.parameterCollection["resolverPosition"])];
			Dictionary<string, string> dictionary;

			if (!resolverResults.ContainsKey(key))
			{
                ResolverInfo resolverInfo = ResolverMgr.GetResolverInfo(ResolutionType.Endpoint, this.step.ResolverCollection[0]);
                if (!resolverInfo.Success)
                {
                    throw new ApplicationException("Could not locate resolver");
                }
                dictionary = ResolverMgr.Resolve(resolverInfo, msg);

                resolverResults.Add(key, dictionary);
			}
			else
			{
				dictionary = (resolverResults[key] as Dictionary<string, string>);
			}
			string filterMoniker = string.Empty;
			string filterExpression = string.Empty;
			this.ParseFilterSettings(this.filterConfig, out filterMoniker, out filterExpression);
			IFilter filter = FilterFactory.Create(filterMoniker);
			if (filter.Execute(filterExpression, dictionary))
			{
				nextId = this.parameterCollection["id"];
				return true;
			}
			return false;
		}

		private IDictionary<string, string> ParsePortSettings(string config)
		{
			int firstIndex = config.IndexOf("[");
			int lastIndex = config.LastIndexOf("]");
			this.filterConfig = config.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
			string portSettings = config.Substring(lastIndex + 2, config.Length - lastIndex - 2);
			string[] portSettingArray = portSettings.Split(";".ToCharArray());
			Dictionary<string, string> dictionary = new Dictionary<string, string>(portSettingArray.Length);

            for (int i = 0; i < portSettingArray.Length; i++)
			{
                string portSettingKey = portSettingArray[i];
				string[] portSettingKeyValue = portSettingKey.Split("=".ToCharArray());
				dictionary.Add(portSettingKeyValue[0], portSettingKeyValue[1]);
			}
			return dictionary;
		}

		private void ParseFilterSettings(string config, out string filterMoniker, out string filterExpression)
		{
			filterExpression = string.Empty;
			filterMoniker = string.Empty;
			string[] array = config.Split(":".ToCharArray(), 2);
			if (array.Length <= 0)
			{
				throw new ApplicationException(string.Format("Cannot locate filter moniker from filter configuration {0}", config));
			}
			filterMoniker = array[0];
			int num = array[1].IndexOf("=");
			if (num < 0 || array.Length < 1)
			{
				throw new ApplicationException(string.Format("Cannot locate filter expression from filter configuration {0}", config));
			}
			filterExpression = array[1].Substring(num + 1, array[1].Length - num - 1);
			filterExpression = filterExpression.Replace(";", string.Empty);
		}

		private string ParseFilterConfiguration(string config)
		{
			if (config == null)
			{
				throw new ArgumentException("config");
			}
			bool flag = true;
			string configText = config;
			while (flag)
			{
				int num = configText.IndexOf(OutPort.resolverDelimeter);
				if (num != -1)
				{
					string text = configText.Substring(0, num + OutPort.resolverDelimeter.Length);
					text = text.Replace(OutPort.resolverStartInd, "");
					text = text.Replace(OutPort.resolverDelimeter, "");
					text = text.Replace("&amp;", "&");
					if (text.Length > 1)
					{
						return text;
					}
					configText = configText.Substring(num + OutPort.resolverDelimeter.Length).Trim();
				}
				else
				{
					num = configText.IndexOf(OutPort.resolverStrdelimeter);
					if (num != -1)
					{
						string text = configText.Substring(0, num + OutPort.resolverStrdelimeter.Length);
						text = text.Replace(OutPort.resolverStrStartInd, "");
						text = text.Replace(OutPort.resolverStrdelimeter, "");
						text = text.Replace("&amp;", "&");
						if (text.Length > 1)
						{
							return text;
						}
						configText = configText.Substring(num + OutPort.resolverStrdelimeter.Length).Trim();
					}
					else
					{
						flag = false;
					}
				}
			}
			throw new ApplicationException(string.Format("Could not parse filter connection string {0}", config));
		}

    }
}
