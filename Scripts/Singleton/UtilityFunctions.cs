//Description: Contains General Functions for ease of usage
//	eg. File Read/Write sequence into 1 function

using Godot;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Godot.Collections;
using YamlDotNet.RepresentationModel;

public class UtilityFunctions : Node
{

//*********************************************************************************************

//FILE HANDLING

//	Returns a Dictionary of the file's contents (Assuming the file is in JSON format)
	public static Dictionary readFile(String filePath)
	{
//		Open the File
		File fileLoader = new File();
		fileLoader.Open(filePath, File.ModeFlags.Read);
//		Get the contents
		Dictionary data = null;
		try
		{
//			Collect the text from the entire file and turn it into 1 line
			String contents = oneLine(fileLoader.GetAsText());
//			Parse the one line string into Dictionary
			data = (Dictionary)JSON.Parse(contents).Result;	
		} catch (Exception e)
		{
			GD.Print(e);
			GD.Print("File is empty, returning null");
		}

		fileLoader.Close();
		return data;
	}


	//Returns a somewhat dictionary from a YAML file
	public static YamlMappingNode readYAML(String filePath)
	{
//		Open the File
		File fileLoader = new File();
		fileLoader.Open(filePath, File.ModeFlags.Read);
//		Get the contents
		YamlMappingNode data = null;
		try
		{
//			Collect the text from the entire file and turn it into 1 line
			String contents = fileLoader.GetAsText();
//			Parse the one line string into Dictionary
			System.IO.StringReader sr = new System.IO.StringReader(contents);
			YamlStream stream = new YamlStream();
			stream.Load(sr);
			data = stream.Documents[0].RootNode as YamlMappingNode;
		} catch (Exception e)
		{
			GD.Print(e);
			GD.Print("File is empty, returning null");
		}

		fileLoader.Close();
		return data;
	}

	
//	Writes the data from the dictionary into the filePath, generates file, if file doesn't exist
	public static void writeFile(String filePath, Dictionary data)
	{
		File dataFile = new File();
		dataFile.Open(filePath, File.ModeFlags.Write);
//		Append Text to file
		dataFile.StoreLine(JSON.Print(data));
		dataFile.Close();
	}
	
	
	public static bool fileExists(String filePath)
	{
		File f = new File();
		return f.FileExists(filePath);
	}
	
//*********************************************************************************************

//STRING FUNCTIONS

//	Makes the entire string into 1 line
	public static String oneLine(String fullString)
	{
//		\n, \r & \r\n are newlines depending on OS
//		\t for tabs & " " for spaces
		String newString = fullString.Replace("\n", "")
							.Replace("\r", "")
							.Replace("\r\n", "")
							.Replace("\t", "");
		return newString;
	}
	
	
	//	Separates the text of Pascal Case
	public static String extractPascal(String pascal)
	{
//		REGEX, Searches for any Capital Letter, then adds space before it
		String output = Regex.Replace(pascal, "([A-Z])", " $1");
//		Trim the Right side
		output = output.StripEdges(true, false);
		return output;
	}

//*********************************************************************************************

//IMAGE FUNCTIONS

//	Returns the ImageTexture given a path of the string
	public static ImageTexture getTexture(String path)
	{
		StreamTexture newImg = ResourceLoader.Load(ProjectSettings.GlobalizePath(path)) as StreamTexture;
		Image newImage = new Image();
//		Load the image filepath
		newImage = newImg.GetData();
//		Generate a texture based on the Image
		ImageTexture imgText = new ImageTexture();
		imgText.CreateFromImage(newImage);
//		Returns the texture
		return imgText;
	}
	
//*********************************************************************************************

//CLASS HANDLING becuz godotsharp is down

	public static void showMethods(Type type, bool toFile=false)
	{
		int i = 0;
		Dictionary methods = new Dictionary();
		foreach (var method in type.GetMethods())
		{
			var parameters = method.GetParameters();
			var parameterDescriptions = string.Join
				(", ", method.GetParameters()
							 .Select(x => x.ParameterType + " " + x.Name)
							 .ToArray());

			String combined = method.ReturnType + " " +
					 method.Name + "(" +
					 parameterDescriptions + ")";
			i++;
			if(toFile)
			{
				methods.Add(i + " " + method.Name, combined);
				continue;
			}
			GD.Print(combined);
		}
		
		writeFile("user://methodAsked.txt", methods);
	}
	
	
	public static void showFields(Type type)
	{
		foreach (var field in type.GetFields())
		{
			GD.Print(field.FieldType + " " +
					 field.Name);
		}
	}
}
