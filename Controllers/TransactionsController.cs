using HomeBanking.Models;
using HomeBanking.Models.dtos;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private ITransactionRepository _transactionRepository;
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        public TransactionsController(ITransactionRepository transactionRepository, IClientRepository clientRepository, IAccountRepository accountRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost]
        public IActionResult Post([FromBody] TransferDTO transferDTO)
        {
            try
            {
                //Se obtiene el cliente con sesion iniciada
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == null)
                    return Forbid("Email vacio.");

                //Verificar si existe el usuario
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                    return Forbid("El cliente no existe.");

                if (String.IsNullOrEmpty(transferDTO.FromAccountNumber) || String.IsNullOrEmpty(transferDTO.ToAccountNumber))
                    return Forbid("Cuenta de origen o cuenta de destino no proporcionada.");

                if (transferDTO.FromAccountNumber == transferDTO.ToAccountNumber)
                    return Forbid("No se permite la transferencia a la misma cuenta.");

                if (transferDTO.Amount == 0 || String.IsNullOrEmpty(transferDTO.Description))
                    return Forbid("Monto o descripción no proporcionados.");

                //Buscamos las cuentas
                Account fromAcc = _accountRepository.FindByNumber(transferDTO.FromAccountNumber);
                if (fromAcc == null)
                    return Forbid("La cuenta de origen no existe.");
                Account toAcc = _accountRepository.FindByNumber(transferDTO.ToAccountNumber);
                if (toAcc == null)
                    return Forbid("La cuenta destino no existe.");

                //Controlamos el monto
                if (transferDTO.Amount > fromAcc.Balance)
                    return Forbid("Fondos insuficientes");

                _transactionRepository.Save(new Transaction
                {
                    Type = TransactionType.DEBIT.ToString(),
                    Amount = transferDTO.Amount * -1,
                    Description = transferDTO.Description + " " + toAcc.Number,
                    Date = DateTime.Now,
                    AccountId = fromAcc.Id
                });

                _transactionRepository.Save(new Transaction
                {
                    Type = TransactionType.CREDIT.ToString(),
                    Amount = transferDTO.Amount,
                    Description = transferDTO.Description + " " + fromAcc.Number,
                    Date = DateTime.Now,
                    AccountId = toAcc.Id
                });

                //Seteamos los valores a las cuentas y actualizamos
                fromAcc.Balance = fromAcc.Balance - transferDTO.Amount;
                _accountRepository.Save(fromAcc);
                toAcc.Balance = toAcc.Balance + transferDTO.Amount;
                _accountRepository.Save(toAcc);

                return Created("Creado con exito", fromAcc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

    }
}
