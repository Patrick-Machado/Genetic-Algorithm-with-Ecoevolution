using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
public class HandleTextFile
{
    public static string Data;
    [MenuItem("Tools/Write file")]
    public static void WriteString(string value, string fileId, bool lineEnter)
    {
        string path = "Assets/Resources/data_"+fileId+".txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        if (lineEnter) { writer.WriteLine(value); }
        else { writer.Write(value); }
        writer.Close();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(path);
        TextAsset asset = Resources.Load<TextAsset>("data_"+fileId);

        //Print the text from the file
        //Debug.Log(asset.text);
    }

    [MenuItem("Tools/Read file")]
    public static void ReadString(string fileID)
    {
        string path = "Assets/Resources/data_"+ fileID + ".txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        Data = (reader.ReadToEnd());
        reader.Close();
    }

    string genToWrite = "Generation: _";//[12] to write the number
    public static string DetectGenerationLine( string fileID)
    {

        string path = "Assets/Resources/data_" + fileID + ".txt";

        if (!File.Exists(path))
        { return "0"; }
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        bool noNeedToReadFurther = false;
        bool found = false;
        foreach (var line in File.ReadLines(path).Reverse())
        {
            if (found)
            {
                noNeedToReadFurther = true;
                return line;
            }
            if (noNeedToReadFurther)
                break;

            // process line here
            if (line[0] == 'G')
            {
                noNeedToReadFurther = true;
                found = true;
            }
        }
        
        Debug.Log("An ERROR ocurred");
        return "0";
    }
    public static void ClearAux(string AuxPath)
    {
        string path = "Assets/Resources/data_" + AuxPath + ".txt";

        if (!File.Exists(path))
        { return ; }
        File.Delete(path);
    }
}
