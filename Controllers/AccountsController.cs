using HomeBanking.Models;
using HomeBanking.Models.dtos;
using HomeBanking.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountRepository _accountRepository;

        public AccountsController (IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try 
            {
                var accounts = _accountRepository.GetAllAccounts();
                var accountsDTO = new List<AccountDTO>();
                foreach (Account account in accounts) 
                {
                    var newAccountDTO = new AccountDTO
                    {
                        Id = account.Id,
                        Number = account.Number,
                        CreationDate = account.CreationDate,
                        Balance = account.Balance,
                        Transactions = account.Transactions.Select(ts => new TransactionDTO
                        {
                            Id = ts.Id,
                            Type = ts.Type,
                            Amount = ts.Amount,
                            Description = ts.Description,
                            Date = ts.Date,
                        }).ToList()
                    };
                    accountsDTO.Add(newAccountDTO);
                }
                return Ok(accountsDTO);
            } 
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("{id}")]
        public IActionResult Get(long id) 
        {
            try 
            {
                var account = _accountRepository.FindById(id);
                if (account == null) 
                {
                    return NotFound();
                }
                var accountDTO = new AccountDTO
                {
                    Id = account.Id,
                    Number = account.Number,
                    CreationDate = account.CreationDate,
                    Balance = account.Balance,
                    Transactions = account.Transactions.Select(ts => new TransactionDTO
                    {
                        Id = ts.Id,
                        Type = ts.Type,
                        Amount = ts.Amount,
                        Description = ts.Description,
                        Date = ts.Date,
                    }).ToList()
                };
                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAccountsByClient(long clientId)
        {
            try 
            {
                var accounts = _accountRepository.GetByClient(clientId);
                if (accounts == null)
                    return Forbid();

                var accountsDTO = new List<AccountDTO>();

                foreach (Account account in accounts) 
                {
                    var newAccountDTO = new AccountDTO
                    {
                        Id = account.Id,
                        Number = account.Number,
                        CreationDate = account.CreationDate,
                        Balance = account.Balance,
                        Transactions = account.Transactions.Select(ts => new TransactionDTO
                        {
                            Id = ts.Id,
                            Type = ts.Type,
                            Amount = ts.Amount,
                            Description = ts.Description,
                            Date = ts.Date,
                        }).ToList()
                    };
                    accountsDTO.Add(newAccountDTO);
                }
                return Ok(accountsDTO);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }

        }

        //Creacion de cuenta:
        [HttpPost]
        public AccountDTO Post(long clientId) 
        {
            Random random = new Random();
            string number;
            Account numberAux;
            try
            {
                //Se crean los datos para la nueva cuenta y se valida que no se repita el numero de cuenta
                do
                {
                    number = "VIN-" + random.Next(100000, 999999).ToString();
                    numberAux = _accountRepository.FindByNumber(number);
                } while (numberAux != null);

                Account newAccount = new Account
                {
                    Number = number,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = clientId,
                };
                _accountRepository.Save(newAccount);
                //Se crea el DTO que sera enviado al front
                AccountDTO newAccountDTO = new AccountDTO
                {
                    Id = newAccount.ClientId,
                    Number = newAccount.Number,
                    CreationDate = newAccount.CreationDate,
                    Balance = newAccount.Balance,
                };
                    
                return newAccountDTO;
            }
            catch
            {
                return null;
            }
        }
    }

}
