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
		T__9=10, T__10=11, ATTR=12, STR=13, WS=14;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
		"T__9", "T__10", "ATTR", "STR", "WS"
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
		"'ew'", "'pr'", "'profile.'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		"ATTR", "STR", "WS"
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
		'\x5964', '\x2', '\x10', 'Y', '\b', '\x1', '\x4', '\x2', '\t', '\x2', 
		'\x4', '\x3', '\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', 
		'\x5', '\x4', '\x6', '\t', '\x6', '\x4', '\a', '\t', '\a', '\x4', '\b', 
		'\t', '\b', '\x4', '\t', '\t', '\t', '\x4', '\n', '\t', '\n', '\x4', '\v', 
		'\t', '\v', '\x4', '\f', '\t', '\f', '\x4', '\r', '\t', '\r', '\x4', '\xE', 
		'\t', '\xE', '\x4', '\xF', '\t', '\xF', '\x3', '\x2', '\x3', '\x2', '\x3', 
		'\x3', '\x3', '\x3', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', '\x3', 
		'\x4', '\x3', '\x5', '\x3', '\x5', '\x3', '\x5', '\x3', '\x5', '\x3', 
		'\x6', '\x3', '\x6', '\x3', '\x6', '\x3', '\a', '\x3', '\a', '\x3', '\a', 
		'\x3', '\b', '\x3', '\b', '\x3', '\b', '\x3', '\t', '\x3', '\t', '\x3', 
		'\t', '\x3', '\n', '\x3', '\n', '\x3', '\n', '\x3', '\v', '\x3', '\v', 
		'\x3', '\v', '\x3', '\f', '\x3', '\f', '\x3', '\f', '\x3', '\f', '\x3', 
		'\f', '\x3', '\f', '\x3', '\f', '\x3', '\f', '\x3', '\f', '\x3', '\r', 
		'\x3', '\r', '\a', '\r', 'I', '\n', '\r', '\f', '\r', '\xE', '\r', 'L', 
		'\v', '\r', '\x3', '\xE', '\x3', '\xE', '\x6', '\xE', 'P', '\n', '\xE', 
		'\r', '\xE', '\xE', '\xE', 'Q', '\x3', '\xE', '\x3', '\xE', '\x3', '\xF', 
		'\x3', '\xF', '\x3', '\xF', '\x3', '\xF', '\x2', '\x2', '\x10', '\x3', 
		'\x3', '\x5', '\x4', '\a', '\x5', '\t', '\x6', '\v', '\a', '\r', '\b', 
		'\xF', '\t', '\x11', '\n', '\x13', '\v', '\x15', '\f', '\x17', '\r', '\x19', 
		'\xE', '\x1B', '\xF', '\x1D', '\x10', '\x3', '\x2', '\x6', '\x4', '\x2', 
		'\x43', '\\', '\x63', '|', '\x6', '\x2', '\x32', ';', '\x43', '\\', '\x61', 
		'\x61', '\x63', '|', '\x3', '\x2', '$', '$', '\x4', '\x2', '\f', '\f', 
		'\"', '\"', '\x2', 'Z', '\x2', '\x3', '\x3', '\x2', '\x2', '\x2', '\x2', 
		'\x5', '\x3', '\x2', '\x2', '\x2', '\x2', '\a', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\t', '\x3', '\x2', '\x2', '\x2', '\x2', '\v', '\x3', '\x2', '\x2', 
		'\x2', '\x2', '\r', '\x3', '\x2', '\x2', '\x2', '\x2', '\xF', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x11', '\x3', '\x2', '\x2', '\x2', '\x2', '\x13', 
		'\x3', '\x2', '\x2', '\x2', '\x2', '\x15', '\x3', '\x2', '\x2', '\x2', 
		'\x2', '\x17', '\x3', '\x2', '\x2', '\x2', '\x2', '\x19', '\x3', '\x2', 
		'\x2', '\x2', '\x2', '\x1B', '\x3', '\x2', '\x2', '\x2', '\x2', '\x1D', 
		'\x3', '\x2', '\x2', '\x2', '\x3', '\x1F', '\x3', '\x2', '\x2', '\x2', 
		'\x5', '!', '\x3', '\x2', '\x2', '\x2', '\a', '#', '\x3', '\x2', '\x2', 
		'\x2', '\t', '\'', '\x3', '\x2', '\x2', '\x2', '\v', '+', '\x3', '\x2', 
		'\x2', '\x2', '\r', '.', '\x3', '\x2', '\x2', '\x2', '\xF', '\x31', '\x3', 
		'\x2', '\x2', '\x2', '\x11', '\x34', '\x3', '\x2', '\x2', '\x2', '\x13', 
		'\x37', '\x3', '\x2', '\x2', '\x2', '\x15', ':', '\x3', '\x2', '\x2', 
		'\x2', '\x17', '=', '\x3', '\x2', '\x2', '\x2', '\x19', '\x46', '\x3', 
		'\x2', '\x2', '\x2', '\x1B', 'M', '\x3', '\x2', '\x2', '\x2', '\x1D', 
		'U', '\x3', '\x2', '\x2', '\x2', '\x1F', ' ', '\a', '*', '\x2', '\x2', 
		' ', '\x4', '\x3', '\x2', '\x2', '\x2', '!', '\"', '\a', '+', '\x2', '\x2', 
		'\"', '\x6', '\x3', '\x2', '\x2', '\x2', '#', '$', '\a', 'p', '\x2', '\x2', 
		'$', '%', '\a', 'q', '\x2', '\x2', '%', '&', '\a', 'v', '\x2', '\x2', 
		'&', '\b', '\x3', '\x2', '\x2', '\x2', '\'', '(', '\a', '\x63', '\x2', 
		'\x2', '(', ')', '\a', 'p', '\x2', '\x2', ')', '*', '\a', '\x66', '\x2', 
		'\x2', '*', '\n', '\x3', '\x2', '\x2', '\x2', '+', ',', '\a', 'q', '\x2', 
		'\x2', ',', '-', '\a', 't', '\x2', '\x2', '-', '\f', '\x3', '\x2', '\x2', 
		'\x2', '.', '/', '\a', 'g', '\x2', '\x2', '/', '\x30', '\a', 's', '\x2', 
		'\x2', '\x30', '\xE', '\x3', '\x2', '\x2', '\x2', '\x31', '\x32', '\a', 
		'\x65', '\x2', '\x2', '\x32', '\x33', '\a', 'q', '\x2', '\x2', '\x33', 
		'\x10', '\x3', '\x2', '\x2', '\x2', '\x34', '\x35', '\a', 'u', '\x2', 
		'\x2', '\x35', '\x36', '\a', 'y', '\x2', '\x2', '\x36', '\x12', '\x3', 
		'\x2', '\x2', '\x2', '\x37', '\x38', '\a', 'g', '\x2', '\x2', '\x38', 
		'\x39', '\a', 'y', '\x2', '\x2', '\x39', '\x14', '\x3', '\x2', '\x2', 
		'\x2', ':', ';', '\a', 'r', '\x2', '\x2', ';', '<', '\a', 't', '\x2', 
		'\x2', '<', '\x16', '\x3', '\x2', '\x2', '\x2', '=', '>', '\a', 'r', '\x2', 
		'\x2', '>', '?', '\a', 't', '\x2', '\x2', '?', '@', '\a', 'q', '\x2', 
		'\x2', '@', '\x41', '\a', 'h', '\x2', '\x2', '\x41', '\x42', '\a', 'k', 
		'\x2', '\x2', '\x42', '\x43', '\a', 'n', '\x2', '\x2', '\x43', '\x44', 
		'\a', 'g', '\x2', '\x2', '\x44', '\x45', '\a', '\x30', '\x2', '\x2', '\x45', 
		'\x18', '\x3', '\x2', '\x2', '\x2', '\x46', 'J', '\t', '\x2', '\x2', '\x2', 
		'G', 'I', '\t', '\x3', '\x2', '\x2', 'H', 'G', '\x3', '\x2', '\x2', '\x2', 
		'I', 'L', '\x3', '\x2', '\x2', '\x2', 'J', 'H', '\x3', '\x2', '\x2', '\x2', 
		'J', 'K', '\x3', '\x2', '\x2', '\x2', 'K', '\x1A', '\x3', '\x2', '\x2', 
		'\x2', 'L', 'J', '\x3', '\x2', '\x2', '\x2', 'M', 'O', '\a', '$', '\x2', 
		'\x2', 'N', 'P', '\n', '\x4', '\x2', '\x2', 'O', 'N', '\x3', '\x2', '\x2', 
		'\x2', 'P', 'Q', '\x3', '\x2', '\x2', '\x2', 'Q', 'O', '\x3', '\x2', '\x2', 
		'\x2', 'Q', 'R', '\x3', '\x2', '\x2', '\x2', 'R', 'S', '\x3', '\x2', '\x2', 
		'\x2', 'S', 'T', '\a', '$', '\x2', '\x2', 'T', '\x1C', '\x3', '\x2', '\x2', 
		'\x2', 'U', 'V', '\t', '\x5', '\x2', '\x2', 'V', 'W', '\x3', '\x2', '\x2', 
		'\x2', 'W', 'X', '\b', '\xF', '\x2', '\x2', 'X', '\x1E', '\x3', '\x2', 
		'\x2', '\x2', '\x5', '\x2', 'J', 'Q', '\x3', '\b', '\x2', '\x2',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
