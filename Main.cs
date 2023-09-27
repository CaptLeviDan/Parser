using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using XMLParser;

namespace XMLParser
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello World!");
                XNamespace my = "http://schemas.microsoft.com/office/infopath/2003/myXSD/2013-08-20T04:11:29";
                XmlDocument doc = new XmlDocument();
                doc.Load("C:\\book.xml");
                XDocument xdoc = XDocument.Load("C:\\book.xml");

                string orderNumber = "";
                string orderTitle = "";

                // ftech a tags.
                foreach (var node in xdoc.Descendants(my + "OrderNo").ToList())
                {
                    orderNumber = node.Value.Replace("\n", "").Replace("\t", "");
                }

                // ftech a tags.
                foreach (var node in xdoc.Descendants(my + "OrderTitle").ToList())
                {
                    orderTitle = node.Value.Replace("\n", "").Replace("\t", "");
                }

                string newDirectoryPath = "E:\\" + orderTitle + orderNumber + "\\";


                //// Remove a tags.
                //foreach (var node in xdoc.Descendants("a").ToList())
                //{
                //    var x1 = node;
                //    var linkValue = "\r\n" + node.Value + "-" + node.FirstAttribute.Value + "\r\n";
                //    node.ReplaceWith(linkValue);
                //}

                // Remove html tags.
                foreach (XElement elementsGroup in xdoc.Descendants(my + "group13").ToList())
                {
                    var typeValue = "";
                    var isStrikeOut = false;

                    foreach (var typeNode in elementsGroup.Descendants(my + "Type"))
                    {
                        typeValue = typeNode.Value.Trim();
                    }


                    ////IF Type is New --> no check required.
                    if (typeValue == "New")
                    {
                        foreach (XElement elements in elementsGroup.Descendants("html").ToList())
                        {
                            var finalString = "\r\n";
                            IEnumerable<XNode> nodes = elements.DescendantNodes();

                            var queryRes = from res in nodes select res;

                            foreach (var node in nodes)
                            {
                                if (node.NodeType == XmlNodeType.Text)
                                {
                                    finalString = finalString + "\t" + node;
                                }
                            }

                            elements.ReplaceWith(finalString);
                        }
                    }


                    ////IF Type is Change --> check for strike data is required.
                    if (typeValue == "Change")
                    {
                        foreach (XElement elements in elementsGroup.Descendants("html").ToList())
                        {
                            var finalString = "\r\n";

                            foreach (var nodes in elements.Descendants().ToList())
                            {
                                if (nodes.HasAttributes && nodes.NodeType == XmlNodeType.Element && !nodes.HasElements)
                                {
                                    var atrrStyle = nodes.Attribute("style");
                                    var atrrHref = nodes.Attribute("href");
                                    if (atrrStyle != null && atrrStyle.Value.Contains("line-through"))
                                    {
                                        isStrikeOut = true;
                                        nodes.ReplaceWith("");
                                    }
                                    else if (atrrHref != null)
                                    {
                                        var linkValue = "\r\n" + atrrHref.Parent.Value + "-" + atrrHref.Value + "\r\n";
                                        finalString = finalString + "\t" + linkValue;
                                    }
                                    else
                                    {
                                        finalString = finalString + "\t" + nodes.Value;
                                    }

                                }
                                else if (!nodes.HasElements)
                                {
                                    finalString = finalString + "\t" + nodes.Value;
                                }

                            }

                            elements.ReplaceWith(finalString);
                        }
                    }

                    ////IF Type is Change --> check for strike data is required.
                    if (typeValue == "Delete" || typeValue == null || typeValue == "")
                    {
                        foreach (XElement elements in elementsGroup.Descendants("html").ToList())
                        {
                            var finalString = "\r\n";
                            var isStrikeData = false;
                            var isNonStrikeData = false;

                            foreach (var nodes in elements.Descendants().ToList())
                            {
                                if (nodes.HasAttributes && nodes.NodeType == XmlNodeType.Element && !nodes.HasElements)
                                {
                                    var atrrStyle = nodes.Attribute("style");
                                    var atrrHref = nodes.Attribute("href");
                                    if (atrrStyle != null && atrrStyle.Value.Contains("line-through"))
                                    {
                                        isStrikeData = true;
                                        nodes.ReplaceWith("");
                                    }
                                    else if (atrrHref != null)
                                    {
                                        var linkValue = "\r\n" + atrrHref.Parent.Value + "-" + atrrHref.Value + "\r\n";
                                        finalString = finalString + "\t" + linkValue;
                                    }
                                    else
                                    {
                                        isNonStrikeData = true;
                                        finalString = finalString + "\t" + nodes.Value;
                                    }
                                }
                                else if (!nodes.HasElements)
                                {
                                    isNonStrikeData = true;
                                    finalString = finalString + "\t" + nodes.Value;
                                }

                            }
                            //if (typeValue == "Change" || typeValue == "New")
                            //{
                            //    elements.ReplaceWith(finalString);
                            //}

                            if (typeValue == "Delete")
                            {
                                if (isStrikeData)
                                {
                                    elements.ReplaceWith("");
                                }
                                else
                                {
                                    elements.ReplaceWith(finalString);
                                }
                            }

                            if (typeValue == null || typeValue == "")
                            {
                                if ((isStrikeData && isNonStrikeData) || (!isStrikeData && isNonStrikeData))
                                {
                                    elements.ReplaceWith(finalString);
                                }
                                else if ((isStrikeData && !isNonStrikeData) || (!isStrikeData && !isNonStrikeData))
                                {
                                    elements.ReplaceWith("");
                                }
                            }

                        }
                    }



                }

                //// Remove html tags.
                //foreach (XElement elements in xdoc.Descendants("html").ToList())
                //{
                //    var finalString = "\r\n";
                //    IEnumerable<XNode> nodes = elements.DescendantNodes();

                //    var queryRes = from res in nodes select res;

                //    foreach (var node in nodes)
                //    {
                //        if (node.NodeType == XmlNodeType.Text)
                //        {
                //            finalString = finalString + "\t" + node;
                //        }
                //    }

                //    elements.ReplaceWith(finalString);
                //}

                //// Remove div tags.
                //foreach (XElement elements in xdoc.Descendants("div").ToList())
                //{
                //    var finalString = "\r\n";
                //    IEnumerable<XNode> nodes = elements.DescendantNodes();

                //    var queryRes = from res in nodes select res;

                //    foreach (var node in nodes)
                //    {
                //        if (node.NodeType == XmlNodeType.Text)
                //        {
                //            finalString = finalString + "\t" + node;
                //        }
                //    }

                //    elements.ReplaceWith(finalString);
                //}


                //foreach (XElement elementObj in element)
                //{
                //    var stringObj = elementObj.ToString();
                //    var doc2 = new HtmlDocument();
                //    doc2.LoadHtml(stringObj);
                //    var result = doc2.DocumentNode.InnerText;

                //    elementObj.ReplaceAll(result);
                //}



                //XmlNodeReader nodeReader = (XmlNodeReader)doc1.CreateReader();
                //doc.ReadNode(nodeReader);
                //XmlNodeList xmlnodeList = doc.SelectNodes("html");

                // var input = "<html><p><span>Novartis Hello world</span></p></html>";
                // var doc2 = new HtmlDocument();
                // doc2.LoadHtml(input);
                //var result = doc2.DocumentNode.InnerText;

                //IEnumerable<XElement> updatedElement = doc1.Descendants("myFields");

                //foreach (XElement elementObj in updatedElement)
                //{
                //    var stringObj = elementObj.ToString();
                //    var doc2 = new HtmlDocument();
                //    doc2.LoadHtml(stringObj);
                //    var resultMain = doc2.DocumentNode.InnerText;

                //    elementObj.ReplaceWith(resultMain);
                //}


                //var resObj = doc1.ToString();

                //resObj.Replace("<html>", "");
                //resObj.Replace("</html>", "");

                //doc.LoadXml(resObj);
                //doc.Save("bookUpdated1.xml");
                Console.WriteLine("File clean up complete");
                Directory.CreateDirectory(newDirectoryPath);

                xdoc.Save(newDirectoryPath + orderTitle + orderNumber + ".xml");
                Console.WriteLine("File saved....");
                Console.WriteLine("Completed...");


                //// Display contents on the console
                //doc.Save(Console.Out);
            }
            catch (Exception ex)
            {

            }

        }
    }
}
