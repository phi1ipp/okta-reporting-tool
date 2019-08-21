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
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.2")]
[System.CLSCompliant(false)]
public partial class BoolExprParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		ATTR=10, STR=11, WS=12;
	public const int
		RULE_expr = 0, RULE_attr_comp = 1, RULE_attr_pr = 2, RULE_attr = 3;
	public static readonly string[] ruleNames = {
		"expr", "attr_comp", "attr_pr", "attr"
	};

	private static readonly string[] _LiteralNames = {
		null, "'('", "')'", "'and'", "'or'", "'not'", "'eq'", "'co'", "'pr'", 
		"'profile.'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, "ATTR", "STR", 
		"WS"
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

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static BoolExprParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public BoolExprParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public BoolExprParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class ExprContext : ParserRuleContext {
		public ExprContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_expr; } }
	 
		public ExprContext() { }
		public virtual void CopyFrom(ExprContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class AndExpContext : ExprContext {
		public ExprContext[] expr() {
			return GetRuleContexts<ExprContext>();
		}
		public ExprContext expr(int i) {
			return GetRuleContext<ExprContext>(i);
		}
		public AndExpContext(ExprContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IBoolExprVisitor<TResult> typedVisitor = visitor as IBoolExprVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitAndExp(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class AttrPrExpContext : ExprContext {
		public Attr_prContext attr_pr() {
			return GetRuleContext<Attr_prContext>(0);
		}
		public AttrPrExpContext(ExprContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IBoolExprVisitor<TResult> typedVisitor = visitor as IBoolExprVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitAttrPrExp(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class ParenthesisExpContext : ExprContext {
		public ExprContext expr() {
			return GetRuleContext<ExprContext>(0);
		}
		public ParenthesisExpContext(ExprContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IBoolExprVisitor<TResult> typedVisitor = visitor as IBoolExprVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitParenthesisExp(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class AttrCompExpContext : ExprContext {
		public Attr_compContext attr_comp() {
			return GetRuleContext<Attr_compContext>(0);
		}
		public AttrCompExpContext(ExprContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IBoolExprVisitor<TResult> typedVisitor = visitor as IBoolExprVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitAttrCompExp(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class OrExpContext : ExprContext {
		public ExprContext[] expr() {
			return GetRuleContexts<ExprContext>();
		}
		public ExprContext expr(int i) {
			return GetRuleContext<ExprContext>(i);
		}
		public OrExpContext(ExprContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IBoolExprVisitor<TResult> typedVisitor = visitor as IBoolExprVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitOrExp(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class NotExpContext : ExprContext {
		public ExprContext expr() {
			return GetRuleContext<ExprContext>(0);
		}
		public NotExpContext(ExprContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IBoolExprVisitor<TResult> typedVisitor = visitor as IBoolExprVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitNotExp(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ExprContext expr() {
		return expr(0);
	}

	private ExprContext expr(int _p) {
		ParserRuleContext _parentctx = Context;
		int _parentState = State;
		ExprContext _localctx = new ExprContext(Context, _parentState);
		ExprContext _prevctx = _localctx;
		int _startState = 0;
		EnterRecursionRule(_localctx, 0, RULE_expr, _p);
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 17;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,0,Context) ) {
			case 1:
				{
				_localctx = new ParenthesisExpContext(_localctx);
				Context = _localctx;
				_prevctx = _localctx;

				State = 9; Match(T__0);
				State = 10; expr(0);
				State = 11; Match(T__1);
				}
				break;
			case 2:
				{
				_localctx = new NotExpContext(_localctx);
				Context = _localctx;
				_prevctx = _localctx;
				State = 13; Match(T__4);
				State = 14; expr(3);
				}
				break;
			case 3:
				{
				_localctx = new AttrCompExpContext(_localctx);
				Context = _localctx;
				_prevctx = _localctx;
				State = 15; attr_comp();
				}
				break;
			case 4:
				{
				_localctx = new AttrPrExpContext(_localctx);
				Context = _localctx;
				_prevctx = _localctx;
				State = 16; attr_pr();
				}
				break;
			}
			Context.Stop = TokenStream.LT(-1);
			State = 27;
			ErrorHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(TokenStream,2,Context);
			while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( ParseListeners!=null )
						TriggerExitRuleEvent();
					_prevctx = _localctx;
					{
					State = 25;
					ErrorHandler.Sync(this);
					switch ( Interpreter.AdaptivePredict(TokenStream,1,Context) ) {
					case 1:
						{
						_localctx = new AndExpContext(new ExprContext(_parentctx, _parentState));
						PushNewRecursionContext(_localctx, _startState, RULE_expr);
						State = 19;
						if (!(Precpred(Context, 5))) throw new FailedPredicateException(this, "Precpred(Context, 5)");
						State = 20; Match(T__2);
						State = 21; expr(6);
						}
						break;
					case 2:
						{
						_localctx = new OrExpContext(new ExprContext(_parentctx, _parentState));
						PushNewRecursionContext(_localctx, _startState, RULE_expr);
						State = 22;
						if (!(Precpred(Context, 4))) throw new FailedPredicateException(this, "Precpred(Context, 4)");
						State = 23; Match(T__3);
						State = 24; expr(5);
						}
						break;
					}
					} 
				}
				State = 29;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,2,Context);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			UnrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public partial class Attr_compContext : ParserRuleContext {
		public Attr_compContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_attr_comp; } }
	 
		public Attr_compContext() { }
		public virtual void CopyFrom(Attr_compContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class CoCompareContext : Attr_compContext {
		public AttrContext attr() {
			return GetRuleContext<AttrContext>(0);
		}
		public ITerminalNode STR() { return GetToken(BoolExprParser.STR, 0); }
		public CoCompareContext(Attr_compContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IBoolExprVisitor<TResult> typedVisitor = visitor as IBoolExprVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitCoCompare(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class EqCompareContext : Attr_compContext {
		public AttrContext attr() {
			return GetRuleContext<AttrContext>(0);
		}
		public ITerminalNode STR() { return GetToken(BoolExprParser.STR, 0); }
		public EqCompareContext(Attr_compContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IBoolExprVisitor<TResult> typedVisitor = visitor as IBoolExprVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitEqCompare(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Attr_compContext attr_comp() {
		Attr_compContext _localctx = new Attr_compContext(Context, State);
		EnterRule(_localctx, 2, RULE_attr_comp);
		try {
			State = 38;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,3,Context) ) {
			case 1:
				_localctx = new EqCompareContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 30; attr();
				State = 31; Match(T__5);
				State = 32; Match(STR);
				}
				break;
			case 2:
				_localctx = new CoCompareContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 34; attr();
				State = 35; Match(T__6);
				State = 36; Match(STR);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Attr_prContext : ParserRuleContext {
		public AttrContext attr() {
			return GetRuleContext<AttrContext>(0);
		}
		public Attr_prContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_attr_pr; } }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IBoolExprVisitor<TResult> typedVisitor = visitor as IBoolExprVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitAttr_pr(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public Attr_prContext attr_pr() {
		Attr_prContext _localctx = new Attr_prContext(Context, State);
		EnterRule(_localctx, 4, RULE_attr_pr);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 40; attr();
			State = 41; Match(T__7);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class AttrContext : ParserRuleContext {
		public AttrContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_attr; } }
	 
		public AttrContext() { }
		public virtual void CopyFrom(AttrContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class ProfileAttrContext : AttrContext {
		public ITerminalNode ATTR() { return GetToken(BoolExprParser.ATTR, 0); }
		public ProfileAttrContext(AttrContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IBoolExprVisitor<TResult> typedVisitor = visitor as IBoolExprVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitProfileAttr(this);
			else return visitor.VisitChildren(this);
		}
	}
	public partial class NonProfileAttrContext : AttrContext {
		public ITerminalNode ATTR() { return GetToken(BoolExprParser.ATTR, 0); }
		public NonProfileAttrContext(AttrContext context) { CopyFrom(context); }
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			IBoolExprVisitor<TResult> typedVisitor = visitor as IBoolExprVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitNonProfileAttr(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public AttrContext attr() {
		AttrContext _localctx = new AttrContext(Context, State);
		EnterRule(_localctx, 6, RULE_attr);
		try {
			State = 46;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case T__8:
				_localctx = new ProfileAttrContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 43; Match(T__8);
				State = 44; Match(ATTR);
				}
				break;
			case ATTR:
				_localctx = new NonProfileAttrContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 45; Match(ATTR);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public override bool Sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 0: return expr_sempred((ExprContext)_localctx, predIndex);
		}
		return true;
	}
	private bool expr_sempred(ExprContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0: return Precpred(Context, 5);
		case 1: return Precpred(Context, 4);
		}
		return true;
	}

	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x3', '\xE', '\x33', '\x4', '\x2', '\t', '\x2', '\x4', '\x3', 
		'\t', '\x3', '\x4', '\x4', '\t', '\x4', '\x4', '\x5', '\t', '\x5', '\x3', 
		'\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', 
		'\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x5', '\x2', '\x14', 
		'\n', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', '\x2', '\x3', 
		'\x2', '\x3', '\x2', '\a', '\x2', '\x1C', '\n', '\x2', '\f', '\x2', '\xE', 
		'\x2', '\x1F', '\v', '\x2', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', '\x3', 
		'\x5', '\x3', ')', '\n', '\x3', '\x3', '\x4', '\x3', '\x4', '\x3', '\x4', 
		'\x3', '\x5', '\x3', '\x5', '\x3', '\x5', '\x5', '\x5', '\x31', '\n', 
		'\x5', '\x3', '\x5', '\x2', '\x3', '\x2', '\x6', '\x2', '\x4', '\x6', 
		'\b', '\x2', '\x2', '\x2', '\x35', '\x2', '\x13', '\x3', '\x2', '\x2', 
		'\x2', '\x4', '(', '\x3', '\x2', '\x2', '\x2', '\x6', '*', '\x3', '\x2', 
		'\x2', '\x2', '\b', '\x30', '\x3', '\x2', '\x2', '\x2', '\n', '\v', '\b', 
		'\x2', '\x1', '\x2', '\v', '\f', '\a', '\x3', '\x2', '\x2', '\f', '\r', 
		'\x5', '\x2', '\x2', '\x2', '\r', '\xE', '\a', '\x4', '\x2', '\x2', '\xE', 
		'\x14', '\x3', '\x2', '\x2', '\x2', '\xF', '\x10', '\a', '\a', '\x2', 
		'\x2', '\x10', '\x14', '\x5', '\x2', '\x2', '\x5', '\x11', '\x14', '\x5', 
		'\x4', '\x3', '\x2', '\x12', '\x14', '\x5', '\x6', '\x4', '\x2', '\x13', 
		'\n', '\x3', '\x2', '\x2', '\x2', '\x13', '\xF', '\x3', '\x2', '\x2', 
		'\x2', '\x13', '\x11', '\x3', '\x2', '\x2', '\x2', '\x13', '\x12', '\x3', 
		'\x2', '\x2', '\x2', '\x14', '\x1D', '\x3', '\x2', '\x2', '\x2', '\x15', 
		'\x16', '\f', '\a', '\x2', '\x2', '\x16', '\x17', '\a', '\x5', '\x2', 
		'\x2', '\x17', '\x1C', '\x5', '\x2', '\x2', '\b', '\x18', '\x19', '\f', 
		'\x6', '\x2', '\x2', '\x19', '\x1A', '\a', '\x6', '\x2', '\x2', '\x1A', 
		'\x1C', '\x5', '\x2', '\x2', '\a', '\x1B', '\x15', '\x3', '\x2', '\x2', 
		'\x2', '\x1B', '\x18', '\x3', '\x2', '\x2', '\x2', '\x1C', '\x1F', '\x3', 
		'\x2', '\x2', '\x2', '\x1D', '\x1B', '\x3', '\x2', '\x2', '\x2', '\x1D', 
		'\x1E', '\x3', '\x2', '\x2', '\x2', '\x1E', '\x3', '\x3', '\x2', '\x2', 
		'\x2', '\x1F', '\x1D', '\x3', '\x2', '\x2', '\x2', ' ', '!', '\x5', '\b', 
		'\x5', '\x2', '!', '\"', '\a', '\b', '\x2', '\x2', '\"', '#', '\a', '\r', 
		'\x2', '\x2', '#', ')', '\x3', '\x2', '\x2', '\x2', '$', '%', '\x5', '\b', 
		'\x5', '\x2', '%', '&', '\a', '\t', '\x2', '\x2', '&', '\'', '\a', '\r', 
		'\x2', '\x2', '\'', ')', '\x3', '\x2', '\x2', '\x2', '(', ' ', '\x3', 
		'\x2', '\x2', '\x2', '(', '$', '\x3', '\x2', '\x2', '\x2', ')', '\x5', 
		'\x3', '\x2', '\x2', '\x2', '*', '+', '\x5', '\b', '\x5', '\x2', '+', 
		',', '\a', '\n', '\x2', '\x2', ',', '\a', '\x3', '\x2', '\x2', '\x2', 
		'-', '.', '\a', '\v', '\x2', '\x2', '.', '\x31', '\a', '\f', '\x2', '\x2', 
		'/', '\x31', '\a', '\f', '\x2', '\x2', '\x30', '-', '\x3', '\x2', '\x2', 
		'\x2', '\x30', '/', '\x3', '\x2', '\x2', '\x2', '\x31', '\t', '\x3', '\x2', 
		'\x2', '\x2', '\a', '\x13', '\x1B', '\x1D', '(', '\x30',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
