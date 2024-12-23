using Call.Core.Configuration;
using Call.Core.Lexing;
using Call.Core.Syntax;
using Call.Core.Utilities;
using Console = Call.Core.Configuration.Console;

var console = new Console(System.Console.Out, System.Console.Error, System.Console.In);
const string code = "a+b+a";

Settings settings = Settings.Default;

var lexer = new Lexer(code, settings, console);

var parser = new Parser(settings, lexer.Tokens, console);
var node = parser.Compare().node;

var left = ParserUtils.Left(node);
// var right = ParserUtils.Right(node);
var alph = ParserUtils.GetAlph(node);
var result = ParserUtils.HzPra(alph.Item1, alph.Item2, alph.Item3);

System.Console.WriteLine(String.Join("->", left));
// System.Console.WriteLine(right);
System.Console.WriteLine(result);