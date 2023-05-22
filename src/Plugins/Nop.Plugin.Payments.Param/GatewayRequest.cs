using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Payments.Param
{

    public class GatewayRequest
    {
        private string _sanalPosId = "";
        private string _clientCode = "";
        private string _clientUsername = "";
        private string _clientPassword = "";
        private string _guid;
        private string _kK_Sahibi = "";
        private string _kK_No = "";
        private string _kK_SK_Ay = "01";
        private string _kK_SK_Yil = "0000";
        private string _kK_CVC = "";
        private string _kK_Sahibi_GSM = "";
        private string _hata_URL = "";
        private string _basarili_URL = "";
        private string _siparis_ID = "";
        private string _siparis_Aciklama = "";
        private int _taksit = 1;
        private string _islem_Tutar = "";
        private string _toplam_Tutar = "";
        private string _islem_Hash = "";
        private string _islem_Guvenlik_Tip = "";
        private string _iPAdr = "";
        private string _ref_URL = "";
        private string _islem_ID = "";
        private string _data1 = "";
        private string _data2 = "";
        private string _data3 = "";
        private string _data4 = "";
        private string _data5 = "";
        private string _data6 = "";
        private string _data7 = "";
        private string _data8 = "";
        private string _data9 = "";
        private string _data10 = "";


        public string SanalPosId { get => _sanalPosId; set => _sanalPosId = value; }
        public string ClientCode { get => _clientCode; set => _clientCode = value; }
        public string ClientUsername { get => _clientUsername; set => _clientUsername = value; }
        public string ClientPassword { get => _clientPassword; set => _clientPassword = value; }
        public string Guid { get => _guid; set => _guid = value; }
        public string KK_Sahibi { get => _kK_Sahibi; set => _kK_Sahibi = value; }
        public string KK_No { get => _kK_No; set => _kK_No = value; }
        public string KK_SK_Ay { get => _kK_SK_Ay; set => _kK_SK_Ay = value; }
        public string KK_SK_Yil { get => _kK_SK_Yil; set => _kK_SK_Yil = value; }
        public string KK_CVC { get => _kK_CVC; set => _kK_CVC = value; }
        public string KK_Sahibi_GSM { get => _kK_Sahibi_GSM; set => _kK_Sahibi_GSM = value; }
        public string Hata_URL { get => _hata_URL; set => _hata_URL = value; }
        public string Basarili_URL { get => _basarili_URL; set => _basarili_URL = value; }
        public string Siparis_ID { get => _siparis_ID; set => _siparis_ID = value; }
        public string Siparis_Aciklama { get => _siparis_Aciklama; set => _siparis_Aciklama = value; }
        public int Taksit { get => _taksit; set => _taksit = value; }
        public string Islem_Tutar { get => _islem_Tutar; set => _islem_Tutar = value; }
        public string Toplam_Tutar { get => _toplam_Tutar; set => _toplam_Tutar = value; }
        public string Islem_Hash { get => _islem_Hash; set => _islem_Hash = value; }
        public string Islem_Guvenlik_Tip { get => _islem_Guvenlik_Tip; set => _islem_Guvenlik_Tip = value; }
        public string IPAdr { get => _iPAdr; set => _iPAdr = value; }
        public string Ref_URL { get => _ref_URL; set => _ref_URL = value; }
        public string Islem_ID { get => _islem_ID; set => _islem_ID = value; }
        public string Data1 { get => _data1; set => _data1 = value; }
        public string Data2 { get => _data2; set => _data2 = value; }
        public string Data3 { get => _data3; set => _data3 = value; }
        public string Data4 { get => _data4; set => _data4 = value; }
        public string Data5 { get => _data5; set => _data5 = value; }
        public string Data6 { get => _data6; set => _data6 = value; }
        public string Data7 { get => _data7; set => _data7 = value; }
        public string Data8 { get => _data8; set => _data8 = value; }
        public string Data9 { get => _data9; set => _data9 = value; }
        public string Data10 { get => _data10; set => _data10 = value; }


        //public string ToXml()
        //{
        //    // We don't really need the overhead of creating an XML DOM object
        //    // to really just concatenate a string together.
        //    var xml = "";
        //    xml += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        //    xml += "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">";
        //    xml += "<soap:Body>";
        //    xml += "<Pos_Odeme xmlns=\"https://turkpos.com.tr/\">";
        //    xml += "<G>";
        //    xml += CreateNode("CLIENT_CODE", XmlHelper.XmlEncodeAsync(_clientCode));
        //    xml += CreateNode("CLIENT_USERNAME", XmlHelper.XmlEncodeAsync(_clientUsername));
        //    xml += CreateNode("CLIENT_PASSWORD", XmlHelper.XmlEncodeAsync(_clientPassword));
        //    xml += "</G>";
        //    xml += CreateNode("SanalPOS_ID", XmlHelper.XmlEncodeAsync(_sanalPosId));
        //    xml += CreateNode("SanalPosId", XmlHelper.XmlEncodeAsync(_sanalPosId));
        //    xml += CreateNode("ClientCode", XmlHelper.XmlEncodeAsync(_clientCode));
        //    xml += CreateNode("ClientUsername", XmlHelper.XmlEncodeAsync(_clientUsername));
        //    xml += CreateNode("ClientPassword", XmlHelper.XmlEncodeAsync(_clientPassword));
        //    xml += CreateNode("Guid", XmlHelper.XmlEncodeAsync(_guid));
        //    xml += CreateNode("KK_Sahibi", XmlHelper.XmlEncodeAsync(_kK_Sahibi));
        //    xml += CreateNode("KK_No", XmlHelper.XmlEncodeAsync(_kK_No));
        //    xml += CreateNode("KK_SK_Ay", XmlHelper.XmlEncodeAsync(_kK_SK_Ay));
        //    xml += CreateNode("KK_SK_Yil", XmlHelper.XmlEncodeAsync(_kK_SK_Yil));
        //    xml += CreateNode("KK_CVC", XmlHelper.XmlEncodeAsync(_kK_CVC));
        //    xml += CreateNode("KK_Sahibi_GSM", XmlHelper.XmlEncodeAsync(_kK_Sahibi_GSM));
        //    xml += CreateNode("Hata_URL", XmlHelper.XmlEncodeAsync(_hata_URL));
        //    xml += CreateNode("Basarili_URL", XmlHelper.XmlEncodeAsync(_basarili_URL));
        //    xml += CreateNode("Siparis_ID", XmlHelper.XmlEncodeAsync(_siparis_ID));
        //    xml += CreateNode("Siparis_Aciklama", XmlHelper.XmlEncodeAsync(_siparis_Aciklama));
        //    xml += CreateNode("Taksit", XmlHelper.XmlEncodeAsync(_taksit.ToString()));
        //    xml += CreateNode("Islem_Tutar", XmlHelper.XmlEncodeAsync(_islem_Tutar));
        //    xml += CreateNode("Toplam_Tutar", XmlHelper.XmlEncodeAsync(_toplam_Tutar));
        //    xml += CreateNode("Islem_Hash", XmlHelper.XmlEncodeAsync(_islem_Hash));
        //    xml += CreateNode("Islem_Guvenlik_Tip", XmlHelper.XmlEncodeAsync(_islem_Guvenlik_Tip));
        //    xml += CreateNode("IPAdr", XmlHelper.XmlEncodeAsync(_iPAdr));
        //    xml += CreateNode("Ref_URL", XmlHelper.XmlEncodeAsync(_ref_URL));
        //    xml += CreateNode("Islem_ID", XmlHelper.XmlEncodeAsync(_islem_ID));
        //    xml += CreateNode("Data1", XmlHelper.XmlEncodeAsync(_data1));
        //    xml += CreateNode("Data2", XmlHelper.XmlEncodeAsync(_data2));
        //    xml += CreateNode("Data3", XmlHelper.XmlEncodeAsync(_data3));
        //    xml += CreateNode("Data4", XmlHelper.XmlEncodeAsync(_data4));
        //    xml += CreateNode("Data5", XmlHelper.XmlEncodeAsync(_data5));
        //    xml += CreateNode("Data6", XmlHelper.XmlEncodeAsync(_data6));
        //    xml += CreateNode("Data7", XmlHelper.XmlEncodeAsync(_data7));
        //    xml += CreateNode("Data8", XmlHelper.XmlEncodeAsync(_data8));
        //    xml += CreateNode("Data9", XmlHelper.XmlEncodeAsync(_data9));
        //    xml += CreateNode("Data10", XmlHelper.XmlEncodeAsync(_data10));
        //    xml += "</Pos_Odeme>";
        //    xml += "</soap:Body>";
        //    xml += "</soap:Envelope>";

        //    return xml;
        //}

        private string CreateNode(string v, Task<string> task)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Builds a simple XML node.
        /// </summary>
        /// <param name="nodeName">The name of the node being created.</param>
        /// <param name="nodeValue">The value of the node being created.</param>
        /// <returns>An XML node as a string.</returns>
        private static string CreateNode(string nodeName, string nodeValue)
        {
            return "<" + nodeName + ">" + nodeValue + "</" + nodeName + ">";
        }
    }
}
