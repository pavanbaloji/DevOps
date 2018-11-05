using System;
using System.Collections;
using System.Xml;
using System.Text;
using Microsoft.Build.Utilities;

namespace Avista.ESB.BuildTasks
{
    /// <summary>
    /// The ConfigEditor provides methods to automatically edit an XML configuration file based on input from an XML rules file.
    /// </summary>
    public class ConfigEditor
    {
        /// <summary>
        /// The command criteria that is used to 
        /// </summary>
        private string rulesFileName;
        private string configSourceFileName;

        public ConfigEditor(string configSourceFileName, string rulesFileName)
        {
            this.configSourceFileName = configSourceFileName;
            this.rulesFileName = rulesFileName;
        }
        
        /// <summary>
        /// Processes rules in the rules file .
        /// </summary>
        public void ProcessRules(Action action, TaskLoggingHelper log)
        {
            XmlDocument rulesDoc = null;
            XmlDocument sourceDoc = null;
            XmlDocument targetDoc = null;
            try
            {
                // Load the rules file.
                rulesDoc = new XmlDocument();
                rulesDoc.Load(rulesFileName);
                // Loop and process each <File> element in the rules file.
                XmlNodeList fileList = rulesDoc.SelectNodes("/Rules/File");
                IEnumerator fileEnumerator = fileList.GetEnumerator();
                while (fileEnumerator.MoveNext())
                {
                    XmlNode fileNode = (XmlNode)fileEnumerator.Current;
                    if (fileNode != null)
                    {
                        string targetFile = GetChildText(fileNode, "@Target", true, null, "Error parsing Target attribute from File element in rules file.");
                        //Logging.Logger.WriteTrace("Source File : " + sourceFile);
                        //Logging.Logger.WriteTrace("Target File : " + targetFile);
                        // Load the source file.
                        sourceDoc = new XmlDocument();
                        sourceDoc.Load(this.configSourceFileName);
                        //Logging.Logger.WriteTrace("Source Doc : " + sourceDoc.OuterXml);
                        // Load the target file and make a backup of it.
                        targetDoc = new XmlDocument();
                        targetDoc.Load(targetFile);
                        //Logging.Logger.WriteTrace("Target Doc : " + targetDoc.OuterXml);
                        targetDoc.Save(targetFile + String.Format("{0:-yyyy-MM-dd-HH-mm-ss}", DateTime.Now) + ".bak");
                        // Now loop through and apply each <Update> specified for the rules file for the <File> being processed.
                        // <Update Path="/configuration/configSections/section" Key="@name" Value="" />
                        XmlNodeList updateList = fileNode.SelectNodes("Update");
                        IEnumerator updateEnumerator = updateList.GetEnumerator();
                        while (updateEnumerator.MoveNext())
                        {
                            XmlNode updateNode = (XmlNode)updateEnumerator.Current;
                            if (updateNode != null)
                            {
                                string updatePath = GetChildText(updateNode, "@Path", true, null, "Error parsing Path attribute from Update element in rules file.");
                                //Logging.Logger.WriteTrace("Update Path : " + updatePath);
                                string updateKey = GetChildText(updateNode, "@Key", false, "", "Error parsing Path attribute from Update element in rules file.");
                                string expectedKeyValue = GetChildText(updateNode, "@Value", false, "", "Error parsing Value attribute from Update element in rules file.");
                                int[] order = GetUpdateOrder(updateNode);
                                // Find matching source nodes.
                                XmlNodeList sourceNodes = sourceDoc.SelectNodes(updatePath);
                                IEnumerator sourceEnumerator = sourceNodes.GetEnumerator();
                                while (sourceEnumerator.MoveNext())
                                {
                                    XmlNode sourceNode = (XmlNode)sourceEnumerator.Current;
                                    if (sourceNode != null)
                                    {
                                        string sourceKeyValue = GetChildText(sourceNode, updateKey, false, "", "Error pasing " + updateKey + " attribute from " + sourceNode.Name + " element in configuration template document " + this.configSourceFileName + ".");
                                        // Was an expected key value specified and does it match?
                                        if (expectedKeyValue == null || expectedKeyValue == "" || sourceKeyValue == expectedKeyValue)
                                        {
                                            // Now look for matches in the target?
                                            bool matchFound = false;
                                            XmlNodeList targetNodes = targetDoc.SelectNodes(updatePath);
                                            IEnumerator targetEnumerator = targetNodes.GetEnumerator();
                                            while (targetEnumerator.MoveNext())
                                            {
                                                XmlNode targetNode = (XmlNode)targetEnumerator.Current;
                                                if (targetNode != null)
                                                {
                                                    string targetKeyValue = GetChildText(targetNode, updateKey, false, "", "Error parsing " + updateKey + " attribute from " + targetNode.Name + " element in target configuration document " + targetFile + ".");
                                                    // Check for a match in the target document and take the appropriate action if a match is found.
                                                    // For an add operation we will display a warning message.
                                                    // For a remove operation we will remove the node from the target document.
                                                    if (sourceKeyValue == targetKeyValue)
                                                    {
                                                        matchFound = true;
                                                        if (action == Action.Add)
                                                        {
                                                            // Display a warning message.
                                                            string message = null;
                                                            if (updateKey == null)
                                                            {
                                                                message = "The element '" + updatePath + "' already exists in target config file '" + targetFile + ". It will not be added.";
                                                            }
                                                            else
                                                            {
                                                                message = "The element '" + updatePath + "/[" + updateKey + "=" + sourceKeyValue + "]' already exists in target config file '" + targetFile + ". It will not be added.";
                                                            }
                                                            log.LogMessage(message);
                                                        }
                                                        else if (action == Action.Remove)
                                                        {
                                                            // Remove the matching node from the target document.
                                                            XmlNode parent = targetNode.ParentNode;
                                                            parent.RemoveChild(targetNode);
                                                            // Remove all empty parents above the node that was removed (but make sure to stop at the document node).
                                                            while (parent.ParentNode.NodeType != XmlNodeType.Document && !parent.HasChildNodes)
                                                            {
                                                                //Logging.Logger.WriteTrace("Removing node :" + parent.Name + " (" + parent.NodeType + ") from " + parent.ParentNode.Name);
                                                                XmlNode grandParent = parent.ParentNode;
                                                                grandParent.RemoveChild(parent);
                                                                parent = grandParent;
                                                            }
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                            // Take the appropriate action if no match was found in the target document.
                                            // For an add operation we will add a node to the target document.
                                            // For a remove operation we will display a warning message.
                                            if (!matchFound)
                                            {
                                                if (action == Action.Add)
                                                {
                                                    AddNode(sourceNode, targetDoc, order);
                                                }
                                                else if (action == Action.Remove)
                                                {
                                                    string message = null;
                                                    if (updateKey == null)
                                                    {
                                                        message = "The element '" + updatePath + "' does not exist in target config file '" + targetFile + ". It will not be removed.";
                                                    }
                                                    else
                                                    {
                                                        message = "The element '" + updatePath + "/[" + updateKey + "=" + sourceKeyValue + "]' does not exist in target config file '" + targetFile + ". It will not be removed.";
                                                    }
                                                    log.LogMessage(message);
                                                }
                                            }
                                        }
                                    }
                                } // End of loop that searches for paths in the source doucment.
                            }
                        } // End of <Update> node processing loop
                        // Write the new verison of the target document.
                        //Logging.Logger.WriteTrace("File to save : " + targetDoc.OuterXml);
                        targetDoc.Save(targetFile);
                    }
                } // End of <File> node processing loop
            }
            catch (Exception e)
            {
                //Logging.Logger.WriteError("Stack Trace : " + e.StackTrace);
                throw new Exception("Error applying configuration rules. " + e.Message);
            }
        }

        /// <summary>
        /// Adds (copies) a node from a source document to a target document. The ancestry of the source node is generated in the target document if it does not already exist.
        /// </summary>
        /// <param name="sourceNode">The source node to be copied into the target document.</param>
        /// <param name="targetDoc">The target document to receive a copy of the source node.</param>
        /// <param name="order">An array of order values which indicate the preferred order (first child, second child, etc) of each element in the path of the source node.</param>
        private void AddNode(XmlNode sourceNode, XmlDocument targetDoc, int[] order)
        {
            // Figure out how many levels deep the source node is within it's document.
            // We do this by following the parent links until we reach the Document node.
            int depth = 0;
            XmlNode node = sourceNode;
            while (node != null && node.NodeType != XmlNodeType.Document)
            {
                depth++;
                node = node.ParentNode;
            }
            // Make sure that the order array matches the depth of the source node.
            // If a null order array was provided then create an order array of 0s.
            if (order == null)
            {
                order = new int[depth];
                for (int i = 0; i<depth; i++)
                {
                    order[i] = 0;
                }
            }
            else if (order.Length != depth)
            {
                throw new Exception("Order specification does not match depth of path to " + sourceNode.LocalName + " element.");
            }
            // Now build an array of the nodes from the root down to the sourceNode.
            XmlNode[] nodeList = new XmlNode[depth];
            int index = depth - 1;
            node = sourceNode;
            while (index >= 0)
            {
                nodeList[index] = node;
                node = node.ParentNode;
                index = index - 1;
            }
            // Now dive into the target document and add any nodes that are missing before the leaf node.
            index = 0;
            XmlNode prev = targetDoc;
            XmlNode curr = null;
            while (index < (depth - 1))
            {
                curr = prev.SelectSingleNode(nodeList[index].LocalName);
                if (curr == null) // Need to copy a node across from the source.
                {
                    curr = targetDoc.ImportNode(nodeList[index], false);
                    AddChild(prev, curr, order[index]);
                }
                prev = curr;
                curr = null;
                index = index + 1;
            }
            // Now add the final node with a deep copy.
            curr = targetDoc.ImportNode(nodeList[depth - 1], true);
            AddChild(prev, curr, order[index]);
        }


        /// <summary>
        /// Inserts a child node into a parent node with a given element order.
        /// </summary>
        /// <param name="parent">The parent node under which the child will be inserted.</param>
        /// <param name="child">The child node that will be inserted.</param>
        /// <param name="order">The order relative to it's siblings at which the child node should be inserted.
        /// A value of 0 indicates that the order is insignificant and the child will be appended to the end of the siblings.
        /// A value of 1 indicates that the child should be the first child of its parent. A value of 2 indicates that it
        /// should be the second child of its parent, and so on.</param>
        private void AddChild(XmlNode parent, XmlNode child, int order)
        {
            if (order == 0) // Order does not matter for this node so just insert it at the end.
            {
                parent.AppendChild(child);
            }
            else if (order == 1) // This should be the first child under the parent.
            {
                XmlNode firstChild = parent.FirstChild;
                if (firstChild == null)
                {
                    parent.AppendChild(child);
                }
                else
                {
                    parent.InsertBefore(child,firstChild);
                }
            }
            else // Insert child after the appropriate sibling (ex: if order is 3 then insert after sibling #2).
            {
                int index = 0;
                XmlNode sibling = parent.FirstChild;
                while (sibling != null && index < order)
                {
                    if (sibling.NodeType == XmlNodeType.Element)
                    {
                        index++;
                    }
                    else
                    {
                        sibling = sibling.NextSibling;
                    }
                }
                if (sibling == null)
                {
                    throw new Exception("Could not insert child with order " + order.ToString() + ". Insufficient siblings.");
                }
                else
                {
                    parent.InsertAfter(child, sibling);
                }
            }
        }

        
        /// <summary>
        /// Gets the text content of a child node. If the path points to an attribute then this will be the attribute value.
        /// If the path points to an element then this will be the value of the text node contained in the element. If either input
        /// parameter is null, if the path is not found, or if the path does not point to an attribute or element, then null will be returned.
        /// </summary>
        /// <param name="parentNode">The parent node from which the path will be searched.</param>
        /// <param name="childPath">The path to the node of interest.</param>
        /// <returns>The text value of the node found at the given path.</returns>
        private string GetChildText(XmlNode parentNode, string childPath, bool required, string defaultValue, string errorMessage)
        {
            string value = null;
            if (parentNode != null && childPath != null && childPath != "")
            {
                XmlNode childNode = parentNode.SelectSingleNode(childPath);
                if (childNode != null)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        XmlNode textNode = childNode.SelectSingleNode("text()");
                        if (textNode != null)
                        {
                            value = textNode.Value;
                        }
                    }
                    else if (childNode.NodeType == XmlNodeType.Attribute)
                    {
                        value = childNode.Value;
                    }
                }
            }
            return value;
        }


        /// <summary>
        /// Parses the Order attribute from an Update element into an integer array.
        /// </summary>
        /// <param name="parentNode">The Update element containing the Order attribute.</param>
        /// <returns>The value of the Order attribute as an integer array.</returns>
        private int[] GetUpdateOrder(XmlNode updateElement)
        {
            int[] order = null;
            try
            {
                if (updateElement != null)
                {
                    string orderText = GetChildText(updateElement, "@Order", false, "", "Error parsing Order attribute from Update element in rules file.");
                    if (orderText != null && orderText != "")
                    {
                        string[] orderValues = orderText.Split(',');
                        order = new int[orderValues.Length];
                        for (int i = 0; i < orderValues.Length; i++)
                        {
                            order[i] = Int32.Parse(orderValues[i]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error parsing order values from Order atribute. " + e.Message);
            }
            return order;
        }
    }
}
