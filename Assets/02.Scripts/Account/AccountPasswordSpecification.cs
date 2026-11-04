using System.Text.RegularExpressions;

// 비밀번호 규칙. 8~30글자이고, 대문자·소문자·기호(영문·숫자·공백이 아닌 글자)를 각각 하나 이상 포함해야 한다.
public class AccountPasswordSpecification
{
    private static readonly Regex PasswordRegex = new Regex(
        @"^(?=.*[A-Z])(?=.*[a-z])(?=.*[^A-Za-z0-9\s]).{8,30}\z");

    public string ErrorMessage => "Password: 8-30 chars, upper, lower & symbol.";

    public bool IsSatisfiedBy(string password)
    {
        return !string.IsNullOrEmpty(password) && PasswordRegex.IsMatch(password);
    }
}
