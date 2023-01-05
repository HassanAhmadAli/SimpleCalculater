//create by hassan ali 2023-01-06 
//dotnet version 6
// Here is a general outline of the steps to validate an arithmetic expression and calculate its value:
//todo: split the input string into a list of individual elements, such as numbers, operators, and parentheses.
//todo: Check the balance of parentheses in the expression. If there are more left parentheses than right parentheses, or vice versa, the expression is invalid.
//todo: Check the syntax of the expression. Make sure that all the numbers and operators are placed correctly and that there are no extraneous characters. If the syntax is invalid, the expression is invalid.
//todo: use the shunting-yard algorithm. This involves reading the elements from left to right and using two stacks to store values and operators. As you read each element, perform the following actions:
//todo: If the element is a number, push it onto the values stack.
//todo: If the element is an operator, push it onto the operators stack. However, before pushing the operator, first apply any existing operators on the stack with higher or equal precedence to the values stack.
//todo: If the element is a left parenthesis, push it onto the operators stack.
//todo: If the element is a right parenthesis, pop all operators from the stack until the matching left parenthesis is encountered and apply them to the values stack.
//todo: When all the elements have been read, pop any remaining operators from the stack and apply them to the values stack.
//todo: The final value on the stack is the result of evaluating the expression. If there are multiple values left on the stack, or if the stack is empty, the expression is invalid.
namespace ConsoleCalculator;
class Calculator
{
    private static void Error(string Message)
    {
        Console.WriteLine("Invalid Expression \nReason:\n" + Message);
    }
    static void Main()
    {
        // if the expression is invalid the program return applicationException that tell the reason of the Error
        try
        {
            //here we add zero to prevent the input of the funcion from being null or ""
            try
            {
                Console.WriteLine(EvaluateExpression(Console.ReadLine()));
            }
            catch (InvalidOperationException error)
            {
                Error(error.Message);
            }
        }
        catch (ApplicationException error)
        {
            Error(error.Message);
        }
        catch (DivideByZeroException error)
        {
            Error(error.Message);
        }
    }
    static double EvaluateExpression(string rawInput)
    {
        string input = "0";
        for (int i = 0; i < rawInput.Length; ++i)
        {
            //here we add " +0 " befor the "-" to prevent `Stack Empty` Exception in Expression like `-1`
            if (rawInput[i] == '-') input += "+0-";
            else
                input += rawInput[i];
        }
        //The EvaluateExpression method uses two stacks,
        //one to hold values and one to hold operators,
        //and the other to evaluate the expression. It does this by iterating through the characters in the input string and performing the following actions:
        Stack<double> values = new();
        Stack<char> ops = new();
        int inputLength = input.Length;
        for (int i = 0; i < inputLength; ++i)
        {
            char c = input[i];
            if (c == ' ')
                continue;
            // If the character is a left parenthesis, it is pushed onto the ops stack.
            else if (c == '(')
                ops.Push(c);
            //  If the character is a digit, it is pushed onto the values stack.
            else if (Char.IsDigit(c))
            {
                string s = "";
                bool flag = true;
                while (i < inputLength && (Char.IsDigit(input[i]) || (input[i] == '.')))
                {
                    if (input[i] == '.')
                    {
                        if (flag) flag = false;
                        else throw new ApplicationException("two dots in a row ");
                    }
                    s += input[i++];
                }
                i--;
                values.Push(double.Parse(s));
            }
            //  If the character is an operator, it is pushed onto the ops stack after any existing operators with higher or equal precedence have been applied to the values stack.
            else if (c == '+' || c == '-' || c == '*' || c == '/' || c == '^')
            {
                while (ops.Count != 0 && HasPrecedence(c, ops.Peek()))
                    values.Push(ApplyOperation(ops.Pop(), values.Pop(), values.Pop()));
                ops.Push(c);
            }
            //  If the character is a right parenthesis, all operators on the ops stack up until the matching left parenthesis are applied to the values stack.
            else if (c == ')')
            {
                while (ops.Peek() != '(')
                    values.Push(ApplyOperation(ops.Pop(), values.Pop(), values.Pop()));
                ops.Pop();
            }
        }
        while (ops.Count != 0)
            values.Push(ApplyOperation(ops.Pop(), values.Pop(), values.Pop()));
        return values.Pop();
    }
    // returns true if the first operator has higher precedence than the second, and false otherwise.
    static bool HasPrecedence(char op1, char op2)
    {
        if ((op2 == '(' || op2 == ')' || op1 == '^') || ((op1 == '*' || op1 == '/') && (op2 == '+' || op2 == '-')))
            return false;
        return true;
    }
    //applies the given operator to the two given values.
    static double ApplyOperation(char operand, double rhs, double lhs)
    {
        switch (operand)
        {
            case '+':
                return lhs + rhs;
            case '-':
                return lhs - rhs;
            case '*':
                return lhs * rhs;
            case '/':
                if (rhs == 0)
                    throw new DivideByZeroException();
                return lhs / rhs;
            case '^':
                if (lhs >= 0 || (lhs < 0 && ((rhs % 1 )!= 0)))
                    return Math.Pow(lhs, rhs);
                else throw new ApplicationException("Rise Negative Number to non integer power");
        }
        return 0;
    }
}
