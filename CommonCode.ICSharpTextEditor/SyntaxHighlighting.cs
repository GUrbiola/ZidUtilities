using System;

namespace ZidUtilities.CommonCode.ICSharpTextEditor
{
    /// <summary>
    /// Enumeration of a small set of common syntax highlighting categories used by the editor.
    /// </summary>
    public enum SyntaxHighlighting
    {
        None,
        CSharp,
        XML,
        TransactSQL,
        MySql,
        JavaScript,
        HTML,
        CSS,
        Json
    }

    /// <summary>
    /// Enumeration of all available syntax highlighting languages.
    /// </summary>
    public enum SyntaxLanguage
    {
        ActionScript,
        Ada,
        ANTLR,
        Assembly,
        AutoHotkey,
        Batch,
        Boo,
        C,
        CPlusPlus,
        Ceylon,
        ChucK,
        Clojure,
        Cocoa,
        CoffeeScript,
        Cool,
        CSharp,
        CSS,
        D,
        Dart,
        Delphi,
        Eiffel,
        Elixir,
        Erlang,
        FSharp,
        Falcon,
        Fantom,
        Fortran95,
        Go,
        Goovy,
        Gui4Cli,
        Haskell,
        Haxe,
        HTML,
        Icon,
        ILYC,
        INI,
        Io,
        Java,
        JavaScript,
        JSON,
        Julia,
        JustBASIC,
        KiXtart,
        Kotlin,
        Lean,
        Lisp,
        Lua,
        MySQL,
        Nemerle,
        Nim,
        ObjectiveC,
        OCaml,
        ParaSail,
        Pascal,
        PHP,
        Pike,
        PowerShell,
        Prolog,
        PureScript,
        Python,
        R,
        Registry,
        Resource,
        Rexx,
        Rust,
        Scala,
        Scheme,
        Solidity,
        Spike,
        SQF,
        Swift,
        TCL,
        Thrift,
        TSQL,
        TypeScript,
        Vala,
        VBNET,
        VBScript,
        Verilog,
        VHDL,
        Volt,
        VSSolution,
        X10,
        XC,
        XML,
        Xtend
    }

