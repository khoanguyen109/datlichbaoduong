using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using Vendare.Error;

namespace Vendare.Utils
{
    public static class XmlParser
    {
        /// <summary>
        /// Returns the values in the nodes as per xpath query
        /// </summary>
        /// <param name="XmlFilePath">Fully qualified file path</param>
        /// <param name="xPathExpression">Xpath Expression</param>
        /// <returns>Generic List of Strings</returns>
        public static List<string> GetNodeValues(string XmlFilePath, string xPathExpression)
        {
            List<string> myList = new List<string>();

            if (XmlFilePath != null && XmlFilePath.Length > 0 && System.IO.File.Exists(XmlFilePath) &&
                xPathExpression != null && xPathExpression.Length > 0)
            {
                XPathDocument xDoc = new XPathDocument(XmlFilePath);
                myList = GetNodeInfo(xDoc, xPathExpression);
            }
            return myList;
        }

        /// <summary>
        /// Returns the values in the nodes as per xpath query
        /// </summary>
        /// <param name="AssemblyEmbeddedXmlFileName">File Name of Embedded Resource.</param>
        /// <param name="xPathExpression">Xpath Expression</param>
        /// <param name="AssemblyName">Long Form of Assembly Name, Pass NULL if your data is in current Assembly</param>
        /// <returns>Generic List of Strings</returns>
        public static List<string> GetNodeValues(string AssemblyEmbeddedXmlFileName, string xPathExpression, string AssemblyName)
        {
            List<string> myList = new List<string>();

            if (AssemblyEmbeddedXmlFileName != null && AssemblyEmbeddedXmlFileName.Length > 0 &&
                xPathExpression != null && xPathExpression.Length > 0)
            {
                Assembly exe;

                if (AssemblyName != null)
                    exe = Assembly.Load(AssemblyName);
                else
                    exe = Assembly.GetExecutingAssembly();

                ManifestResourceInfo info = exe.GetManifestResourceInfo(AssemblyEmbeddedXmlFileName);
                Stream reader = exe.GetManifestResourceStream(AssemblyEmbeddedXmlFileName);
                XPathDocument xDoc = new XPathDocument(reader);
                myList = GetNodeInfo(xDoc, xPathExpression);
            }
            return myList;
        }

        /// <summary>
        /// Returns the values in the nodes as per xpath query
        /// </summary>
        /// <param name="Stream">IO Stream that has XML in it.</param>
        /// <param name="xPathExpression">Xpath Expression</param>
        /// <returns>Generic List of Strings</returns>
        public static List<string> GetNodeValues(Stream ioStream, string xPathExpression)
        {
            List<string> myList = new List<string>();
            if (ioStream != null && xPathExpression != null)
            {
                XPathDocument xDoc = new XPathDocument(ioStream);
                myList = GetNodeInfo(xDoc, xPathExpression);
            }

            return myList;
        }

        /// <summary>
        /// Returns the values in the nodes as per xpath query
        /// </summary>
        /// <param name="Stream">IO Stream that has XML in it.</param>
        /// <param name="xPathExpression">Xpath Expression</param>
        /// <param name="attributes">get all attributes</param>
        /// <returns>Generic List of Strings</returns>
        public static List<string> GetNodeValues(Stream ioStream, string xPathExpression, out Dictionary<string, string> attributes)
        {
            List<string> myList = new List<string>();
            attributes = new Dictionary<string, string>();
            if (ioStream != null && xPathExpression != null)
            {
                XPathDocument xDoc = new XPathDocument(ioStream);
                myList = GetNodeInfo(xDoc, xPathExpression, out attributes);
            }

            return myList;
        }

        /// <summary>
        /// Actual Internal Proc that Opens the XML and gets nodes using xpath expression and captures its values
        /// </summary>
        /// <param name="xDoc">Document object of Xml</param>
        /// <param name="xPathExpression">Xpath Expression to Query</param>
        /// <returns>Gereric List of String values</returns>
        private static List<string> GetNodeInfo(XPathDocument xDoc, string xPathExpression)
        {
            List<string> myList = new List<string>();
            try
            {
                XPathNavigator xNav = xDoc.CreateNavigator();
                XPathExpression xExpr = xNav.Compile(xPathExpression);
                XPathNodeIterator iterator = xNav.Select(xExpr);

                while (iterator.MoveNext())
                {
                    XPathNavigator xNavTemp = iterator.Current.Clone();
                    myList.Add(xNavTemp.Value.Trim().ToLower());
                }

                iterator = null;
                xExpr = null;
                xNav = null;
            }
            catch (Exception ex)
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("xPath", xPathExpression);
                throw new LoggableException(ex, nvc);
            }
            return myList;
        }

        /// <summary>
        /// Actual Internal Proc that Opens the XML and gets nodes using xpath expression and captures its values
        /// </summary>
        /// <param name="xDoc">Document object of Xml</param>
        /// <param name="xPathExpression">Xpath Expression to Query</param>
        /// <param name="attributes">get all the attributes for the node</param>
        /// <returns>Gereric List of String values</returns>
        private static List<string> GetNodeInfo(XPathDocument xDoc, string xPathExpression, out Dictionary<string, string> attributes)
        {
            List<string> myList = new List<string>();
            attributes = new Dictionary<string, string>();
            try
            {
                XPathNavigator xNav = xDoc.CreateNavigator();
                XPathExpression xExpr = xNav.Compile(xPathExpression);
                XPathNodeIterator iterator = xNav.Select(xExpr);

                while (iterator.MoveNext())
                {
                    XPathNavigator xNavTemp = iterator.Current.Clone();
                    myList.Add(xNavTemp.Value.Trim().ToLower());

                    xNavTemp.MoveToFirstAttribute();
                    attributes.Add(xNavTemp.Name.Trim().ToLower(), xNavTemp.Value.Trim().ToLower());

                    while (xNavTemp.MoveToNextAttribute())
                    {
                        attributes.Add(xNavTemp.Name.Trim().ToLower(), xNavTemp.Value.Trim().ToLower());
                    }
                }

                iterator = null;
                xExpr = null;
                xNav = null;
            }
            catch (Exception ex)
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("xPath", xPathExpression);
                throw new LoggableException(ex, nvc);
            }
            return myList;
        }

    }
}

