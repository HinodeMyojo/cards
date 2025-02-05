using CardsServer.BLL.Infrastructure.Result;

namespace CardsServer.BLL.Infrastructure
{
    /// <summary>
    /// ����� ��������� ��� ������� ���������
    /// </summary>
    public interface IValidator<T>
    {
        public Result<string> Validate (T obj);
    }
}