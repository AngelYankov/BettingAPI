using System.Xml;

namespace BettingAPI.Services
{
    public interface IDeserializeService
    {
        XmlDocument TransformXml();
    }
}