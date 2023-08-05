using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Xml;
using XMLTAgsExtractor.Models;

namespace XMLTAgsExtractor.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class XMLTagExtractorController : ControllerBase
    {
        ApiResponse _response = new ApiResponse();

        [HttpPost]
        public ActionResult<ApiResponse> ExtractTags(ExtractionRequest extractionRequest)
        {
            if (extractionRequest == null || string.IsNullOrEmpty(extractionRequest.FileUrl))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("File Url field cannot be empty");
                return _response;
            }
            bool isAvailable = VerifyFileUrl(extractionRequest.FileUrl);
            if (!isAvailable)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("File Url is invalid");
                return _response;
            }
            bool isXML = CheckFileExtension(extractionRequest.FileUrl);
            if (!isXML)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("File extension must be .xml");
                return _response;
            }
            List<string> extractedTags = ExtractTags(extractionRequest.FileUrl);

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = extractedTags;
            return _response;
        }


        private List<string> ExtractTags(string fileUrl)
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

        void TraverseNode(XmlNode node, List<string> tagsList)
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

        private static byte[] ReadAllBytes(string fileUrl)
        {
            return System.IO.File.ReadAllBytes(fileUrl);
        }
        private bool VerifyFileUrl(string fileUrl)
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
        private bool CheckFileExtension(string fileUrl)
        {
            bool isXML = false;

            string ext = fileUrl.Substring(fileUrl.Length - 3);

            if (ext == "xml")
            {
                isXML = true;
            }
            return isXML;
        }
        
    [HttpPost]
    public ActionResult<ApiResponse> ExtractTagsWithData(ExtractionRequest extractionRequest)
    {
            bool isAvailable = VerifyFileUrl(extractionRequest.FileUrl);
            if (!isAvailable)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("File Url is invalid");
                return _response;
            }
            bool isXML = CheckFileExtension(extractionRequest.FileUrl);
            if (!isXML)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("File extension must be .xml");
                return _response;
            }

            Dictionary<string, object> extractedData = ExtractTags2(extractionRequest.FileUrl);
        _response.IsSuccess = true;
        _response.StatusCode = HttpStatusCode.OK;
        _response.Result = extractedData;
        return _response;
    }

    private Dictionary<string, object> ExtractTags2(string fileUrl)
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

            //return TraverseNode(xmlDocument.DocumentElement);
    }

        //private Dictionary<string, object> TraverseNode(XmlNode node)
        //{
        //    Dictionary<string, object> result = new Dictionary<string, object>();

        //    if (node.NodeType == XmlNodeType.Element)
        //    {
        //        Dictionary<string, object> attributes = new Dictionary<string, object>();
        //        foreach (XmlAttribute attribute in node.Attributes)
        //        {
        //            attributes[attribute.Name] = attribute.Value;
        //        }

        //        List<Dictionary<string, object>> children = new List<Dictionary<string, object>>();
        //        foreach (XmlNode childNode in node.ChildNodes)
        //        {
        //            if (childNode.NodeType == XmlNodeType.Element)
        //            {
        //                children.Add(TraverseNode(childNode));
        //            }
        //        }

        //        if (children.Any())
        //        {
        //            result[node.Name] = children;
        //        }
        //        else if (attributes.Any())
        //        {
        //            result[node.Name] = attributes;
        //        }
        //        else
        //        {
        //            result[node.Name] = node.InnerText;
        //        }
        //    }

        //    return result;
        //}
        //private Dictionary<string, object> TraverseNode(XmlNode node)
        //{
        //    Dictionary<string, object> result = new Dictionary<string, object>();

        //    if (node.NodeType == XmlNodeType.Element)
        //    {
        //        List<Dictionary<string, object>> children = new List<Dictionary<string, object>>();
        //        Dictionary<string, object> attributes = new Dictionary<string, object>();

        //        foreach (XmlNode childNode in node.ChildNodes)
        //        {
        //            if (childNode.NodeType == XmlNodeType.Element)
        //            {
        //                children.Add(TraverseNode(childNode));
        //            }
        //        }

        //        if (node.HasAttributes)
        //        {
        //            foreach (XmlAttribute attribute in node.Attributes)
        //            {
        //                attributes[attribute.Name] = attribute.Value;
        //            }
        //        }

        //        if (children.Any() || attributes.Any())
        //        {
        //            result[node.Name] = children.Any() ? children : attributes;
        //        }
        //        else
        //        {
        //            result[node.Name] = node.InnerText;
        //        }
        //    }

        //    return result;
        //}
        private Dictionary<string, object> TraverseNode(XmlNode node)
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


    }
}