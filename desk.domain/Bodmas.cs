using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace desk.domain
{
    public class Bodmas
    {
        private List<char> operators = new List<char>();

        public Bodmas()
        {
            SetOperators();
        }

        public decimal Calculate(string equation)
        {
            equation = CleanEquation(equation);

            if (!Validate(equation))
                throw new ArgumentException("Invalid equation");

            var charList = equation.ToList();

            if (charList.Contains(Constants.leftBracket) && charList.Contains(Constants.rightBracket))
            {
                int leftBracketPosition = equation.LastIndexOf(Constants.leftBracket);
                int rightBracketPosition = equation.Substring(leftBracketPosition).IndexOf(Constants.rightBracket);

                string subEquation = equation.Substring(leftBracketPosition + 1, (leftBracketPosition + rightBracketPosition) - leftBracketPosition - 1);
                var subResult = Calculate(subEquation);

                string newEquation = equation.Replace($"({subEquation})", $"({subResult}#");
                StringBuilder newEquationBuilder = new StringBuilder(newEquation);

                if (leftBracketPosition == 0)
                {
                    newEquationBuilder.Remove(leftBracketPosition, 1);
                    newEquationBuilder.Insert(leftBracketPosition, " ");
                }
                else if (leftBracketPosition != 0 && IsInt(equation[leftBracketPosition - 1].ToString()) && !IsOperator(equation[leftBracketPosition - 1]))
                {
                    newEquationBuilder.Remove(leftBracketPosition, 1);
                    newEquationBuilder.Insert(leftBracketPosition, "*");
                }


                else
                {
                    if (newEquationBuilder[leftBracketPosition - 1] == Constants.rightBracket)
                    {
                        newEquationBuilder.Remove(leftBracketPosition, 1);
                        newEquationBuilder.Insert(leftBracketPosition, "*");
                    }
                    else
                    {
                        newEquationBuilder.Remove(leftBracketPosition, 1);
                        newEquationBuilder.Insert(leftBracketPosition, " ");
                    }

                }

                int rightSideIndex = newEquationBuilder.ToString().IndexOf('#');
                if (rightSideIndex + 1 == newEquationBuilder.Length)
                {
                    newEquationBuilder.Remove(rightSideIndex, 1);
                    newEquationBuilder.Insert(rightSideIndex, " ");
                }
                else if (rightSideIndex != equation.Length - 1 && IsInt(newEquationBuilder[(rightSideIndex + 1)].ToString()) && !IsOperator(newEquationBuilder[(rightSideIndex + 1)]))
                {
                    newEquationBuilder.Remove(rightSideIndex, 1);
                    newEquationBuilder.Insert(rightSideIndex, "*");
                }
                else if (rightSideIndex == equation.Length)
                {
                    newEquationBuilder.Remove(rightSideIndex, 1);
                    newEquationBuilder.Insert(rightSideIndex, " ");
                }
                else
                {
                    if (newEquationBuilder[rightSideIndex + 1] == Constants.leftBracket)
                    {
                        newEquationBuilder.Remove(rightSideIndex, 1);
                        newEquationBuilder.Insert(rightSideIndex, "*");
                    }
                    else
                    {
                        newEquationBuilder.Remove(rightSideIndex, 1);
                        newEquationBuilder.Insert(rightSideIndex, " ");
                    }

                }

                return Calculate(newEquationBuilder.ToString());
            }

            var result = SubCalc(equation);

            if (IsDecimal(result))
                return Convert.ToDecimal(result);

            return Calculate(result);
        }

        private string SubCalc(string equation)
        {
            if (IsDecimal(equation))
                return equation;

            var charList = equation.ToCharArray();
            int lastPositionToReplace = 1;
            decimal result = 0;

            for (int i = 1; i < charList.Length; i++)
            {
                //do multiplication/division
                if (charList[i] == Constants.multiply || charList[i] == Constants.divide)
                {
                    var operatorPosition = i;

                    //get left side value
                    StringBuilder val1 = new StringBuilder();
                    decimal dec1;
                    for (int j = 0; j < operatorPosition; j++)
                    {
                        val1.Append(charList[j].ToString());
                    }
                    dec1 = Convert.ToDecimal(val1.ToString());

                    //get right side value
                    StringBuilder val2 = new StringBuilder();
                    decimal dec2;
                    for (int j = operatorPosition + 1; j < charList.Length; j++)
                    {
                        if (j == operatorPosition + 1 && charList[j] != Constants.subtract)
                        {
                            if (IsOperator(charList[j]))
                                break;
                        }
                        else if (j != operatorPosition + 1)
                        {
                            if (IsOperator(charList[j]))
                                break;
                        }
                        lastPositionToReplace = j;

                        val2.Append(charList[j].ToString());
                    }
                    dec2 = Convert.ToDecimal(val2.ToString());

                    if (charList[i] == Constants.multiply)
                    {
                        result = Multiply(dec1, dec2);
                        break;
                    }
                    else
                    {
                        result = Divide(dec1, dec2);
                        break;
                    }
                }
                else if (charList[i] == Constants.add || charList[i] == Constants.subtract)
                {
                    var operatorPosition = i;

                    //get left side value
                    StringBuilder val1 = new StringBuilder();
                    decimal dec1;
                    for (int j = 0; j < operatorPosition; j++)
                    {
                        val1.Append(charList[j].ToString());
                    }
                    dec1 = Convert.ToDecimal(val1.ToString());

                    //get right side value
                    StringBuilder val2 = new StringBuilder();
                    decimal dec2;
                    for (int j = operatorPosition + 1; j < charList.Length; j++)
                    {
                        if (j == operatorPosition + 1 && charList[j] != Constants.subtract)
                        {
                            if (IsOperator(charList[j]))
                                break;
                        }
                        else if (j != operatorPosition + 1)
                        {
                            if (IsOperator(charList[j]))
                                break;
                        }
                        lastPositionToReplace = j;

                        val2.Append(charList[j].ToString());
                    }
                    dec2 = Convert.ToDecimal(val2.ToString());

                    if (charList[i] == Constants.add)
                    {
                        result = Add(dec1, dec2);
                        break;
                    }
                    else
                    {
                        result = Subtract(dec1, dec2);
                        break;
                    }
                }
            }

            if (lastPositionToReplace > 1)
            {
                StringBuilder equateBuilder = new StringBuilder(equation);
                equateBuilder.Remove(0, lastPositionToReplace + 1);
                equateBuilder.Insert(0, result.ToString());

                return equateBuilder.ToString();
            }

            throw new Exception("Something went wrong");
        }

        private bool Validate(string equation)
        {
            if (string.IsNullOrEmpty(equation))
                return false;

            var charList = equation.ToCharArray().ToList();

            if (!IsDecimal(charList.First().ToString()) && charList.First() != Constants.leftBracket && charList.First() != Constants.subtract)
                return false;

            if (!IsDecimal(charList.Last().ToString()) && charList.Last() != Constants.rightBracket)
                return false;

            if (charList.Count(x => x == Constants.rightBracket) != charList.Count(x => x == Constants.leftBracket))
                return false;

            bool valid = true;
            for (int i = 0; i < equation.Length; i++)
            {
                if (!IsOperator(equation[i]) && charList[i] != '.' && !IsInt(charList[i].ToString()))
                    return false;
            }

            return valid;
        }

        private string CleanEquation(string equation)
        {
            string newEquation = equation.Replace(" ", "");
            for (int i = 1; i < equation.Length; i++)
            {
                if (equation[i] == Constants.multiply || equation[i] == Constants.divide)
                {
                    if (equation[i - 1] == equation[i])
                        newEquation = equation.Remove(i, 1);
                }
            }

            return newEquation;
        }
        private bool IsOperator(char val)
        {
            return operators.Contains(val);
        }

        private bool IsDecimal(string character)
        {
            decimal value;
            if (Decimal.TryParse(character, out value))
                return true;
            else
                return false;
        }

        private bool IsInt(string character)
        {
            int value;
            if (int.TryParse(character, out value))
                return true;
            else
                return false;
        }

        private decimal Multiply(decimal val1, decimal val2)
        {
            return val1 * val2;
        }

        private decimal Divide(decimal val1, decimal val2)
        {
            return val1 / val2;
        }

        private decimal Add(decimal val1, decimal val2)
        {
            return val1 + val2;
        }

        private decimal Subtract(decimal val1, decimal val2)
        {
            return val1 - val2;
        }

        private void SetOperators()
        {
            operators.Add(Constants.leftBracket);
            operators.Add(Constants.rightBracket);
            operators.Add(Constants.multiply);
            operators.Add(Constants.divide);
            operators.Add(Constants.add);
            operators.Add(Constants.subtract);
        }
    }
}
