using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.BizTalk.ExplorerOM;

namespace Avista.ESB.Admin
{

      /// <summary>
      /// Class to load the bizTalk Group, host and host instance settings file.
      /// </summary>
      public sealed class BizTalkSettings
      {

            /// <summary>
            /// Method to read and load the Biztalk group setttings section.
            /// </summary>
            /// <param name="path">BizTalk group settings file path.</param>
            /// <returns></returns>
            public static SettingsRoot LoadBiztalkGroupSettings (string path)
            {
                  SettingsRoot root;
                  try
                  {
                        XmlDocument document = new XmlDocument();
                        document.Load( path );
                        List<SettingElement> list = new List<SettingElement>( 1 );
                        foreach ( XmlNode nameNode in document.SelectNodes( "/Settings/GroupSettings/Setting" ) )
                        {
                              list.Add( new SettingElement( nameNode.Attributes[ "Name" ].Value, nameNode.InnerText ) );
                        }
                        root = new SettingsRoot( list );
                  }
                  catch ( Exception exception )
                  {
                        throw exception;
                  }
                  return root;
            }
            /// <summary>
            /// Method to read and load the host settings section.
            /// </summary>
            /// <param name="path">BizTalk group settings file path</param>
            /// <returns></returns>
            public static HostSettings LoadBiztalkHostsSettings (string path)
            {
                  HostSettings settings;
                  try
                  {
                        XmlDocument document = new XmlDocument();
                        document.Load( path );
                        List<SettingsContainerWithNameAttr> list = new List<SettingsContainerWithNameAttr>();
                        foreach ( XmlNode node in document.SelectNodes( "/Settings/HostSettings/Host" ) )
                        {
                              var list2 = new List<SettingElement>(1);
                              foreach ( XmlNode settingsNode in node.SelectNodes("Setting"))
                              {
                                    list2.Add( new SettingElement(settingsNode.Attributes["Name"].Value, settingsNode.InnerText));
                              }
                              var item = new SettingsContainerWithNameAttr(node.Attributes["Name"].Value, list2 );
                              list.Add( item );
                        }
                        settings = new HostSettings(list);
                  }
                  catch ( Exception exception )
                  {
                        throw exception;
                  }
                  return settings;
            }
            /// <summary>
            /// Method to read and load the host instance settings section.
            /// </summary>
            /// <param name="path">BizTalk group settings file path</param>
            /// <returns></returns>
            public static HostInstanceSettings LoadBizTalkHostInstancesSettings (string path)
            {
                  HostInstanceSettings settings;
                  try
                  {
                        XmlDocument document = new XmlDocument();
                        document.Load( path );
                        List<ServerSettingsContainerWithNameAttr> settingsContainer = new List<ServerSettingsContainerWithNameAttr>();
                        foreach ( XmlNode HostNode in document.SelectNodes("/Settings/HostInstanceSettings/Host"))
                        {
                              List<SettingsContainerWithNameAttr> container = new List<SettingsContainerWithNameAttr>(1);
                              foreach ( XmlNode serverNode in HostNode.SelectNodes("Server"))
                              {
                                    var list = new List<SettingElement>(1);
                                    foreach ( XmlNode settingsNode in serverNode.SelectNodes("Setting"))
                                    {
                                          list.Add( new SettingElement(settingsNode.Attributes["Name"].Value, settingsNode.InnerText));
                                    }
                                    container.Add( new SettingsContainerWithNameAttr( serverNode.Attributes["Name"].Value, list));
                              }
                              settingsContainer.Add( new ServerSettingsContainerWithNameAttr( HostNode.Attributes["Name"].Value, container));
                        }
                        settings = new HostInstanceSettings(settingsContainer);
                  }
                  catch ( Exception exception )
                  {
                        throw exception;
                  }
                  return settings;
            }

       
      }
}
