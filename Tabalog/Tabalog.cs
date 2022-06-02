using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tabalog
{
	public static class Unpacker
	{
		public static Dictionary<string, string> Unpack(string text)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();

			List<Token> tokens = Tokenizer(text);
			tokens = tokens.CondenseTabs();

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

                    if (tokens[1].Type == TokenType.Is)
					{
                        value = tokens[2].Value;
                        TokensRead += 2;
					}

					data.Add(key, value);
					context.Add((key, tabs));
				}

				if (tokens[0].Type == TokenType.Is)
				{
					Debug.LogError($"Token #{LoopCount} Is Token Detected where it shouldn't be");
				}

				// Remove the tokens that were processed
				tokens.RemoveRange(0, TokensRead);

				LoopCount++;
				if(LoopCount > 256)
				{
					Debug.LogError("Infinite Loop Detected");
					break;
				}

				Debug.Log("");
            }

			return data;
        }

		public static void Test(string text, TokenType type)
        {
            // Print match word
            MatchCollection matches = Regex.Matches(text, TokenLookup[type]);
			string output = "";
			foreach (Match match in matches)
			{
				output += $"\"{match.Value}\"\n";
			}
			Debug.Log(output);
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
	}

	public enum TokenType
	{
		Word,
		Is,
		Tab,
	}

	public struct Token
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
