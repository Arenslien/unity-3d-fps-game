using System.Text.RegularExpressions;

// 아이디 규칙. 이메일 형식이어야 한다.
public class AccountIdSpecification
{
    // 앞부분과 도메인의 각 마디는 '.'이 처음·끝·연속으로 오지 못하고, 마지막 마디는 영문 2글자 이상.
    // 예) abc.def@gmail.com, user_01@mail.co.kr
    private static readonly Regex EmailRegex = new Regex(
        @"^[A-Za-z0-9_%+-]+(\.[A-Za-z0-9_%+-]+)*@[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,}\z");

    public string ErrorMessage => "ID must be a valid email address.";

    public bool IsSatisfiedBy(string email)
    {
        return !string.IsNullOrEmpty(email) && EmailRegex.IsMatch(email);
    }
}
