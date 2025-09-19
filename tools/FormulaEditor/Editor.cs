using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Kontecg.Accounting;
using Kontecg.Accounting.Formulas;
using Kontecg.Extensions;
using static Kontecg.Accounting.Formulas.FormulaEvaluator;

namespace FormulaEditor
{
    public partial class Editor : DevExpress.XtraEditors.XtraForm
    {
        private readonly Dictionary<string, Color> _syntaxColors = new Dictionary<string, Color>
        {
            { "keywords", Color.Blue },
            { "numbers", Color.Green },
            { "strings", Color.Orange },
            { "operators", Color.Red },
            { "functions", Color.Purple },
            { "comments", Color.Gray },
            { "grouping", Color.DarkCyan }
        };

        private readonly string[] _keywords = {
            "IF", "CASE", "DEFAULT", "FOR", "TO", "STEP",
            "SET", "GETCTA", "GETSCTA", "GETSCTL", "GETANAL",
            "GETEGSTO", "GETSCOPE", "WRITE", "WRITEFULL", "VOID"
        };

        private Dictionary<string, string> _syntaxHelp = new()
        {
            {"IF", "IF(condition, true_expr, false_expr)"},
            {"CASE", "CASE(switch, case1:result1, case2:result2, DEFAULT:default_result)"},
            {"FOR", "FOR(i := start TO end STEP step) { ... }"}
        };

        public Editor()
        {
            InitializeComponent();
            SetupEditor();
        }

        private void SetupEditor()
        {
            richEditor.Font = new Font("Consolas", 11);
            richEditor.TextChanged += (s, e) => HighlightSyntax();
            //richEditor.SelectionChanged += (s, e) => HandleBracketMatching();

            btnCheck.Click += (s, e) => ValidateSyntax();
        }

        private void HighlightSyntax()
        {
            int cursorPos = richEditor.SelectionStart;
            int selectionLen = richEditor.SelectionLength;

            // Reset all formatting
            richEditor.Select(0, richEditor.TextLength);
            richEditor.SelectionColor = Color.Black;

            // Tokenize and highlight
            string text = richEditor.Text;
            var lexer = new FormulaLexer(text);
            var tokens = lexer.Tokenize();

            foreach (var token in tokens)
            {
                richEditor.Select(token.StartIndex, token.Length);

                if (Array.Exists(_keywords, k => k.Equals(token.Lexeme, StringComparison.OrdinalIgnoreCase)))
                {
                    richEditor.SelectionColor = _syntaxColors["keywords"];
                }
                else if (token.Type == TokenType.Number)
                {
                    richEditor.SelectionColor = _syntaxColors["numbers"];
                }
                else if (token.Type == TokenType.String)
                {
                    richEditor.SelectionColor = _syntaxColors["strings"];
                }
                else if (token.Type == TokenType.Operator)
                {
                    richEditor.SelectionColor = _syntaxColors["operators"];
                }
                else if (token.Type == TokenType.Function)
                {
                    richEditor.SelectionColor = _syntaxColors["functions"];
                }
                else if (token.Type == TokenType.Grouping)
                {
                    richEditor.SelectionColor = _syntaxColors["grouping"];
                }
            }

            // Restore cursor position
            richEditor.Select(cursorPos, selectionLen);
            richEditor.SelectionColor = Color.Black;
        }

        private void HandleBracketMatching()
        {
            int pos = richEditor.SelectionStart;
            if (pos < 1 || pos > richEditor.TextLength) return;

            char currentChar = richEditor.Text[pos - 1];
            char matchChar = GetMatchingBracket(currentChar);
            if (matchChar == '\0') return;

            int matchPos = FindMatchingBracket(richEditor.Text, pos - 1, currentChar, matchChar);
            if (matchPos != -1)
            {
                richEditor.Select(matchPos, 1);
                richEditor.SelectionBackColor = Color.Coral;
                richEditor.Select(pos - 1, 1);
                richEditor.SelectionBackColor = Color.Coral;

                // Reset after delay
                var timer = new Timer { Interval = 100 };
                timer.Tick += (s, e) => {
                    richEditor.Select(matchPos, 1);
                    richEditor.SelectionBackColor = richEditor.BackColor;
                    richEditor.Select(pos - 1, 1);
                    richEditor.SelectionBackColor = richEditor.BackColor;
                    timer.Stop();
                };
                timer.Start();
            }
        }

        private char GetMatchingBracket(char c)
        {
            switch (c)
            {
                case '(': return ')';
                case ')': return '(';
                case '[': return ']';
                case ']': return '[';
                case '{': return '}';
                case '}': return '{';
                default: return '\0';
            }
        }

        private int FindMatchingBracket(string text, int pos, char open, char close)
        {
            int direction = open == '(' || open == '[' || open == '{' ? 1 : -1;
            int count = 1;

            for (int i = pos + direction; i >= 0 && i < text.Length; i += direction)
            {
                if (text[i] == open) count++;
                else if (text[i] == close) count--;
                if (count == 0) return i;
            }

            return -1; // No match found
        }

