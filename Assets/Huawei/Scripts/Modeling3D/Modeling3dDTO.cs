using System;

[Serializable]
public class Modeling3dDTO
{
    public string TaskId;
    public string Status;
    public int Type; //1-Upload, 2-Download, 3-Preview
    public string Name;
    public string CoverImagePath;
    public string UploadFilePath;
    public string DownloadFilePath;

    public override string ToString()
    {
        return $"TaskId: {TaskId} Status: {Status} Type: {Type} Name: {Name}";
    }

    public override bool Equals(object obj)
    {
        if (obj is Modeling3dDTO)
        {
            return TaskId == (obj as Modeling3dDTO).TaskId;
        }
        if(obj is string)
        {
            return TaskId == (string)obj;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}