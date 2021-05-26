//Description: Contains General Functions for ease of usage
//	eg. File Read/Write sequence into 1 function

using Godot;
using System;
using System.Text.RegularExpressions;
using Godot.Collections;
using GA = Godot.Collections.Array;

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
//		Change the image
		Image newImage = new Image();
//		Load the image filepath
		newImage.Load(ProjectSettings.GlobalizePath(path));
//		Generate a texture based on the Image
		ImageTexture imgText = new ImageTexture();
		imgText.CreateFromImage(newImage);
//		Returns the texture
		return imgText;
	}
	
//*********************************************************************************************
}
