// Skeleton written by Joe Zachary for CS 3500, January 2015
// Revised by Joe Zachary, January 2016
// JLZ Repaired pair of mistakes, January 23, 2016

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public class Formula
    {
        private ArrayList list = new ArrayList();

        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        public Formula(String formula)
        {
            int numOfOp = 0;
            int numOfCp = 0;
            IEnumerable<string> tokens = GetTokens(formula);

            var e = tokens.First();
            var lastE = tokens.Last();

            if(e == null || e ==" ")
            {
                throw new FormulaFormatException("no elements");
            }

            if (!(lastE.Equals(")") || ifVar(lastE) || ifDouble(lastE)))
            {
                throw new FormulaFormatException("the last elements is invalid");
            }

            if (!(e.Equals("(") || ifVar(e) || ifDouble(e)))
            {
                throw new FormulaFormatException("the first elements is invalid");
            }
            bool a = true;
            int i = 0;
            string pre = "";
            foreach (string s in tokens)
            {
                
                 if(a)
                {
                    a = false;
                    pre = s;
                }
                if (s.Equals("("))
                    numOfOp++;
                if (s.Equals(")"))
                    numOfCp++;
                if (numOfCp > numOfOp)
                    throw new FormulaFormatException("wrong of using parentheses");
                if (i >= 1)
                {
                    if (pre.Equals("(")||isOp(pre))
                    {
                        if (!(s.Equals("(") || ifDouble(s) || ifVar(s)))
                            throw new FormulaFormatException("wrong token followed the openning parentheses");
                    }

                    if (ifDouble(pre) || pre.Equals((")")) || ifVar(pre))
                    {
                        if (!(s.Equals(")") || isOp(s)))
                            throw new FormulaFormatException("wrong token before the closing parentheses");
                    }
                }
                pre = s;
                list.Add(s);
                i++;           
            }

            if (numOfOp != numOfCp)
                throw new FormulaFormatException("The number opening parentheses must equal the total number of closing parentheses");

        }
        private static bool ifVar(string s)
        {
             return Regex.IsMatch(s, @"[a-zA-Z][0-9a-zA-Z]*");
        }

        private static bool ifDouble(string s)
        {
            double num;
            if (Double.TryParse(s, out num))
            {
                if (num < 0)
                    return true;
                else
                    return true;
            }
            else
                return false;
        }

        private static bool isOp(string s)
        {
            return Regex.IsMatch(s, @"[\+\-*/]");
        }

        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {

            Stack<double> num = new Stack<double>();
            Stack<string> op = new Stack<string>();

            foreach(string e in list)
            {
                if (ifDouble(e))
                {
                    double temp;
                    Double.TryParse(e,out temp);
                    if(op.Count()!=0&&op.Peek().Equals("*"))
                    {
                        double temp1 = num.Pop();
                        op.Pop();
                        num.Push(temp * temp1);
                    }
                    else if (op.Count() != 0 && op.Peek().Equals("/"))
                    {
                        double temp1 = num.Pop();
                        op.Pop();
                        if (temp1 == 0)
                            throw new FormulaEvaluationException("can not divided by 0");
                        else
                            num.Push(temp / temp1);
                    }
                    else
                        num.Push(temp);
                }

                else if (ifVar(e))
                {
                    double temp;
                    try
                    {
                        temp = lookup(e);
                    }
                    catch (UndefinedVariableException)
                    {                    
                    }
                    temp = lookup(e);
                    if (op.Count() != 0 && op.Peek().Equals("*"))
                    {
                        double temp1 = num.Pop();
                        op.Pop();
                        num.Push(temp * temp1);
                    }
                    else if (op.Count() != 0 && op.Peek().Equals("/"))
                    {
                        double temp1 = num.Pop();
                        op.Pop();
                        if (temp1 == 0)
                            throw new FormulaEvaluationException("can not divided by 0");
                        else
                            num.Push(temp / temp1);
                    }
                    else
                        num.Push(temp);
                }

                else if(e.Equals("+")||e.Equals("-"))
                {
                    if(op.Count() != 0 && op.Peek().Equals("+"))
                    {
                        op.Pop();
                        double temp1 = num.Pop();
                        double temp2 = num.Pop();
                        num.Push(temp1 + temp2);
                        op.Push(e);
                    }
                    else if (op.Count() != 0 && op.Peek().Equals("-"))
                    {
                        op.Pop();
                        double temp1 = num.Pop();
                        double temp2 = num.Pop();
                        num.Push(temp1 - temp2);
                        op.Push(e);
                    }
                    else
                        op.Push(e);
                }

                else if(e.Equals("*") || e.Equals("/"))
                {
                    op.Push(e);
                }

                else if (e.Equals("("))
                {
                    op.Push(e);
                }
                
                else if (e.Equals(")"))
                {
                    if (op.Count() != 0 && op.Peek().Equals("+"))
                    {
                        op.Pop();
                        double temp1 = num.Pop();
                        double temp2 = num.Pop();
                        num.Push(temp1 + temp2);
                        op.Push(e);
                        op.Pop();
                    }
                    else if (op.Count() != 0 && op.Peek().Equals("-"))
                    {
                        op.Pop();
                        double temp1 = num.Pop();
                        double temp2 = num.Pop();
                        num.Push(temp1 - temp2);
                        op.Push(e);
                        op.Pop();
                    }
                    else
                        op.Pop();
                    if (op.Count() != 0 && op.Peek().Equals("*"))
                    {
                        op.Pop();
                        double temp1 = num.Pop();
                        double temp2 = num.Pop();
                        num.Push(temp1 * temp2);
                    }
                    else if (op.Count() != 0 && op.Peek().Equals("/"))
                    {
                        op.Pop();
                        double temp1 = num.Pop();
                        double temp2 = num.Pop();
                        num.Push(temp1 / temp2);
                    }
                }
            }
            if (op.Count() == 0)
            {
                return num.Pop();
            }
            else
            {
                if (op.Count() != 0 && op.Peek().Equals("+"))
                {
                    double temp1 = num.Pop();
                    double temp2 = num.Pop();
                    return temp1 + temp2;
                }
                else
                {
                    double temp1 = num.Pop();
                    double temp2 = num.Pop();
                    return temp1 - temp2;
                }           
            }
                
              
           


        }

        private bool ifDouble(object e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}