        private void ValidateSyntax()
        {
            try
            {
                var records = new List<ViewNameResultRecord>()
                {
                    new("1", "3", "1.25", 400349, 731, 22631, "505", "C_PVAC", false, true, false, "CUP", 1693.98M, 1693.98M * 0.0909M, 1693.98M * 0.125M, 1693.98M * 0.05M, 1693.98M * 0.015M),
                    new("1", "1", "2.25", 400349, 731, 22631, "504", "C_TP504", false, true, false, "CUP", 10163.86M, 10163.86M * 0.0909M, 10163.86M * 0.125M, 10163.86M * 0.05M, 10163.86M * 0.015M),
                    new("1", "1", "2.25", 400349, 731, 22631, "504", "C_PPEN", false, true, false, "CUP", 10163.86M, 10163.86M * 0.0909M, 10163.86M * 0.125M, 10163.86M * 0.05M, 10163.86M * 0.015M),
                };

                var ctx = new FormulaEvaluator.ExecutionContext();
                ctx.SetVariable("ACCOUNTS", new FormulaEvaluator.TableValue<AccountDefinition>([]));
                ctx.SetVariable("EXPENSE_ITEMS", new FormulaEvaluator.TableValue<ExpenseItemDefinition>([]));
                ctx.SetVariable("CLASSIFIERS", new FormulaEvaluator.TableValue<AccountingClassifierDefinition>(
                [
                    new AccountingClassifierDefinition("Company") {Id = 1},
                    new AccountingClassifierDefinition("Personal") {Id = 2}
                ]));

                ctx.SetVariable("NOTES", new FormulaEvaluator.TableValue<AccountingVoucherNote>([]));

                foreach (var scope in (int[]) [1, 2])
                {
                    foreach (var rec in records)
                    {
                        //Convierto las variables listas para ser usadas en el contexto
                        foreach (var variable in rec.ToPropertyDictionary())
                        {
                            switch (variable.Value)
                            {
                                case int:
                                    ctx.SetVariable(variable.Key, new IntegerValue(Convert.ToInt32(variable.Value)));
                                    break;
                                case decimal or float or double:
                                    ctx.SetVariable(variable.Key, new DecimalValue(Convert.ToDecimal(variable.Value)));
                                    break;
                                case bool:
                                    ctx.SetVariable(variable.Key, new BooleanValue(Convert.ToBoolean(variable.Value)));
                                    break;
                                default:
                                    ctx.SetVariable(variable.Key, new StringValue(variable.Value.ToString()));
                                    break;
                            }
                        }

                        ctx.SetVariable("SCOPE", new IntegerValue(scope));

                        //Construyo la formula pasando el contexto
                        var formula = new FormulaEvaluator(ctx);
                        formula.Evaluate(richEditor.Text);
                    }
                }

                message.Text = "Sintaxis válida";
                message.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                message.Text = $"Error: {ex.Message}";
                message.ForeColor = Color.Red;
            }
        }

        // Lexer adaptado para el editor
        public class FormulaLexer
        {
            private readonly string _source;
            private int _current;

            public FormulaLexer(string source) => _source = source;

            public List<EditorToken> Tokenize()
            {
                var tokens = new List<EditorToken>();
                _current = 0;

                while (_current < _source.Length)
                {
                    char c = _source[_current];

                    if (char.IsWhiteSpace(c))
                    {
                        _current++;
                        continue;
                    }

                    if (char.IsDigit(c))
                    {
                        tokens.Add(ReadNumber());
                        continue;
                    }

                    if (c == '"' || c == '\'')
                    {
                        tokens.Add(ReadString());
                        continue;
                    }

                    if ("(){}[]".Contains(c))
                    {
                        tokens.Add(new EditorToken(TokenType.Grouping, c.ToString(), _current, 1));
                        _current++;
                        continue;
                    }

                    if (IsOperatorChar(c))
                    {
                        tokens.Add(ReadOperator());
                        continue;
                    }

                    if (char.IsLetter(c) || c == '_')
                    {
                        tokens.Add(ReadIdentifier());
                        continue;
                    }

                    _current++; // Skip unknown characters
                }

                return tokens;
            }

            private EditorToken ReadNumber()
            {
                int start = _current;
                while (_current < _source.Length && char.IsDigit(_source[_current])) _current++;
                if (_current < _source.Length && _source[_current] == '.') _current++;
                while (_current < _source.Length && char.IsDigit(_source[_current])) _current++;

                return new EditorToken(
                    TokenType.Number,
                    _source.Substring(start, _current - start),
                    start,
                    _current - start
                );
            }

            private EditorToken ReadString()
            {
                char quote = _source[_current];
                int start = _current;
                _current++; // Skip opening quote

                while (_current < _source.Length && _source[_current] != quote)
                    _current++;

                if (_current < _source.Length) _current++; // Skip closing quote

                return new EditorToken(
                    TokenType.String,
                    _source.Substring(start, _current - start),
                    start,
                    _current - start
                );
            }

            private EditorToken ReadOperator()
            {
                int start = _current;
                while (_current < _source.Length && IsOperatorChar(_source[_current]))
                    _current++;

                string op = _source.Substring(start, _current - start);
                return new EditorToken(
                    TokenType.Operator,
                    op,
                    start,
                    _current - start
                );
            }

            private EditorToken ReadIdentifier()
            {
                int start = _current;
                while (_current < _source.Length && (char.IsLetterOrDigit(_source[_current]) || _source[_current] == '_'))
                    _current++;

                string id = _source.Substring(start, _current - start);
                return new EditorToken(
                    TokenType.Identifier,
                    id,
                    start,
                    _current - start
                );
            }

            private bool IsOperatorChar(char c) => "+-*/%^&|!<>=~:".Contains(c);
        }

        public enum TokenType
        {
            Keyword,
            Number,
            String,
            Operator,
            Function,
            Identifier,
            Grouping,
            Comment
        }

        public class EditorToken
        {
            public TokenType Type { get; }
            public string Lexeme { get; }
            public int StartIndex { get; }
            public int Length { get; }

            public EditorToken(TokenType type, string lexeme, int startIndex, int length)
            {
                Type = type;
                Lexeme = lexeme;
                StartIndex = startIndex;
                Length = length;
            }
        }
    }
}
