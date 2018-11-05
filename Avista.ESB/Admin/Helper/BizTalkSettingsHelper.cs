using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.BizTalk.ExplorerOM;

namespace Avista.ESB.Admin.Helper
{
      /// <summary>
      /// BizTalk settings helper object
      /// </summary>
      public sealed class BizTalkSettingsHelper
      {

            /// <summary>
            /// Parse group settings
            /// </summary>
            /// <param name="path">Settings file path</param>
            /// <returns>setting root</returns>
            public static SettingsRoot ParseGroupSettings (string path)
            {
                  SettingsRoot root;
                  try
                  {
                        XmlDocument document = new XmlDocument();
                        document.Load( path );
                        List<SettingElement> listSettingsElement = new List<SettingElement>( 1 );
                        foreach ( XmlNode node in document.SelectNodes( "/Settings/GroupSettings/Setting" ) )
                        {
                              listSettingsElement.Add( new SettingElement( node.Attributes[ "Name" ].Value, node.InnerText ) );
                        }
                        root = new SettingsRoot( listSettingsElement );
                  }
                  catch ( Exception )
                  {
                        throw;
                  }
                  return root;
            }

            /// <summary>
            /// Parse Host Instance Settings
            /// </summary>
            /// <param name="path">settings file path</param>
            /// <returns>host instance settings</returns>
            public static HostInstanceSettings ParseHostInstancesSettings (string path)
            {
                  HostInstanceSettings settings;
                  try
                  {
                        XmlDocument document = new XmlDocument();
                        document.Load( path );
                        List<ServerSettingsContainerWithNameAttr> serverSettibgsContainerWithNameAttr = new List<ServerSettingsContainerWithNameAttr>();
                        foreach ( XmlNode node in document.SelectNodes( "/Settings/HostInstanceSettings/Host" ) )
                        {
                              List<SettingsContainerWithNameAttr> settingsContainerWithNameAttr = new List<SettingsContainerWithNameAttr>( 1 );

                              foreach ( XmlNode node2 in node.SelectNodes( "Server" ) )
                              {
                                    List<SettingElement> listSettingElement = new List<SettingElement>( 1 );
                                    foreach ( XmlNode node3 in node2.SelectNodes( "Setting" ) )
                                    {
                                          listSettingElement.Add( new SettingElement( node3.Attributes[ "Name" ].Value, node3.InnerText ) );
                                    }
                                    settingsContainerWithNameAttr.Add( new SettingsContainerWithNameAttr( node2.Attributes[ "Name" ].Value, listSettingElement ) );
                              }

                              serverSettibgsContainerWithNameAttr.Add( new ServerSettingsContainerWithNameAttr( node.Attributes[ "Name" ].Value, settingsContainerWithNameAttr ) );
                        }

                        settings = new HostInstanceSettings( serverSettibgsContainerWithNameAttr );
                  }
                  catch ( Exception )
                  {
                        throw;
                  }
                  return settings;
            }
            /// <summary>
            /// Parse host settings
            /// </summary>
            /// <param name="path">host settings file path</param>
            /// <returns>host settings</returns>
            public static HostSettings ParseHostsSettings (string path)
            {
                  HostSettings settings;
                  try
                  {
                        XmlDocument document = new XmlDocument();
                        document.Load( path );
                        List<SettingsContainerWithNameAttr> listSettingsNameAttribute = new List<SettingsContainerWithNameAttr>();
                        foreach ( XmlNode node in document.SelectNodes( "/Settings/HostSettings/Host" ) )
                        {
                              var listSettingsElement = new List<SettingElement>( 1 );

                              foreach ( XmlNode node2 in node.SelectNodes( "Setting" ) )
                              {
                                    listSettingsElement.Add( new SettingElement( node2.Attributes[ "Name" ].Value, node2.InnerText ) );
                              }

                              var item = new SettingsContainerWithNameAttr( node.Attributes[ "Name" ].Value, listSettingsElement );
                              listSettingsNameAttribute.Add( item );
                        }
                        settings = new HostSettings( listSettingsNameAttribute );
                  }
                  catch ( Exception )
                  {
                        throw;
                  }
                  return settings;
            }

      }
}