using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Dictionary
{
	public class Word
	{
		private string _text = "";

		public Word()
		{
		}
		public Word(string text)
		{
			_text = text;
		}

		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		public override string ToString()
		{
			return _text;
		}
	}
}
