using HomeBanking.Models;
using HomeBanking.Models.dtos;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private ITransactionRepository _transactionRepository;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository, ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        public IActionResult Post([FromBody] LoanApplicationDTO loanAppDTO)
        {
            try
            {
                //Se obtiene el cliente con sesion iniciada
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == null)
                    return Forbid();

                //Verificar si existe el usuario
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                    return Forbid();

                //Validar que el prestamo seleccionado exista
                var loan = _loanRepository.FindById(loanAppDTO.LoanId);
                if (loan == null)
                    return Forbid("El prestamo solicitado no existe.");

                //Validar que las cuotas sean seleccionadas y la cantidad valida
                if (String.IsNullOrEmpty(loanAppDTO.Payments))
                    return Forbid("Seleccione la cantidad de cuotas.");
                var paymentList = loan.Payments.Split(',');
                if (!paymentList.Contains(loanAppDTO.Payments))
                    return Forbid("Error en las cuotas solicitadas.");

                //Validar monto
                if (loanAppDTO.Amount <= 0 || loanAppDTO.Amount > loan.MaxAmount)
                    return Forbid("Error al seleccionar monto.");

                //Verificar cuenta de destino
                if (String.IsNullOrEmpty(loanAppDTO.ToAccountNumber))
                    return Forbid("Seleccione la cuenta de destino.");
                Account toAcc = _accountRepository.FindByNumber(loanAppDTO.ToAccountNumber);
                if (toAcc == null)
                    return Forbid("La cuenta seleccionada no existe.");
                if (toAcc.ClientId != client.Id)
                    return Forbid("La cuenta no pertenece al cliente.");

                //Validar tipo de dato del dto de clientloan 
                if (!int.TryParse(loanAppDTO.Payments, out int numericPayments))
                    return StatusCode(403, "Error en tipo de datos.");

                var newClientLoan = new ClientLoan
                {
                    Amount = loanAppDTO.Amount * 1.20,
                    Payments = loanAppDTO.Payments,
                    ClientId = client.Id,
                    LoanId = loan.Id
                };

                var newClientLoanDTO = new ClientLoanDTO
                {
                    LoanId = newClientLoan.Id,
                    Name = loan.Name,
                    Amount = newClientLoan.Amount,
                    Payments = numericPayments
                };

                _clientLoanRepository.Save(newClientLoan);

                var newTransaction = new Transaction
                {
                    Type = TransactionType.CREDIT.ToString(),
                    Amount = loanAppDTO.Amount,
                    Description = $"Prestamo {loan.Name} aprobado",
                    Date = DateTime.Now,
                    AccountId = toAcc.Id,
                };

                _transactionRepository.Save(newTransaction);

                toAcc.Balance += newTransaction.Amount;
                _accountRepository.Save(toAcc);

                return Created("", newClientLoanDTO);

            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Get()
        {
            try 
            {
                var loans = _loanRepository.GetAll();
                var loansDTO = new List<LoanDTO>();
                foreach (Loan loan in loans)
                {
                    var newLoanDTO = new LoanDTO 
                    {
                        Id = loan.Id,
                        Name = loan.Name,
                        MaxAmount = loan.MaxAmount,
                        Payments = loan.Payments,
                    };
                    loansDTO.Add(newLoanDTO);
                }
                return Ok(loansDTO);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
