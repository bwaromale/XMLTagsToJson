using System.Xml;

namespace XMLTAgsExtractor.Services
{
    public interface IExtraction
    {
        List<string> ExtractTags(string fileUrl);
        Dictionary<string, object> ExtractTagsandTagContent(string fileUrl);
        bool VerifyFileUrl(string fileUrl);
        bool CheckFileExtension(string fileUrl);
        
    }
}