    /// <summary>
    /// Helper class to retrieve syntax definition files from embedded resources.
    /// </summary>
    public static class SyntaxProvider
    {
        /// <summary>
        /// Gets the syntax definition file content for the specified language.
        /// </summary>
        /// <param name="language">The syntax language to retrieve.</param>
        /// <returns>
        /// The XML content of the syntax definition file (.xshd) for the specified <paramref name="language"/>.
        /// The returned string contains the complete syntax definition used by the editor's highlighting engine.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when the language is not supported.</exception>
        /// <remarks>
        /// This method maps the <see cref="SyntaxLanguage"/> enumeration values to corresponding
        /// embedded resource strings in the <c>SyntaxFiles</c> class. Consumers should handle
        /// <see cref="ArgumentException"/> in case an unsupported or unknown language value is supplied.
        /// The syntax highlighting definitions all come from this URL: https://github.com/xv/ICSharpCode.TextEditor-Lexers
        /// Except for TSQL which was created by me.
        /// </remarks>
        public static string GetSyntaxFile(SyntaxLanguage language)
        {
            switch (language)
            {
                case SyntaxLanguage.ActionScript:
                    return SyntaxFiles.ActionScript;
                case SyntaxLanguage.Ada:
                    return SyntaxFiles.Ada;
                case SyntaxLanguage.ANTLR:
                    return SyntaxFiles.ANTLR;
                case SyntaxLanguage.Assembly:
                    return SyntaxFiles.Assembly;
                case SyntaxLanguage.AutoHotkey:
                    return SyntaxFiles.AutoHotkey;
                case SyntaxLanguage.Batch:
                    return SyntaxFiles.Batch;
                case SyntaxLanguage.Boo:
                    return SyntaxFiles.Boo;
                case SyntaxLanguage.C:
                    return SyntaxFiles.C;
                case SyntaxLanguage.CPlusPlus:
                    return SyntaxFiles.CPlusPlus;
                case SyntaxLanguage.Ceylon:
                    return SyntaxFiles.Ceylon;
                case SyntaxLanguage.ChucK:
                    return SyntaxFiles.ChucK;
                case SyntaxLanguage.Clojure:
                    return SyntaxFiles.Clojure;
                case SyntaxLanguage.Cocoa:
                    return SyntaxFiles.Cocoa;
                case SyntaxLanguage.CoffeeScript:
                    return SyntaxFiles.CoffeeScript;
                case SyntaxLanguage.Cool:
                    return SyntaxFiles.Cool;
                case SyntaxLanguage.CSharp:
                    return SyntaxFiles.CSharp;
                case SyntaxLanguage.CSS:
                    return SyntaxFiles.CSS;
                case SyntaxLanguage.D:
                    return SyntaxFiles.D;
                case SyntaxLanguage.Dart:
                    return SyntaxFiles.Dart;
                case SyntaxLanguage.Delphi:
                    return SyntaxFiles.Delphi;
                case SyntaxLanguage.Eiffel:
                    return SyntaxFiles.Eiffel;
                case SyntaxLanguage.Elixir:
                    return SyntaxFiles.Elixir;
                case SyntaxLanguage.Erlang:
                    return SyntaxFiles.Erlang;
                case SyntaxLanguage.FSharp:
                    return SyntaxFiles.FSharp;
                case SyntaxLanguage.Falcon:
                    return SyntaxFiles.Falcon;
                case SyntaxLanguage.Fantom:
                    return SyntaxFiles.Fantom;
                case SyntaxLanguage.Fortran95:
                    return SyntaxFiles.Fortran95;
                case SyntaxLanguage.Go:
                    return SyntaxFiles.Go;
                case SyntaxLanguage.Goovy:
                    return SyntaxFiles.Goovy;
                case SyntaxLanguage.Gui4Cli:
                    return SyntaxFiles.Gui4Cli;
                case SyntaxLanguage.Haskell:
                    return SyntaxFiles.Haskell;
                case SyntaxLanguage.Haxe:
                    return SyntaxFiles.Haxe;
                case SyntaxLanguage.HTML:
                    return SyntaxFiles.HTML;
                case SyntaxLanguage.Icon:
                    return SyntaxFiles.Icon;
                case SyntaxLanguage.ILYC:
                    return SyntaxFiles.ILYC;
                case SyntaxLanguage.INI:
                    return SyntaxFiles.INI;
                case SyntaxLanguage.Io:
                    return SyntaxFiles.Io;
                case SyntaxLanguage.Java:
                    return SyntaxFiles.Java;
                case SyntaxLanguage.JavaScript:
                    return SyntaxFiles.JavaScript;
                case SyntaxLanguage.JSON:
                    return SyntaxFiles.JSON;
                case SyntaxLanguage.Julia:
                    return SyntaxFiles.Julia;
                case SyntaxLanguage.JustBASIC:
                    return SyntaxFiles.JustBASIC;
                case SyntaxLanguage.KiXtart:
                    return SyntaxFiles.KiXtart;
                case SyntaxLanguage.Kotlin:
                    return SyntaxFiles.Kotlin;
                case SyntaxLanguage.Lean:
                    return SyntaxFiles.Lean;
                case SyntaxLanguage.Lisp:
                    return SyntaxFiles.Lisp;
                case SyntaxLanguage.Lua:
                    return SyntaxFiles.Lua;
                case SyntaxLanguage.MySQL:
                    return SyntaxFiles.MySQL;
                case SyntaxLanguage.Nemerle:
                    return SyntaxFiles.Nemerle;
                case SyntaxLanguage.Nim:
                    return SyntaxFiles.Nim;
                case SyntaxLanguage.ObjectiveC:
                    return SyntaxFiles.ObjectiveC;
                case SyntaxLanguage.OCaml:
                    return SyntaxFiles.OCaml;
                case SyntaxLanguage.ParaSail:
                    return SyntaxFiles.ParaSail;
                case SyntaxLanguage.Pascal:
                    return SyntaxFiles.Pascal;
                case SyntaxLanguage.PHP:
                    return SyntaxFiles.PHP;
                case SyntaxLanguage.Pike:
                    return SyntaxFiles.Pike;
                case SyntaxLanguage.PowerShell:
                    return SyntaxFiles.PowerShell;
                case SyntaxLanguage.Prolog:
                    return SyntaxFiles.Prolog;
                case SyntaxLanguage.PureScript:
                    return SyntaxFiles.PureScript;
                case SyntaxLanguage.Python:
                    return SyntaxFiles.Python;
                case SyntaxLanguage.R:
                    return SyntaxFiles.R;
                case SyntaxLanguage.Registry:
                    return SyntaxFiles.Registry;
                case SyntaxLanguage.Resource:
                    return SyntaxFiles.Resource;
                case SyntaxLanguage.Rexx:
                    return SyntaxFiles.Rexx;
                case SyntaxLanguage.Rust:
                    return SyntaxFiles.Rust;
                case SyntaxLanguage.Scala:
                    return SyntaxFiles.Scala;
                case SyntaxLanguage.Scheme:
                    return SyntaxFiles.Scheme;
                case SyntaxLanguage.Solidity:
                    return SyntaxFiles.Solidity;
                case SyntaxLanguage.Spike:
                    return SyntaxFiles.Spike;
                case SyntaxLanguage.SQF:
                    return SyntaxFiles.SQF;
                case SyntaxLanguage.Swift:
                    return SyntaxFiles.Swift;
                case SyntaxLanguage.TCL:
                    return SyntaxFiles.TCL;
                case SyntaxLanguage.Thrift:
                    return SyntaxFiles.Thrift;
                case SyntaxLanguage.TSQL:
                    return SyntaxFiles.TSQL;
                case SyntaxLanguage.TypeScript:
                    return SyntaxFiles.TypeScript;
                case SyntaxLanguage.Vala:
                    return SyntaxFiles.Vala;
                case SyntaxLanguage.VBNET:
                    return SyntaxFiles.VBNET;
                case SyntaxLanguage.VBScript:
                    return SyntaxFiles.VBScript;
                case SyntaxLanguage.Verilog:
                    return SyntaxFiles.Verilog;
                case SyntaxLanguage.VHDL:
                    return SyntaxFiles.VHDL;
                case SyntaxLanguage.Volt:
                    return SyntaxFiles.Volt;
                case SyntaxLanguage.VSSolution:
                    return SyntaxFiles.VSSolution;
                case SyntaxLanguage.X10:
                    return SyntaxFiles.X10;
                case SyntaxLanguage.XC:
                    return SyntaxFiles.XC;
                case SyntaxLanguage.XML:
                    return SyntaxFiles.XML;
                case SyntaxLanguage.Xtend:
                    return SyntaxFiles.Xtend;
                default:
                    throw new ArgumentException($"Unsupported syntax language: {language}", nameof(language));
            }
        }
    }
}
