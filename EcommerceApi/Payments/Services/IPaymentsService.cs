using System.Threading.Tasks;
using Payments.Commands;

namespace Payments.Services;

public interface IPaymentsService
{
    Task MakePayment(MakePayment command);
}