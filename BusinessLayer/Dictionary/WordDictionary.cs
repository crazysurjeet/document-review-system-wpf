using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Configuration;

namespace BusinessLayer.Dictionary
{
	public class WordDictionary : System.ComponentModel.Component
	{
		private Hashtable _baseWords = new Hashtable();
		private string _dictionaryFile = "english.dict";
		private string _dictionaryFolder = "Dictionary";
		private ArrayList _possibleBaseWords = new ArrayList();
		private Hashtable _userWords = new Hashtable();
		private System.ComponentModel.Container components = null;
		public readonly string ERROR_COLOR = ConfigurationManager.AppSettings["ERROR_COLOR"];

		public WordDictionary()
		{
			InitializeComponent();
		}


		public StringBuilder MisspelledWords(StringBuilder content, out long count)
		{
			count = 0;
			var temp = content.ToString().Split(' ', '.',',',':','&','\n','\r').ToList();
			HashSet<StringBuilder> misspelledwords = new HashSet<StringBuilder>();
			for (int i = 0; i < temp.Count; i++)
			{
				if (!string.IsNullOrEmpty(temp[i].Trim()))
					if (!_baseWords.Contains(temp[i].ToLower()))
					{
						if (!_baseWords.Contains(Clean(temp[i]).ToLower()))
						{
							if (!_baseWords.Contains(CleanSub1(temp[i]).ToLower()))
							{
								if (!_baseWords.Contains(CleanSub2(temp[i]).ToLower()))
								{
									misspelledwords.Add(new StringBuilder(temp[i]));
									temp.Insert(i, $"[size=20][url=][b][color={ERROR_COLOR}]");
									temp.Insert(i + 2, "[/color][/b][/url][/size]");
									count++;
									i += 2;
								}
							}
						}
					}
			}
			return new StringBuilder(string.Join(" ",temp));
		}
		public string Clean(string data)
		{
			data = data.Trim();
			if (data.Length > 0)
				if (char.ToLowerInvariant((data[data.Length - 1])) == 's') data = data.Remove(data.Length - 1, 1);
			if (data.Length > 0)
				if (!char.IsLetter(data[0])) data = data.Remove(0, 1);
			if(data.Length>0)
				if (!char.IsLetter(data[data.Length - 1])) data = data.Remove(data.Length - 1, 1);
			if (data.Length > 3)
				if (data.Substring(data.Length - 3, 3).ToLowerInvariant() == "ing") { 
					data = data.Remove(data.Length - 3, 3);
					if (data.Length > 1)
						data = data.Insert(data.Length, "e");
				}
			if (data.Length > 2)
				if (data.Substring(data.Length - 2, 2).ToLowerInvariant() == "ly") data = data.Remove(data.Length - 2, 2);
			if (data.Length > 2)
				if (data.Substring(data.Length - 2, 2).ToLowerInvariant() == "ed") data = data.Remove(data.Length - 2, 2);
			return data;
		}
		public string CleanSub1(string data)
		{
			data = data.Trim();
			if (data.Length > 3)
				if (data.Substring(data.Length - 3, 3).ToLowerInvariant() == "ing")
				{
					data = data.Remove(data.Length - 3, 3);
				}
			return data;
		}
		public string CleanSub2(string data)
		{
			data = data.Trim();
			if (data.Length > 1)
				if (data.Substring(data.Length - 1, 1).ToLowerInvariant() == "d") data = data.Remove(data.Length - 1, 1);
			if (data.Length > 3)
				if (data.Substring(data.Length - 3, 3).ToLowerInvariant() == "ies")
				{
					data = data.Remove(data.Length - 3, 3);
					if (data.Length > 1)
						data = data.Insert(data.Length, "y");
				}
			return data;
		}



		/// <summary>
		///     Initializes the dictionary by loading the dictionary file.
		/// </summary>
		public void Initialize()
		{
			// clean up data first
			_baseWords.Clear();

			string currentSection = "";
			string dictionaryPath = Path.Combine(_dictionaryFolder, _dictionaryFile);

			// open dictionary file
			FileStream fs = new FileStream(dictionaryPath, FileMode.Open, FileAccess.Read, FileShare.Read);
			StreamReader sr = new StreamReader(fs, Encoding.UTF8);

			// read line by line
			while (sr.Peek() >= 0)
			{
				string tempLine = sr.ReadLine().Trim();
				if (tempLine.Length > 0)
				{
					switch (tempLine)
					{
						case "[words]":
							currentSection = tempLine;
							break;
						default:
							switch (currentSection)
							{
								case "[words]": 
									string[] parts = tempLine.Split('/');
									Word tempWord = new Word();
									tempWord.Text = tempLine;
										if(!BaseWords.ContainsKey(tempWord.Text))
											this.BaseWords.Add(tempWord.Text, tempWord);
									break;
							} 
							break;
					} 
				} 
			} 
			// close file handles
			sr.Close();
			fs.Close();
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		public Hashtable BaseWords
		{
			get { return _baseWords; }
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
	}
}
