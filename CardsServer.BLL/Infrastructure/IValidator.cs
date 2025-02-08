using CardsServer.BLL.Infrastructure.Result;

namespace CardsServer.BLL.Infrastructure
{
    /// <summary>
    /// ����� ��������� ��� ������� ���������
    /// </summary>
    public interface IValidator
    {
        public Result<string> Validate<T> (T obj);
    }
}