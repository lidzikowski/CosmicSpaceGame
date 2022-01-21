using System;

[Serializable]
public class FileClass
{
    public string name;
    public string path;
    //public string sha;
    public long size;
    public string url;
    //public string html_url;
    //public string git_url;
    public string download_url;
    public string type;
    public FileLink _links;
}

[Serializable]
public class FileLink
{
    public string self;
    //public string git;
    //public string html;
}

[Serializable]
public class FileDecoder
{
    public FileClass[] result;
}

[Serializable]
public class RepositoryState
{
    public FileClass File;
    public float Progress;
    public bool Ready;
}