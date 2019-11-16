using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;

public class SaveAndLoad : MonoBehaviour {

    static public List<int> Read(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        List<int> fileContent = new List<int>();
        StreamReader sr = new StreamReader(path);
        
        string line = sr.ReadLine();
        while (line != null)
        {
            int temp;
            int.TryParse(line,out temp);
            fileContent.Add(temp);
            line = sr.ReadLine();
        }

        sr.Close();
        return fileContent;
    }

    static public void Write(ref List<int> st, string fileName)
    {
        string result = "";
        foreach(int a in st)
            result += a.ToString() + "\n";

        File.WriteAllText(Application.persistentDataPath + "/" + fileName, result);
    }

    /// <summary>
    /// Si no existe el archivo 
    /// </summary>
    static public void Initialize(List<int> st, string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        if (!File.Exists(path))
        {
            StreamWriter sw = File.CreateText(path);

            foreach (int item in st)
                sw.WriteLine(item.ToString());
            
            sw.Close();
        }
    }
}