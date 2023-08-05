using System.Xml;

namespace XMLTAgsExtractor.Services
{
    public interface IExtraction
    {
        List<string> ExtractTags(string fileUrl);
        Dictionary<string, object> ExtractTagsandTagContent(string fileUrl);
        void TraverseNode(XmlNode node, List<string> tagsList);
        Dictionary<string, object> TraverseNode(XmlNode node);
        bool VerifyFileUrl(string fileUrl);
        bool CheckFileExtension(string fileUrl);
        byte[] ReadAllBytes(string fileUrl);
    }
}
