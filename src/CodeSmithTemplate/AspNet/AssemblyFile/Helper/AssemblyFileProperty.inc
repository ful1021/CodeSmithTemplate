<script runat="template">
public System.Collections.Generic.Dictionary<string, string> AnalyticGetColumns(string docXmlFile, string namespaceName, string className, bool isFirstLetterCamel = true)
{
    string prefix = namespaceName + "." + className;
    System.Collections.Generic.Dictionary<string, string>
        dict = new System.Collections.Generic.Dictionary<string, string>();
    CSComment doc = LoadDocXmlFile(docXmlFile);
    if (doc == null)
    {
        return dict;
    }
    var list = doc.members.Where(a => a.name.StartsWith(prefix)).ToList();
    foreach (var item in list)
    {
        var name = item.name.Substring(prefix.Length + 1);
        var text = item.Items.FirstOrDefault().ToString().Trim();
        if (isFirstLetterCamel)
        {
            name = ToFirstLetterCamel(name);
        }
        dict[name] = text;
    }
    return dict;
}

private CSComment LoadDocXmlFile(string docXmlFile)
{
    if (string.IsNullOrWhiteSpace(docXmlFile))
    {
        return null;
    }
    var xml = System.IO.File.ReadAllText(docXmlFile);
    CSComment doc = null;
    using (System.IO.StringReader sr = new System.IO.StringReader(xml))
    {
        System.Xml.Serialization.XmlSerializer xmldes = new System.Xml.Serialization.XmlSerializer(typeof(CSComment));
        doc = xmldes.Deserialize(sr) as CSComment;
    }
    return doc;
}
</script>