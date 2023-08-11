using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XMLTAgsExtractor.Services
{
    public class Extraction : IExtraction
    {
        private readonly ILogger<Extraction> _logger;

        public Extraction(ILogger<Extraction> logger)
        {
            _logger = logger;
        }
        public bool CheckFileExtension(string fileUrl)
        {
            bool isXML = false;
            try
            {
                string ext = fileUrl.Substring(fileUrl.Length - 3);

                if (ext == "xml")
                {
                    isXML = true;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($" - Exception at CheckFileExtension. Message: {ex.Message}");
            }
            return isXML;
        }
        public bool VerifyFileUrl(string fileUrl)
        {
            bool isAvailable = false;
            try
            {
                Uri fileUri = new Uri(fileUrl);
                if (fileUri.Scheme != Uri.UriSchemeHttp || fileUri.Scheme != Uri.UriSchemeHttps)
                {
                    if (fileUri.Scheme != "file")
                    {
                        return isAvailable;
                    }
                }
                isAvailable = true;
            }
            catch (UriFormatException ex) 
            {
                _logger.LogError($" - Exception at VerifyFileUrl. Message: {ex.Message}");
            }
            return isAvailable;
        }

        public List<string> ExtractTags(string fileUrl)
        {
            List<string> tagsStartingWithAngleBracket = new List<string>();
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                byte[] xmlBytes = ReadAllBytes(fileUrl);
                string xmlContent = Encoding.UTF8.GetString(xmlBytes).TrimStart('\uFEFF');
                if (string.IsNullOrEmpty(xmlContent))
                {
                    return new List<string>();
                }
                xmlDocument.LoadXml(xmlContent);

                if (xmlDocument.HasChildNodes)
                {
                    foreach (XmlNode node in xmlDocument.ChildNodes)
                    {
                        TraverseNode(node, tagsStartingWithAngleBracket);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($" - Exception at ExtractTags. Message: {ex.Message}");
            }
            return tagsStartingWithAngleBracket;
        }
        public Dictionary<string, object> ExtractTagsandTagContent(string fileUrl)
        {
            XmlDocument xmlDocument = new XmlDocument();
            byte[] xmlBytes = ReadAllBytes(fileUrl);
            string xmlContent = Encoding.UTF8.GetString(xmlBytes).TrimStart('\uFEFF');
            if (string.IsNullOrEmpty(xmlContent))
            {
                return new Dictionary<string, object>();
            }
            xmlDocument.LoadXml(xmlContent);

            var rootNode = TraverseNode(xmlDocument.DocumentElement);
            return new Dictionary<string, object> { 
                { xmlDocument.DocumentElement.Name, rootNode} 
            };
        }

        private byte[] ReadAllBytes(string fileUrl)
        {
            return System.IO.File.ReadAllBytes(fileUrl);
        }

        private void TraverseNode(XmlNode node, List<string> tagsList)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                tagsList.Add("<" + node.Name + ">");

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    TraverseNode(childNode, tagsList);
                }
            }
        }

        
        private object TraverseNode(XmlNode node)
        {
            if (node.HasChildNodes && node.ChildNodes.Count == 1 && node.FirstChild.NodeType == XmlNodeType.Text)
            {
                return node.InnerText.Trim();
            }
            else if (node.HasChildNodes)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        var childResult = TraverseNode(childNode);
                        if (result.ContainsKey(childNode.Name))
                        {
                            var existingValue = result[childNode.Name];
                            if (existingValue is List<Dictionary<string, object>> existingList)
                            {
                                existingList.Add((Dictionary<string, object>)childResult);
                            }
                            else
                            {
                                result[childNode.Name] = new List<object> { existingValue, childResult };

                            }
                        }
                        else
                        {
                            result[childNode.Name] = childResult;
                        }
                    }
                }

                if (node.Attributes != null && node.Attributes.Count > 0)
                {
                    Dictionary<string, string> attributes = new Dictionary<string, string>();
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        attributes[attribute.Name] = attribute.Value;
                    }

                    result["@attributes"] = attributes;
                }

                return result;
            }
            else if (node.Attributes != null && node.Attributes.Count > 0)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                Dictionary<string, string> attributes = new Dictionary<string, string>();
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    attributes[attribute.Name] = attribute.Value;
                }
                result["@attributes"] = attributes;
                return result;
            }
            else
            {
                return node.InnerText.Trim();
            }
        }


        
    }
}
