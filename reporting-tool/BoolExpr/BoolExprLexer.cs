//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from /Users/philippgrigoryev/projects/reporting-tool/reporting-tool/BoolExpr/BoolExpr.g4 by ANTLR 4.7.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.2")]
[System.CLSCompliant(false)]
public partial class BoolExprLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, ATTR=11, STR=12, WS=13;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
		"T__9", "ATTR", "STR", "WS"
	};


	public BoolExprLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public BoolExprLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'('", "')'", "'not'", "'and'", "'or'", "'eq'", "'co'", "'sw'", 
		"'pr'", "'profile.'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, "ATTR", 
		"STR", "WS"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "BoolExpr.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static BoolExprLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x2', '\xF', 'T', '\b', '\x1', '\x4', '\x2', '\t', '\x2', '\x4', 
		'\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', 
		'\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', '\t', 
		'\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x4', '\v', '\t', 
		'\v', '\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', '\x4', '\xE', 
		'\t', '\xE', '\x3', '\x2', '\x3', '\x2', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', '\x5', '\x3', 
		'\x5', '\x3', '\x5', '\x3', '\x5', '\x3', '\x6', '\x3', '\x6', '\x3', 
		'\x6', '\x3', '\a', '\x3', '\a', '\x3', '\a', '\x3', '\b', '\x3', '\b', 
		'\x3', '\b', '\x3', '\t', '\x3', '\t', '\x3', '\t', '\x3', '\n', '\x3', 
		'\n', '\x3', '\n', '\x3', '\v', '\x3', '\v', '\x3', '\v', '\x3', '\v', 
		'\x3', '\v', '\x3', '\v', '\x3', '\v', '\x3', '\v', '\x3', '\v', '\x3', 
		'\f', '\x3', '\f', '\a', '\f', '\x44', '\n', '\f', '\f', '\f', '\xE', 
		'\f', 'G', '\v', '\f', '\x3', '\r', '\x3', '\r', '\x6', '\r', 'K', '\n', 
		'\r', '\r', '\r', '\xE', '\r', 'L', '\x3', '\r', '\x3', '\r', '\x3', '\xE', 
		'\x3', '\xE', '\x3', '\xE', '\x3', '\xE', '\x2', '\x2', '\xF', '\x3', 
		'\x3', '\x5', '\x4', '\a', '\x5', '\t', '\x6', '\v', '\a', '\r', '\b', 
		'\xF', '\t', '\x11', '\n', '\x13', '\v', '\x15', '\f', '\x17', '\r', '\x19', 
		'\xE', '\x1B', '\xF', '\x3', '\x2', '\x6', '\x4', '\x2', '\x43', '\\', 
		'\x63', '|', '\x6', '\x2', '\x32', ';', '\x43', '\\', '\x61', '\x61', 
		'\x63', '|', '\x3', '\x2', '$', '$', '\x4', '\x2', '\f', '\f', '\"', '\"', 
		'\x2', 'U', '\x2', '\x3', '\x3', '\x2', '\x2', '\x2', '\x2', '\x5', '\x3', 
		'\x2', '\x2', '\x2', '\x2', '\a', '\x3', '\x2', '\x2', '\x2', '\x2', '\t', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\v', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\r', '\x3', '\x2', '\x2', '\x2', '\x2', '\xF', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x11', '\x3', '\x2', '\x2', '\x2', '\x2', '\x13', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x15', '\x3', '\x2', '\x2', '\x2', '\x2', '\x17', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x19', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x1B', '\x3', '\x2', '\x2', '\x2', '\x3', '\x1D', '\x3', '\x2', 
		'\x2', '\x2', '\x5', '\x1F', '\x3', '\x2', '\x2', '\x2', '\a', '!', '\x3', 
		'\x2', '\x2', '\x2', '\t', '%', '\x3', '\x2', '\x2', '\x2', '\v', ')', 
		'\x3', '\x2', '\x2', '\x2', '\r', ',', '\x3', '\x2', '\x2', '\x2', '\xF', 
		'/', '\x3', '\x2', '\x2', '\x2', '\x11', '\x32', '\x3', '\x2', '\x2', 
		'\x2', '\x13', '\x35', '\x3', '\x2', '\x2', '\x2', '\x15', '\x38', '\x3', 
		'\x2', '\x2', '\x2', '\x17', '\x41', '\x3', '\x2', '\x2', '\x2', '\x19', 
		'H', '\x3', '\x2', '\x2', '\x2', '\x1B', 'P', '\x3', '\x2', '\x2', '\x2', 
		'\x1D', '\x1E', '\a', '*', '\x2', '\x2', '\x1E', '\x4', '\x3', '\x2', 
		'\x2', '\x2', '\x1F', ' ', '\a', '+', '\x2', '\x2', ' ', '\x6', '\x3', 
		'\x2', '\x2', '\x2', '!', '\"', '\a', 'p', '\x2', '\x2', '\"', '#', '\a', 
		'q', '\x2', '\x2', '#', '$', '\a', 'v', '\x2', '\x2', '$', '\b', '\x3', 
		'\x2', '\x2', '\x2', '%', '&', '\a', '\x63', '\x2', '\x2', '&', '\'', 
		'\a', 'p', '\x2', '\x2', '\'', '(', '\a', '\x66', '\x2', '\x2', '(', '\n', 
		'\x3', '\x2', '\x2', '\x2', ')', '*', '\a', 'q', '\x2', '\x2', '*', '+', 
		'\a', 't', '\x2', '\x2', '+', '\f', '\x3', '\x2', '\x2', '\x2', ',', '-', 
		'\a', 'g', '\x2', '\x2', '-', '.', '\a', 's', '\x2', '\x2', '.', '\xE', 
		'\x3', '\x2', '\x2', '\x2', '/', '\x30', '\a', '\x65', '\x2', '\x2', '\x30', 
		'\x31', '\a', 'q', '\x2', '\x2', '\x31', '\x10', '\x3', '\x2', '\x2', 
		'\x2', '\x32', '\x33', '\a', 'u', '\x2', '\x2', '\x33', '\x34', '\a', 
		'y', '\x2', '\x2', '\x34', '\x12', '\x3', '\x2', '\x2', '\x2', '\x35', 
		'\x36', '\a', 'r', '\x2', '\x2', '\x36', '\x37', '\a', 't', '\x2', '\x2', 
		'\x37', '\x14', '\x3', '\x2', '\x2', '\x2', '\x38', '\x39', '\a', 'r', 
		'\x2', '\x2', '\x39', ':', '\a', 't', '\x2', '\x2', ':', ';', '\a', 'q', 
		'\x2', '\x2', ';', '<', '\a', 'h', '\x2', '\x2', '<', '=', '\a', 'k', 
		'\x2', '\x2', '=', '>', '\a', 'n', '\x2', '\x2', '>', '?', '\a', 'g', 
		'\x2', '\x2', '?', '@', '\a', '\x30', '\x2', '\x2', '@', '\x16', '\x3', 
		'\x2', '\x2', '\x2', '\x41', '\x45', '\t', '\x2', '\x2', '\x2', '\x42', 
		'\x44', '\t', '\x3', '\x2', '\x2', '\x43', '\x42', '\x3', '\x2', '\x2', 
		'\x2', '\x44', 'G', '\x3', '\x2', '\x2', '\x2', '\x45', '\x43', '\x3', 
		'\x2', '\x2', '\x2', '\x45', '\x46', '\x3', '\x2', '\x2', '\x2', '\x46', 
		'\x18', '\x3', '\x2', '\x2', '\x2', 'G', '\x45', '\x3', '\x2', '\x2', 
		'\x2', 'H', 'J', '\a', '$', '\x2', '\x2', 'I', 'K', '\n', '\x4', '\x2', 
		'\x2', 'J', 'I', '\x3', '\x2', '\x2', '\x2', 'K', 'L', '\x3', '\x2', '\x2', 
		'\x2', 'L', 'J', '\x3', '\x2', '\x2', '\x2', 'L', 'M', '\x3', '\x2', '\x2', 
		'\x2', 'M', 'N', '\x3', '\x2', '\x2', '\x2', 'N', 'O', '\a', '$', '\x2', 
		'\x2', 'O', '\x1A', '\x3', '\x2', '\x2', '\x2', 'P', 'Q', '\t', '\x5', 
		'\x2', '\x2', 'Q', 'R', '\x3', '\x2', '\x2', '\x2', 'R', 'S', '\b', '\xE', 
		'\x2', '\x2', 'S', '\x1C', '\x3', '\x2', '\x2', '\x2', '\x5', '\x2', '\x45', 
		'L', '\x3', '\b', '\x2', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
