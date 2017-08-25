using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CodeSmithTemplate.AspNet.AssemblyFile
{
    /// <remarks/>
    [SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "doc")]
    public partial class CSComment
    {
        /// <remarks/>
        public docAssembly assembly { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("member", IsNullable = false)]
        public docMember[] members { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class docAssembly
    {
        /// <remarks/>
        public string name { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class docMember
    {
        /// <remarks/>
        [XmlElement("inheritdoc", typeof(object))]
        [XmlElement("param", typeof(docMemberParam))]
        [XmlElement("returns", typeof(string))]
        [XmlElement("summary", typeof(string))]
        [XmlElement("typeparam", typeof(docMemberTypeparam))]
        [XmlChoiceIdentifierAttribute("ItemsElementName")]
        public object[] Items { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class docMemberParam
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class docMemberTypeparam
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name { get; set; }
    }

    /// <remarks/>
    [Serializable()]
    [XmlType(IncludeInSchema = false)]
    public enum ItemsChoiceType
    {
        /// <remarks/>
        inheritdoc,

        /// <remarks/>
        param,

        /// <remarks/>
        returns,

        /// <remarks/>
        summary,

        /// <remarks/>
        typeparam,
    }
}
