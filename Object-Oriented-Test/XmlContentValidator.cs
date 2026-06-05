using System.Xml;

namespace RepositoryManager;

public sealed class XmlContentValidator : IContentValidator
{
    public bool Validate(string content)
    {
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(content);
            return true;
        }
        catch (XmlException)
        {
            return false;
        }
    }
}
