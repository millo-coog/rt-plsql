using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.IO;

namespace RT {
	public class xmlHelper {
		private string prvXMLFilename;

		// Get/set methods
		public string filename {
			get { return prvXMLFilename; }
			set {
				if (value == null)
					throw new Exception("You cannot specify a null filename.");

				if (value.Trim() == String.Empty)
					throw new Exception("You cannot specify an empty filename.");

				if (File.Exists(value) == false)
					throw new Exception("XML file '" + value + "' does not exist.");

				prvXMLFilename = value;
			}
		}

		// Constructor
		public xmlHelper(String xmlFilename) {
			filename = xmlFilename;
		}

		// Returns the requested attribute's value from the given node if it exists;
		// otherwise, just an empty string is returned. Particularly useful for getting
		// optional attributes.
		public string getAttribute(XmlNode node, String name, String defaultValue = "", bool required = false) {
			// Validate parameters
			if (node == null)
				throw new Exception("Node parameter object cannot be null.");

			if (name == null)
				throw new Exception("Attribute name cannot be null.");

			if (name.Trim() == String.Empty)
				throw new Exception("Attribute name cannot be empty.");

			if (required && defaultValue != String.Empty)
				throw new Exception("You cannot specify a default value for required parameters.");
			
			// Get the value of the requested attribute...
			if (node.Attributes[name] == null) {
				// This node doesn't have the requested attribute...
				if (required) {
					throw new Exception("When loading '" + prvXMLFilename + "', the '" + node.Name + "' tag does not have the required '" + name + "' attribute.");
				} else {
					return defaultValue;
				}
			} else {
				// Attribute was found...
				String attributeValue = node.Attributes[name].Value;

				if (attributeValue.Trim() == String.Empty) {
					if (required) {
						// Required attributes must also have a value specified...
						throw new Exception("When loading '" + prvXMLFilename + "', the '" + node.Name + "' tag must have a value in the required '" + name + "' attribute.");
					} else {
						// The attribute is declared, but is empty and optional, so treat it like the non-required attribute were entirely missing.
						return defaultValue;
					}
				} else {
					return attributeValue;
				}
			}
		}

		public bool hasChildNode(XmlNode node, string name) {
			bool hasChild = false;

			foreach (XmlNode child in node.ChildNodes) {
				if (child.Name == name) {
					hasChild = true;
					break;
				}
			}

			return hasChild;
		}

		// Returns the requested single child node of the given name, from the given XML node.
		public XmlNode getSingleNode(XmlNode node, String name, bool required = false) {
			// Validate parameters
			if (node == null)
				throw new Exception("Node parameter object cannot be null.");

			if (name == null)
				throw new Exception("name parameter cannot be null.");

			if (name == String.Empty)
				throw new Exception("name parameter cannot be empty.");

			// Obtain the requested child node...
			XmlNodeList childNodes = node.SelectNodes(name);

			if (childNodes.Count > 1)
				throw new Exception("Found more than one '" + name + "' child nodes in '" + prvXMLFilename + "'. There can only be one of these.");

			if (required && childNodes.Count == 0) {
				throw new Exception("Cannot find the required '" + name + "' node in '" + prvXMLFilename + "'.");
			}

			return childNodes[0];
		}

		public string getSingleNodesText(XmlNode node, String name, bool required = false) {
			XmlNode childNode = getSingleNode(node: node, name: name, required: required);

			if (childNode == null) {
				return string.Empty;
			}  else {
				return childNode.InnerText;
			}
		}
	}
}
