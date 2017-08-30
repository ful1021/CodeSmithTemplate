<script runat="template">
public static System.Collections.Generic.Dictionary<System.String, System.String> AnalyticGetColumns(string dllFile, string className, bool isFirstLetterCamel = true)
{
    var props = GetProperties(dllFile, className);
    System.Collections.Generic.Dictionary<System.String, System.String>
dict = new System.Collections.Generic.Dictionary<System.String, System.String>();
    foreach (var item in props)
    {
        var name = item.Name;
        var textInfo = blqw.Reflection.CSCommentReader.Create(item);
        var text = textInfo != null ? textInfo.Summary : "";
        if (name == "CreationTime")
        {
            text = "创建时间";
        }
        else if (name == "CreatorUserId")
        {
            text = "创建人";
        }
        else if (name == "LastModificationTime")
        {
            text = "上次修改时间";
        }
        else if (name == "LastModifierUserId")
        {
            text = "上次修改人";
        }
        if (isFirstLetterCamel)
        {
            name = ToFirstLetterCamel(name);
        }
        dict[name] = text;
    }
    return dict;
}
</script>