using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tabalog
{
	public static class TabalogUnpacker
	{
		static int infLoopCap = 512;

		public static Dictionary<string, string> Unpack(string text)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();

			if (text == null)
			{
				Debug.LogError("[Unpacker] : File or Text is null");
				return data;
			}
			if (text == "")
			{
				Debug.LogError("[Unpacker] : File or Text is empty");
				return data;
			}

			List<Token> tokens = Tokenizer(text);
			tokens = tokens.CondenseTabs();

			if (tokens.Count == 0)
			{
				Debug.LogError("[Unpacker] : No tokens found");
				return data;
			}

			int LoopCount = 0;

			List<(string, int)> context = new List<(string, int)>();
			context.Add(("", 0));

            while(tokens.Count > 0)
			{
				// Debug.Log($"Token Type: {tokens[0].Type}\tToken Value: {tokens[0].Value}\t\tTokens left: {tokens.Count}");
				int TokensRead = 1;

                string pre = "";
				int tabs = 0;
                if(tokens[0].Type == TokenType.Tab)
				{
					tabs = int.Parse(tokens[0].Value);
					int last = context.FindLastIndex(x => x.Item2 == tabs - 1);

					pre = context[last].Item1 + "/";
					tokens.RemoveAt(0);
				}

				if (tokens[0].Type == TokenType.Word)
				{
					string key = pre + tokens[0].Value;
                    string value = "";

                    if (tokens.Count > 2 && tokens[1].Type == TokenType.Is)
					{
                        value = tokens[2].Value;
                        TokensRead += 2;
					}

					if (!data.ContainsKey(key))
					{
						data.Add(key, value);
					}
						context.Add((key, tabs));
				}

				if (tokens[0].Type == TokenType.Is)
				{
					Debug.LogError($"Token #{LoopCount} Is Token Detected where it shouldn't be");
				}

				// Remove the tokens that were processed
				if (TokensRead > 0)
				{
					if(TokensRead > tokens.Count)
					{
						Debug.LogError($"[Unpacker] TokensRead({TokensRead}) > tokens.Count({tokens.Count})");
					}
					
					tokens.RemoveRange(0, TokensRead);
				}

				LoopCount++;
				if (LoopCount > infLoopCap)
				{
					Debug.LogError($"[Unpacker] Infinite Loop Cap reached ({infLoopCap})");
					return data;
				}

				// Debug.Log("");
            }

			return data;
        }

        static List<Token> Tokenizer(string text)
		{
			List<Token> tokens = new List<Token>();

			while (text.Length > 0)
			{
				List<(TokenType, Match)> matches = new List<(TokenType, Match)>();
				foreach (TokenType type in TokenLookup.Keys)
				{
					Match m = Regex.Match(text, TokenLookup[type]);
					if (m.Success) matches.Add((type, m));
				}

				if (matches.Count == 0)
				{
					Debug.LogError($"No match found for \"{text}\"");
					break;
				}

				matches = matches.OrderBy(m => m.Item2.Index).ToList();
				var match = matches[0];
				
				// Debug.Log($"Token Type: {match.Item1}\tToken Value: {match.Item2.Value}");
				tokens.Add(new Token(match.Item1, match.Item2.Value));

				text = text.Remove(0, match.Item2.Index + match.Item2.Length);
			}

			return tokens;
		}

		static List<Token> CondenseTabs(this List<Token> tokens)
		{
			List<Token> returnTokens = new List<Token>();

			int tabCount = 0;
			foreach (Token token in tokens)
			{
				if (token.Type == TokenType.Tab)
				{
					tabCount++;
				}
				else
				{
					if (tabCount > 0)
					{
						returnTokens.Add(new Token(TokenType.Tab, tabCount.ToString()));
						tabCount = 0;
					}
					returnTokens.Add(token);
				}
			}

			return returnTokens;
		}

		static Dictionary<TokenType, string> TokenLookup => new Dictionary<TokenType, string>
		{
			{ TokenType.Word, @"\b((\S+)( \w)?)+" },
			{ TokenType.Is, @"[:=]" },
			{ TokenType.Tab, @"\t" },
		};

        enum TokenType
        {
            Word,
            Is,
            Tab,
        }

        struct Token
        {
            public TokenType Type;
			#nullable enable
            public string Value;

            public Token(TokenType type, string? value = null)
            {
                Type = type;
                Value = value ?? "";
            }
			#nullable disable
        }
	}

	public static class TabalogPacker
	{
        public static string Pack(Dictionary<string, string> Dict)
		{
            if (Dict == null)
            {
                Debug.LogError("[Packer] : Dictionary is null");
                return null;
            }

            var list = Dict.ToList();
			list.Sort((x, y) => x.Key.CompareTo(y.Key));
            Dict = list.ToDictionary(k => k.Key, v => v.Value);

			string text = "";

			for (int i = 0; i < Dict.Count; i++)
			{
				string key = Dict.Keys.ElementAt(i);
				string value = Dict.Values.ElementAt(i);
				List<string> split = key.Split('/').ToList();
				
				string tabString = "";
				for (int j = 1; j < split.Count; j++)
				{
					tabString += "\t";
				}

				if (value == null || value == "")
					text += $"{tabString + split.Last()}";
				else
					text += $"{tabString + split.Last()} : {value}";

				if (i < Dict.Count - 1) text += "\n";
			}

			return text;
		}

		public static string DataToString(List<data> Data, int tabs = 0)
		{
			string text = "";

			for (int i = 0; i < Data.Count; i++)
			{
				data d = Data[i];

				string tabString = "";
				for (int j = 0; j < d.tabs; j++)
				{
					tabString += "\t";
				}

				text += $"{tabString + d.Key} : {d.Value}";

				if (i < Data.Count - 1) text += "\n";
			}

			return text;
        }

		public struct data
		{
			#nullable enable
			public string Key;
            public string? Value;
			public List<data> Children;

            public int tabs;

            public data(string key, string? value = null, int tabs = 0, List<data>? children = null)
			{
				this.Key = key;
				this.Value = value;
				this.tabs = tabs;
				this.Children = children ?? new List<data>();
			}
			#nullable disable
		}
	}

	public static class TabalogUtility
	{
		public static bool IsValid(this string text)
		{
			// return !(string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text));
			bool isValid = text != null && text != "";

			return isValid;
		}
	}
}