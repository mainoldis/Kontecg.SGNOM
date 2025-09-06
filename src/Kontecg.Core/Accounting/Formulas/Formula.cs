using System.Text;

namespace Kontecg.Accounting.Formulas
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using NMoneys;

    public sealed class FormulaEvaluator
    {
        #region Sistema de Tipos y Valores
        public enum ValueType { Integer, Decimal, Boolean, String, Void }

        public abstract class Value
        {
            public abstract ValueType Type { get; }
            public abstract object RawValue { get; }
            public int AsInteger() => this is IntegerValue dv ? dv.Val : throw new InvalidCastException("Expected integer");
            public decimal AsDecimal() => this is DecimalValue dv ? dv.Val : throw new InvalidCastException("Expected decimal");
            public bool AsBoolean() => this is BooleanValue bv ? bv.Val : throw new InvalidCastException("Expected boolean");
            public string AsString() => this is StringValue sv ? sv.Val : throw new InvalidCastException("Expected string");
            public IList<T> AsTable<T>() => this is TableValue<T> sv ? sv.Val : throw new InvalidCastException("Expected List<T>");
            public bool IsVoid => Type == ValueType.Void;
        }

        public sealed class IntegerValue : Value
        {
            public int Val { get; }
            public override ValueType Type => ValueType.Integer;
            public override object RawValue => Val;
            public IntegerValue(int value) => Val = value;

            public static IntegerValue Instance { get; } = new IntegerValue(0);
        }

        public sealed class DecimalValue : Value
        {
            public decimal Val { get; }
            public override ValueType Type => ValueType.Decimal;
            public override object RawValue => Val;
            public DecimalValue(decimal value) => Val = value;
        }

        public sealed class BooleanValue : Value
        {
            public bool Val { get; }
            public override ValueType Type => ValueType.Boolean;
            public override object RawValue => Val;
            public BooleanValue(bool value) => Val = value;
        }

        public sealed class StringValue : Value
        {
            public string Val { get; }
            public override ValueType Type => ValueType.String;
            public override object RawValue => Val;
            public StringValue(string value) => Val = value;
        }

        public sealed class TableValue<T> : Value
        {
            public List<T> Val { get; }

            /// <inheritdoc />
            public override object RawValue => Val;

            /// <inheritdoc />
            public override ValueType Type => ValueType.Void;

            /// <inheritdoc />
            public TableValue(List<T> val) => Val = val;
        }

        public sealed class VoidValue : Value
        {
            public override ValueType Type => ValueType.Void;
            public override object RawValue => null;
            public static VoidValue Instance { get; } = new VoidValue();
        }
        #endregion

        #region Contexto de Ejecución
        public sealed class ExecutionContext
        {
            private readonly string[] _constantNames = ["PI", "TRUE", "FALSE", "VOID"];
            private readonly Dictionary<string, Value> _variables = new();
            private readonly Dictionary<string, FunctionDescriptor> _functions = new();

            public void SetVariable(string name, Value value) => _variables[name] = value;
            public Value GetVariable(string name) => _variables.TryGetValue(name, out var val) ? val : throw new FormulaException($"Undefined variable: {name}");

            public void RegisterFunction(string name, int minArgs, int maxArgs, FunctionImplementation impl)
                => _functions[name.ToUpper()] = new FunctionDescriptor(minArgs, maxArgs, impl);

            public FunctionDescriptor GetFunction(string name)
                => _functions.TryGetValue(name.ToUpper(), out var func) ? func : null;

            public void InitializeConstants()
            {
                SetVariable("PI", new DecimalValue(3.14159265358979323846m));
                SetVariable("TRUE", new BooleanValue(true));
                SetVariable("FALSE", new BooleanValue(false));
                SetVariable("VOID", VoidValue.Instance);
            }

            public bool TryGetVariable(string name, out Value value)
            {
                return _variables.TryGetValue(name, out value);
            }

            public void RemoveVariable(string name)
            {
                _variables.Remove(name);
            }

            /// <inheritdoc />
            public override string ToString()
            {
                var sb = new StringBuilder();
                foreach (var variable in _variables)
                {
                    if (!_constantNames.Contains(variable.Key))
                    {
                        if (variable.Value is IntegerValue or DecimalValue or BooleanValue or StringValue)
                            sb.AppendLine($"{variable.Key}: {variable.Value.RawValue}");
                        else if (variable.Value.Type is ValueType.Void)
                            sb.AppendLine($"{variable.Key}: [..]");
                    }
                }
                return $"{nameof(_variables)}:\n{sb}";
            }
        }

        public delegate Value FunctionImplementation(ExecutionContext context, Value[] args);

        public sealed record FunctionDescriptor(
            int MinArgs,
            int MaxArgs,
            FunctionImplementation Implementation
        );
        #endregion

        #region Árbol de Sintaxis Abstracta (AST)
        public abstract class Expression
        {
            public abstract Value Evaluate(ExecutionContext context);
        }

        private sealed class BlockExpression : Expression
        {
            private readonly Expression[] _expressions;
            public BlockExpression(Expression[] expressions) => _expressions = expressions;

            public override Value Evaluate(ExecutionContext context)
            {
                Value result = VoidValue.Instance;
                foreach (var expr in _expressions)
                    result = expr.Evaluate(context);
                return result;
            }
        }

        private sealed class VariableExpression : Expression
        {
            internal readonly string _name;
            public VariableExpression(string name) => _name = name;
            public override Value Evaluate(ExecutionContext context) => context.GetVariable(_name);
        }

        private sealed class AssignmentExpression : Expression
        {
            private readonly string _name;
            private readonly Expression _valueExpr;
            public AssignmentExpression(string name, Expression valueExpr)
                => (_name, _valueExpr) = (name, valueExpr);

            public override Value Evaluate(ExecutionContext context)
            {
                // Evaluar la expresión del valor PRIMERO
                var value = _valueExpr.Evaluate(context);

                // Luego asignar a la variable
                context.SetVariable(_name, value);

                return VoidValue.Instance;
            }
        }

        private sealed class LiteralExpression : Expression
        {
            private readonly Value _value;
            public LiteralExpression(Value value) => _value = value;
            public override Value Evaluate(ExecutionContext context) => _value;
        }

        private sealed class BinaryExpression : Expression
        {
            private readonly Expression _left;
            private readonly string _operator;
            private readonly Expression _right;

            public BinaryExpression(Expression left, string op, Expression right) => (_left, _operator, _right) = (left, op, right);

            public override Value Evaluate(ExecutionContext context)
            {
                if (_operator == ":=")
                    throw new FormulaException("Assignment operator := not allowed in binary expressions");

                var leftVal = _left.Evaluate(context);
                var rightVal = _right.Evaluate(context);

                return _operator switch
                {
                    // Aritméticos
                    "+" => ArithmeticOperation(leftVal, rightVal, (a, b) => a + b),
                    "-" => ArithmeticOperation(leftVal, rightVal, (a, b) => a - b),
                    "*" => ArithmeticOperation(leftVal, rightVal, (a, b) => a * b),
                    "/" => ArithmeticOperation(leftVal, rightVal, (a, b) => b != 0 ? a / b : throw new DivideByZeroException()),
                    "%" => ArithmeticOperation(leftVal, rightVal, (a, b) => a % b),
                    "^" => ArithmeticOperation(leftVal, rightVal, (a, b) => (decimal)Math.Pow((double)a, (double)b)),

                    // Comparación
                    "<" => CompareOperation(leftVal, rightVal, (a, b) => a < b),
                    ">" => CompareOperation(leftVal, rightVal, (a, b) => a > b),
                    "<=" => CompareOperation(leftVal, rightVal, (a, b) => a <= b),
                    ">=" => CompareOperation(leftVal, rightVal, (a, b) => a >= b),
                    "=" => EqualityOperation(leftVal, rightVal),
                    "<>" => Negate(EqualityOperation(leftVal, rightVal)),

                    // Booleanos
                    "&" => BooleanOperation(leftVal, rightVal, (a, b) => a && b),
                    "|" => BooleanOperation(leftVal, rightVal, (a, b) => a || b),

                    _ => throw new FormulaException($"Invalid operator: {_operator}")
                };
            }

            private static Value ArithmeticOperation(Value a, Value b, Func<decimal, decimal, decimal> op)
            {
                decimal aVal = a is IntegerValue ia ? ia.Val :
                    a is DecimalValue da ? da.Val :
                    throw new FormulaException("Expected numeric operand");

                decimal bVal = b is IntegerValue ib ? ib.Val :
                    b is DecimalValue db ? db.Val :
                    throw new FormulaException("Expected numeric operand");

                return new DecimalValue(op(aVal, bVal));
            }

            private static Value CompareOperation(Value a, Value b, Func<decimal, decimal, bool> op)
            {
                decimal aVal = a is IntegerValue ia ? ia.Val :
                    a is DecimalValue da ? da.Val :
                    throw new FormulaException("Expected numeric operand");

                decimal bVal = b is IntegerValue ib ? ib.Val :
                    b is DecimalValue db ? db.Val :
                    throw new FormulaException("Expected numeric operand");
                return new BooleanValue(op(aVal, bVal));
            }

            private static Value EqualityOperation(Value a, Value b)
            {
                return a switch
                {
                    IntegerValue ia when b is IntegerValue ib => new BooleanValue(ia.Val == ib.Val),
                    DecimalValue da when b is DecimalValue db => new BooleanValue(da.Val == db.Val),
                    BooleanValue ba when b is BooleanValue bb => new BooleanValue(ba.Val == bb.Val),
                    StringValue sa when b is StringValue sb => new BooleanValue(sa.Val == sb.Val),
                    _ => new BooleanValue(false)
                };
            }

            private static Value Negate(Value v) => v is BooleanValue bv
                ? new BooleanValue(!bv.Val)
                : throw new FormulaException("Negation requires boolean operand");

            private static Value BooleanOperation(Value a, Value b, Func<bool, bool, bool> op)
            {
                if (a is not BooleanValue ba || b is not BooleanValue bb)
                    throw new FormulaException("Boolean operations require boolean operands");
                return new BooleanValue(op(ba.Val, bb.Val));
            }
        }

        private sealed class UnaryExpression : Expression
        {
            private readonly string _operator;
            private readonly Expression _operand;

            public UnaryExpression(string op, Expression operand) => (_operator, _operand) = (op, operand);

            public override Value Evaluate(ExecutionContext context)
            {
                var val = _operand.Evaluate(context);

                return _operator switch
                {
                    "-" => val is DecimalValue dv ? new DecimalValue(-dv.Val) : throw new FormulaException("Unary '-' requires decimal operand"),
                    "!" => val is BooleanValue bv ? new BooleanValue(!bv.Val) : throw new FormulaException("Unary '!' requires boolean operand"),
                    _ => throw new FormulaException($"Invalid unary operator: {_operator}")
                };
            }
        }

        private sealed class FunctionCallExpression : Expression
        {
            private readonly string _name;
            private readonly Expression[] _args;

            public FunctionCallExpression(string name, Expression[] args) => (_name, _args) = (name, args);

            public override Value Evaluate(ExecutionContext context)
            {
                var func = context.GetFunction(_name) ?? throw new FormulaException($"Undefined function: {_name}");
                var args = _args.Select(arg => arg.Evaluate(context)).ToArray();

                if (args.Length < func.MinArgs || args.Length > func.MaxArgs)
                    throw new FormulaException($"Function {_name} expects {func.MinArgs}-{func.MaxArgs} arguments");

                return func.Implementation(context, args);
            }
        }

        private sealed class IffExpression : Expression
        {
            private readonly Expression _condition;
            private readonly Expression _trueExpr;
            private readonly Expression _falseExpr;

            public IffExpression(Expression cond, Expression trueExpr, Expression falseExpr)
                => (_condition, _trueExpr, _falseExpr) = (cond, trueExpr, falseExpr);

            public override Value Evaluate(ExecutionContext context)
            {
                var condVal = _condition.Evaluate(context);
                if (condVal is not BooleanValue bv)
                    throw new FormulaException("IF condition must be boolean");

                return bv.Val
                    ? _trueExpr.Evaluate(context)
                    : _falseExpr?.Evaluate(context) ?? VoidValue.Instance;
            }
        }

        private sealed class CaseExpression : Expression
        {
            private readonly Expression _switchValue; // null para modo condición
            private readonly (Expression Value, Expression Result)[] _cases;
            private readonly Expression _defaultResult;

            public CaseExpression(
                Expression switchValue,
                (Expression Value, Expression Result)[] cases,
                Expression defaultResult)
            {
                _switchValue = switchValue;
                _cases = cases;
                _defaultResult = defaultResult;
            }

            public override Value Evaluate(ExecutionContext context)
            {
                if (_switchValue != null)
                {
                    // Modo comparación (switch)
                    var switchVal = _switchValue.Evaluate(context);
                    foreach (var (caseValue, caseResult) in _cases)
                    {
                        var val = caseValue.Evaluate(context);
                        if (EqualityOperation(switchVal, val).AsBoolean())
                            return caseResult.Evaluate(context);
                    }
                }
                else
                {
                    // Modo condición
                    foreach (var (condition, caseResult) in _cases)
                    {
                        var condVal = condition.Evaluate(context);
                        if (condVal is not BooleanValue bv)
                            throw new FormulaException("Case condition must be boolean");
                        if (bv.Val)
                            return caseResult.Evaluate(context);
                    }
                }
                return _defaultResult.Evaluate(context);
            }

            private static Value EqualityOperation(Value a, Value b)
            {
                return a switch
                       {
                           IntegerValue ia when b is IntegerValue ib => new BooleanValue(ia.Val == ib.Val),
                           DecimalValue da when b is DecimalValue db => new BooleanValue(da.Val == db.Val),
                           BooleanValue ba when b is BooleanValue bb => new BooleanValue(ba.Val == bb.Val),
                           StringValue sa when b is StringValue sb => new BooleanValue(sa.Val == sb.Val),
                           _ => new BooleanValue(false)
                       };
            }
        }

        private sealed class ForExpression : Expression
        {
            private readonly string _variable;
            private readonly Expression _start;
            private readonly Expression _end;
            private readonly Expression _step;
            private readonly Expression _body;

            public ForExpression(
                string variable,
                Expression start,
                Expression end,
                Expression step,
                Expression body)
            {
                _variable = variable;
                _start = start;
                _end = end;
                _step = step;
                _body = body;
            }

            public override Value Evaluate(ExecutionContext context)
            {
                // Guardar el valor previo de la variable (si existe)
                Value previousValue = null;
                bool variableExisted = context.TryGetVariable(_variable, out previousValue);

                try
                {
                    // Evaluar los límites del bucle
                    var startVal = _start.Evaluate(context).AsInteger();
                    var endVal = _end.Evaluate(context).AsInteger();
                    var stepVal = _step != null
                        ? _step.Evaluate(context).AsInteger()
                        : (startVal <= endVal ? 1 : -1);

                    // Validar paso
                    if (stepVal == 0) throw new FormulaException("STEP cannot be zero");
                    if ((stepVal > 0 && startVal > endVal) || (stepVal < 0 && startVal < endVal))
                        return VoidValue.Instance;

                    // Ejecutar el bucle
                    for (int i = startVal;
                         stepVal > 0 ? i <= endVal : i >= endVal;
                         i += stepVal)
                    {
                        context.SetVariable(_variable, new IntegerValue(i));
                        _body.Evaluate(context);
                    }

                    return VoidValue.Instance;
                }
                finally
                {
                    // Restaurar el valor original de la variable
                    if (variableExisted)
                        context.SetVariable(_variable, previousValue);
                    else
                        context.RemoveVariable(_variable);
                }
            }
        }
        #endregion

        #region Analizador Léxico (Lexer)
        private enum TokenType { Number, String, Identifier, Operator, Keyword, Punctuation, EOF }

        private sealed class Token
        {
            public TokenType Type { get; }
            public string Lexeme { get; }
            public Value Literal { get; }
            public int Position { get; }

            public Token(TokenType type, string lexeme, Value literal, int position)
                => (Type, Lexeme, Literal, Position) = (type, lexeme, literal, position);
        }

        private sealed class Lexer
        {
            private readonly string _source;
            private int _start;
            private int _current;
            private int _position => _current - 1;

            private static readonly Dictionary<string, TokenType> Keywords = new(StringComparer.OrdinalIgnoreCase)
            {
                ["PI"] = TokenType.Keyword,
                ["TRUE"] = TokenType.Keyword,
                ["FALSE"] = TokenType.Keyword,
                ["IF"] = TokenType.Keyword,
                ["CASE"] = TokenType.Keyword,
                ["DEFAULT"] = TokenType.Keyword,
                ["VOID"] = TokenType.Keyword,
                ["FOR"] = TokenType.Keyword,
                ["TO"] = TokenType.Keyword,
                ["STEP"] = TokenType.Keyword
            };

            public Lexer(string source) => _source = source;

            public List<Token> Tokenize()
            {
                var tokens = new List<Token>();
                _start = 0;
                _current = 0;

                if (string.IsNullOrWhiteSpace(_source))
                {
                    tokens.Add(new Token(TokenType.EOF, "", null, 0));
                    return tokens;
                }

                while (!IsAtEnd())
                {
                    _start = _current;
                    char c = Advance();

                    switch (c)
                    {
                        case ' ': case '\t': case '\r': case '\n': break;
                        case '"': case '\'': tokens.Add(ReadString()); break;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            tokens.Add(ReadNumber()); break;
                        case ',': tokens.Add(new Token(TokenType.Punctuation, ",", null, _position)); break;
                        case ';': tokens.Add(new Token(TokenType.Punctuation, ";", null, _position)); break;
                        case '(': tokens.Add(new Token(TokenType.Punctuation, "(", null, _position)); break;
                        case ')': tokens.Add(new Token(TokenType.Punctuation, ")", null, _position)); break;
                        case '{': tokens.Add(new Token(TokenType.Punctuation, "{", null, _position)); break;
                        case '}': tokens.Add(new Token(TokenType.Punctuation, "}", null, _position)); break;
                        case ':':
                            if (Peek() == '=')
                            {
                                Advance(); // Consume '='
                                tokens.Add(new Token(TokenType.Operator, ":=", null, _position));
                            }
                            else
                                tokens.Add(new Token(TokenType.Punctuation, ":", null, _position));

                            break;
                        default:
                            if (IsAlpha(c)) tokens.Add(ReadIdentifier());
                            else if (IsOperator(c)) tokens.Add(ReadOperator());
                            else throw new FormulaException($"Unexpected character: {c}");
                            break;
                    }
                }

                tokens.Add(new Token(TokenType.EOF, "", null, _current));
                return tokens;
            }

            private Token ReadString()
            {
                while (Peek() != '"' && Peek() != '\'' && !IsAtEnd()) Advance();
                if (IsAtEnd()) throw new FormulaException("Unterminated string");

                Advance(); // Consume closing "
                string text = _source.Substring(_start + 1, _current - _start - 2);
                return new Token(TokenType.String, text, new StringValue(text), _position);
            }

            private Token ReadNumber()
            {
                bool isInteger = true;
                while (char.IsDigit(Peek())) Advance();

                if (Peek() == '.' && char.IsDigit(PeekNext()))
                {
                    isInteger = false;
                    Advance();
                    while (char.IsDigit(Peek())) Advance();
                }

                string num = _source.Substring(_start, _current - _start);

                if (isInteger && int.TryParse(num, out int intValue))
                    return new Token(TokenType.Number, num, new IntegerValue(intValue), _position);

                if (decimal.TryParse(num, out decimal decValue))
                    return new Token(TokenType.Number, num, new DecimalValue(decValue), _position);

                throw new FormulaException($"Invalid number: {num}");
            }

            private Token ReadIdentifier()
            {
                while (IsAlphaNumeric(Peek())) Advance();
                string id = _source.Substring(_start, _current - _start);

                if (Keywords.TryGetValue(id.ToUpper(), out TokenType type))
                    return new Token(type, id, id.ToUpper() switch
                    {
                        "TRUE" => new BooleanValue(true),
                        "FALSE" => new BooleanValue(false),
                        "PI" => new DecimalValue(3.14159265358979323846m),
                        "VOID" => VoidValue.Instance,
                        _ => null
                    }, _position);

                return new Token(TokenType.Identifier, id, null, _position);
            }

            private Token ReadOperator()
            {
                // Manejar operadores compuestos
                while (IsOperator(Peek())) Advance();
                string op = _source.Substring(_start, _current - _start);

                // Normalizar operadores
                op = op switch
                {
                    "==" => "=",
                    "!=" => "<>",
                    ":=" => ":=",
                    _ => op
                };

                return new Token(TokenType.Operator, op, null, _position);
            }

            private char Advance() => _source[_current++];
            private char Peek() => IsAtEnd() ? '\0' : _source[_current];
            private char PeekNext() => _current + 1 < _source.Length ? _source[_current + 1] : '\0';
            private bool IsAtEnd() => _current >= _source.Length;
            private bool IsAlpha(char c) => char.IsLetter(c) || c == '_';
            private bool IsAlphaNumeric(char c) => IsAlpha(c) || char.IsDigit(c);
            private bool IsOperator(char c) => "+-*/%^&|!<>=~".Contains(c);
        }
        #endregion

        #region Analizador Sintáctico (Parser)
        private sealed class Parser
        {
            private readonly List<Token> _tokens;
            private int _current;

            public Parser(List<Token> tokens) => _tokens = tokens;

            public Expression Parse()
            {
                return IsAtEnd() ? new LiteralExpression(VoidValue.Instance) : ParseExpression();
            }

            private Expression ParseExpression()
            {
                return ParseAssignment();
            }

            private Expression ParseAssignment()
            {
                var expr = ParseTernary();

                if (Match(TokenType.Operator, ":="))
                {
                    if (expr is not VariableExpression varExpr)
                        throw new Exception("Left side must be a variable");

                    var valueExpr = ParseExpression();
                    return new AssignmentExpression(varExpr._name, valueExpr);
                }

                return expr;
            }

            private Expression ParseTernary()
            {
                // 1. Primero verificar si es IF antes de parsear binarios
                if (Match(TokenType.Keyword, "IF"))
                {
                    Consume(TokenType.Punctuation, "(", "Expected '(' after IF");
                    var condition = ParseExpression();
                    Consume(TokenType.Punctuation, ",", "Expected ',' after condition");
                    var trueExpr = ParseExpression();

                    Expression falseExpr = new LiteralExpression(VoidValue.Instance);
                    if (Match(TokenType.Punctuation, ","))
                        falseExpr = ParseExpression();
                    Consume(TokenType.Punctuation, ")", $"Expected ')' after IF at position {Previous().Position}");
                    return new IffExpression(condition, trueExpr, falseExpr);
                }

                if (Match(TokenType.Keyword, "CASE"))
                    return ParseCaseExpression();

                if (Match(TokenType.Keyword, "FOR"))
                    return ParseForExpression();

                return ParseBinary();
            }

            private Expression ParseBinary(int minPrecedence = 0)
            {
                var expr = ParseUnary();

                while (true)
                {
                    if (Peek().Type != TokenType.Operator) break;
                    string op = Peek().Lexeme;

                    // Excluir operador de asignación del manejo binario
                    if (op == ":=") break;

                    int precedence = GetPrecedence(op);

                    if (precedence < minPrecedence) break;
                    Advance(); // Consume operator

                    var right = ParseBinary(precedence + 1);
                    expr = new BinaryExpression(expr, op, right);
                }

                return expr;
            }

            private Expression ParseUnary()
            {
                if (Match(TokenType.Operator, "-") || Match(TokenType.Operator, "!"))
                {
                    string op = Previous().Lexeme;
                    return new UnaryExpression(op, ParseUnary());
                }

                return ParsePrimary();
            }

            private Expression ParsePrimary()
            {
                if (Match(TokenType.Punctuation, "{"))
                    return ParseBlock();

                if (Match(TokenType.Number)) return new LiteralExpression(Previous().Literal);
                if (Match(TokenType.String)) return new LiteralExpression(Previous().Literal);
                
                if (Match(TokenType.Keyword))
                    return Previous().Literal is VoidValue ? new LiteralExpression(VoidValue.Instance) : new LiteralExpression(Previous().Literal);

                if (Match(TokenType.Identifier))
                {
                    string name = Previous().Lexeme;

                    if (Match(TokenType.Punctuation, "("))
                    {
                        var args = new List<Expression>();
                        if (!Check(TokenType.Punctuation, ")"))
                        {
                            do args.Add(ParseExpression());
                            while (Match(TokenType.Punctuation, ","));
                        }
                        Consume(TokenType.Punctuation, ")", "Expected ')' after arguments");
                        return new FunctionCallExpression(name, args.ToArray());
                    }

                    return new VariableExpression(name);
                }

                if (Match(TokenType.Punctuation, "("))
                {
                    var expr = ParseExpression();
                    Consume(TokenType.Punctuation, ")", "Expected ')' after expression");
                    return expr;
                }

                throw new FormulaException($"Unexpected token: {Peek().Lexeme}");
            }

            private Expression ParseBlock()
            {
                if (Check(TokenType.Punctuation, "}"))
                {
                    Advance(); // Consume '}'
                    return new BlockExpression([]);
                }

                var expressions = new List<Expression>();
                while (!Check(TokenType.Punctuation, "}") && !Check(TokenType.EOF))
                {
                    expressions.Add(ParseExpression());

                    // Solo consumir delimitador si no es el último elemento
                    if (Check(TokenType.Punctuation, ",") || Check(TokenType.Punctuation, ";"))
                    {
                        Advance();
                        // Salir si después del delimitador viene '}'
                        if (Check(TokenType.Punctuation, "}")) break;
                    }
                }
                Consume(TokenType.Punctuation, "}", $"Expected '}}' after block at position {Peek().Position}");

                return expressions.Count == 1 ? expressions[0] : new BlockExpression(expressions.ToArray());
            }

            private Expression ParseCaseExpression()
            {
                Consume(TokenType.Punctuation, "(", "Expected '(' after CASE");
                Expression switchValue = null;
                var cases = new List<(Expression, Expression)>();

                // Determinar el modo (comparación o condición)
                Expression firstExpr = ParseExpression();
                Token nextToken = Peek();

                if (nextToken.Type == TokenType.Punctuation && nextToken.Lexeme == ",")
                {
                    // Modo comparación: primer expresión es el valor a comparar
                    switchValue = firstExpr;
                    Advance(); // Consumir la coma

                    // Parsear casos (valor:resultado)
                    while (true)
                    {
                        var caseValue = ParseExpression();
                        Consume(TokenType.Punctuation, ":", "Expected ':' after case value");
                        var caseResult = ParseExpression();
                        cases.Add((caseValue, caseResult));

                        if (!Match(TokenType.Punctuation, ",")) break;
                    }
                }
                else if (nextToken.Type == TokenType.Punctuation && nextToken.Lexeme == ":")
                {
                    // Modo condición: primera expresión es una condición
                    Advance(); // Consumir ":"
                    var firstResult = ParseExpression();
                    cases.Add((firstExpr, firstResult));

                    // Parsear casos adicionales separados por coma
                    while (Match(TokenType.Punctuation, ","))
                    {
                        var condition = ParseExpression();
                        Consume(TokenType.Punctuation, ":", "Expected ':' after condition");
                        var result = ParseExpression();
                        cases.Add((condition, result));
                    }
                }
                else
                {
                    throw new FormulaException("Expected ',' or ':' after expression in CASE");
                }

                // Parsear cláusula DEFAULT
                Expression defaultResult = new LiteralExpression(VoidValue.Instance);
                if (Match(TokenType.Keyword, "DEFAULT"))
                {
                    Consume(TokenType.Punctuation, ":", "Expected ':' after 'DEFAULT'");
                    defaultResult = ParseExpression();
                }

                Consume(TokenType.Punctuation, ")", "Expected ')' after CASE expression");
                return new CaseExpression(switchValue, cases.ToArray(), defaultResult);
            }

            private Expression ParseForExpression()
            {
                Consume(TokenType.Punctuation, "(", "Expected '(' after FOR");

                // Parsear variable de control
                if (!Match(TokenType.Identifier))
                    throw new FormulaException("Expected variable name after '(' in FOR");
                string variable = Previous().Lexeme;

                // Parsear asignación inicial
                Consume(TokenType.Operator, ":=", "Expected ':=' in FOR initialization");
                Expression start = ParseExpression();

                // Palabra clave TO
                Consume(TokenType.Keyword, "TO", "Expected 'TO' in FOR loop");
                Expression end = ParseExpression();

                // Paso opcional (STEP)
                Expression step = null;
                if (Match(TokenType.Keyword, "STEP"))
                {
                    step = ParseExpression();
                }

                Consume(TokenType.Punctuation, ")", "Expected ')' after FOR parameters");

                // Parsear cuerpo del bucle (debe ser un bloque)
                if (!Match(TokenType.Punctuation, "{"))
                    throw new FormulaException("Expected '{' for FOR body");
                Expression body = ParseBlock();

                return new ForExpression(variable, start, end, step, body);
            }
            private int GetPrecedence(string op) => op switch
            {
                ":=" => 1,
                "|" => 2,
                "&" => 3,
                "=" or "<>" => 4,
                "<" or ">" or "<=" or ">=" => 5,
                "+" or "-" => 6,
                "*" or "/" or "%" => 7,
                "^" => 8,
                _ => 0
            };

            // Métodos auxiliares
            private Token Advance() => _tokens[_current++];
            private Token Previous() => _tokens[_current - 1];
            private Token Peek() => _tokens[_current];
            private bool Check(TokenType type, string lexeme = null)
                => !IsAtEnd() && Peek().Type == type && (lexeme == null || Peek().Lexeme == lexeme);

            private bool Match(TokenType type, string lexeme = null)
            {
                if (Check(type, lexeme))
                {
                    Advance();
                    return true;
                }
                return false;
            }

            private void Consume(TokenType type, string lexeme, string error)
            {
                if (!Match(type, lexeme)) throw new Exception(error);
            }

            private bool IsAtEnd() => Peek().Type == TokenType.EOF;
        }
        #endregion

        #region API Pública
        private readonly ExecutionContext _context = new();

        public FormulaEvaluator()
        {
            _context.InitializeConstants();
        }

        public FormulaEvaluator(ExecutionContext ctx)
        {
            _context = ctx;
            RegisterCoreFunctions();
            _context.InitializeConstants();
        }

        private void RegisterCoreFunctions()
        {
            _context.RegisterFunction("SET", 2, 2, (ctx, args) =>
            {
                if (args[0] is not StringValue variableName)
                    throw new FormulaException("SET function expects variable name as first argument");

                // Validar que el nombre de variable sea válido
                if (string.IsNullOrWhiteSpace(variableName.Val))
                    throw new FormulaException("Variable name cannot be empty");

                // Tipos especiales que no pueden asignarse directamente
                if (args[1] is TableValue<object>)
                    throw new FormulaException("Cannot assign table values directly");

                ctx.SetVariable(variableName.Val, args[1]);
                return VoidValue.Instance;
            });
            _context.RegisterFunction("GETCTA",1, 1, (ctx, args) =>
            {
                if (args[0] is not StringValue reference)
                    throw new FormulaException("GETCTA function expects reference as argument");

                var table = ctx.GetVariable("ACCOUNTS").AsTable<AccountValue>().FirstOrDefault(r => r.Reference == reference.Val);
                return table != null ? new IntegerValue(table.Account) : IntegerValue.Instance;
            });

            _context.RegisterFunction("GETSCTA", 1, 1, (ctx, args) =>
            {
                if (args[0] is not StringValue reference)
                    throw new FormulaException("GETSCTA function expects reference as argument");

                var table = ctx.GetVariable("ACCOUNTS").AsTable<AccountValue>().FirstOrDefault(r => r.Reference == reference.Val);
                return table != null ? new IntegerValue(table.SubAccount) : IntegerValue.Instance;
            });
            _context.RegisterFunction("GETSCTL", 1, 1, (ctx, args) =>
            {
                if (args[0] is not StringValue reference)
                    throw new FormulaException("GETSCTL function expects reference as argument");

                var table = ctx.GetVariable("ACCOUNTS").AsTable<AccountValue>().FirstOrDefault(r => r.Reference == reference.Val);
                return table != null ? new IntegerValue(table.SubControl) : IntegerValue.Instance;
            });
            _context.RegisterFunction("GETANAL", 1, 1, (ctx, args) =>
            {
                if (args[0] is not StringValue reference)
                    throw new FormulaException("GETANAL function expects reference as argument");

                var table = ctx.GetVariable("ACCOUNTS").AsTable<AccountValue>().FirstOrDefault(r => r.Reference == reference.Val);
                return table != null ? new IntegerValue(table.Analysis) : IntegerValue.Instance;
            });
            _context.RegisterFunction("GETEGSTO", 1, 1, (ctx, args) =>
            {
                if (args[0] is not StringValue reference)
                    throw new FormulaException("GETEGSTO function expects reference as argument");

                var table = ctx.GetVariable("EXPENSE_ITEMS").AsTable<ExpenseItemValue>().FirstOrDefault(r => r.Reference == reference.Val);
                return table != null ? new IntegerValue(table.Code) : IntegerValue.Instance;
            });
            _context.RegisterFunction("GETSCOPE", 1, 1, GetScope);

            _context.RegisterFunction("WRITE", 6, 6, (ctx, args) =>
            {
                if (args[0] is not IntegerValue n1)
                    throw new FormulaException("WRITE function expects number as first argument");
                if (args[1] is not IntegerValue n2)
                    throw new FormulaException("WRITE function expects number as second argument");
                if (args[2] is not IntegerValue n3)
                    throw new FormulaException("WRITE function expects number as third argument");
                if (args[3] is not IntegerValue n4)
                    throw new FormulaException("WRITE function expects number as fourth argument");
                if (args[4] is not DecimalValue exp)
                    throw new FormulaException("WRITE function expects decimal as fifth argument");

                var op = args[5].RawValue.To<AccountOperation>();
                int scope = GetScope(ctx, [ctx.GetVariable("SCOPE")]).Val;
                var currency = ctx.GetVariable("CURRENCY").RawValue.To<CurrencyIsoCode>();
                var values = ctx.GetVariable("NOTES").AsTable<NoteValue>();
                var row = values.FirstOrDefault(r => 
                    r.ScopeId == scope &&
                    r.Account == n1.Val &&
                    r.SubAccount == n2.Val &&
                    r.SubControl == n3.Val &&
                    r.Analysis == n4.Val &&
                    r.Currency == currency);

                if (row == null)
                {
                    row = new NoteValue(scope, n1.Val, n2.Val, n3.Val, n4.Val, currency, op)
                        { Amount = op == AccountOperation.Credit ? exp.Val * -1 : exp.Val };
                    values.Add(row);
                }
                else
                    row.Amount += op == AccountOperation.Credit ? exp.Val * -1 : exp.Val;

                return VoidValue.Instance;
            });
            _context.RegisterFunction("WRITEFULL", 3, 3, (ctx, args) =>
            {
                if (args[0] is not StringValue reference)
                    throw new FormulaException("WRITE function expects reference as first argument");
                if (args[1] is not DecimalValue exp)
                    throw new FormulaException("WRITE function expects decimal as second argument");
                var op = args[2].RawValue.To<AccountOperation>();
                int scope = GetScope(ctx, [ctx.GetVariable("SCOPE")]).Val;
                var currency = ctx.GetVariable("CURRENCY").RawValue.To<CurrencyIsoCode>();
                var account = GetAccount(ctx, reference);
                if(account is null)
                    throw new FormulaException("WRITEFULL function expects a valid reference argument");

                var values = ctx.GetVariable("NOTES").AsTable<NoteValue>();
                
                var row = values.FirstOrDefault(r =>
                    r.ScopeId == scope &&
                    r.Account == account.Account &&
                    r.SubAccount == account.SubAccount &&
                    r.SubControl == account.SubControl &&
                    r.Analysis == account.Analysis &&
                    r.Currency == currency);

                if (row == null)
                {
                    row = new NoteValue(scope, account.Account, account.SubAccount, account.SubControl,
                            account.Analysis, currency, op)
                        {Amount = op == AccountOperation.Credit ? exp.Val * -1 : exp.Val};
                    values.Add(row);
                }
                else
                    row.Amount += op == AccountOperation.Credit ? exp.Val * -1 : exp.Val;

                return VoidValue.Instance;
            });
        }

        private AccountValue GetAccount(ExecutionContext ctx, Value args)
        {
            if (args is not StringValue reference)
                throw new FormulaException("GetAccount function expects reference as argument");
            var table = ctx.GetVariable("ACCOUNTS").AsTable<AccountValue>().FirstOrDefault(r => r.Reference == reference.Val);
            return table;
        }

        private IntegerValue GetScope(ExecutionContext ctx, Value[] args)
        {
            string reference = args[0].Type == ValueType.Integer ? args[0].AsInteger().ToString() : args[0].AsString();
            var table = ctx.GetVariable("CLASSIFIERS").AsTable<ClassifierValue>()
                           .FirstOrDefault(r => r.Description == reference || r.Id.ToString() == reference);
            return table != null ? new IntegerValue(table.Id) : new IntegerValue(1);
        }

        public void RegisterFunction(string name, int minArgs, int maxArgs, FunctionImplementation impl)
            => _context.RegisterFunction(name, minArgs, maxArgs, impl);

        public Value Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return VoidValue.Instance;

            var lexer = new Lexer(expression);
            var tokens = lexer.Tokenize();
            var parser = new Parser(tokens);
            var expr = parser.Parse();
            return expr.Evaluate(_context);
        }
        #endregion
    }
}