using System.Text;
using System.Xml;

namespace XMLTAgsExtractor.Services
{
    public class Extraction : IExtraction
    {
        public bool CheckFileExtension(string fileUrl)
        {
            bool isXML = false;

            string ext = fileUrl.Substring(fileUrl.Length - 3);

            if (ext == "xml")
            {
                isXML = true;
            }
            return isXML;
        }

        public List<string> ExtractTags(string fileUrl)
        {
            XmlDocument xmlDocument = new XmlDocument();
            byte[] xmlBytes = ReadAllBytes(fileUrl);
            string xmlContent = Encoding.UTF8.GetString(xmlBytes).TrimStart('\uFEFF');
            if (string.IsNullOrEmpty(xmlContent))
            {
                return new List<string>();
            }
            xmlDocument.LoadXml(xmlContent);
            List<string> tagsStartingWithAngleBracket = new List<string>();
            if (xmlDocument.HasChildNodes)
            {
                foreach (XmlNode node in xmlDocument.ChildNodes)
                {
                    TraverseNode(node, tagsStartingWithAngleBracket);
                }
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
            var rootNode = new Dictionary<string, object>();
            rootNode[xmlDocument.DocumentElement.Name] = TraverseNode(xmlDocument.DocumentElement);
            return rootNode;
        }

        public byte[] ReadAllBytes(string fileUrl)
        {
            return System.IO.File.ReadAllBytes(fileUrl);
        }

        public void TraverseNode(XmlNode node, List<string> tagsList)
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

        public Dictionary<string, object> TraverseNode(XmlNode node)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            if (node.NodeType == XmlNodeType.Element)
            {
                List<Dictionary<string, object>> children = new List<Dictionary<string, object>>();
                Dictionary<string, object> attributes = new Dictionary<string, object>();

                if (node is XmlElement xmlElement && xmlElement.HasAttributes)
                {
                    foreach (XmlAttribute attribute in xmlElement.Attributes)
                    {
                        attributes[attribute.Name] = attribute.Value;
                    }
                }

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        children.Add(TraverseNode(childNode));
                    }
                }

                if (children.Any() || attributes.Any())
                {
                    result[node.Name] = children.Any() ? children : attributes;
                }
                else
                {
                    result[node.Name] = node.InnerText;
                }
            }

            return result;
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
            catch (UriFormatException) { }
            return isAvailable;
        }
    }
}
